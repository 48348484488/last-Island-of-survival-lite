using System;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using SC.UI;
using Share;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

public class DoorMgr : Singleton<DoorMgr>
{
	private readonly Dictionary<string, DoorInfo> _dicInstanceId2DoorObj = new Dictionary<string, DoorInfo>();

	private readonly Dictionary<string, SBuildingStatusChange> _dicInstanceId2DoorData = new Dictionary<string, SBuildingStatusChange>();

	private readonly HashSet<long> _setOpenedLockInstanceId = new HashSet<long>();

	private readonly HashSet<long> _setCreateLockInstanceId = new HashSet<long>();

	public int OtherPlayerLayer;

	private const int Close = 0;

	private const int Open1 = 3;

	private const int Open2 = 1;

	private long _aimDoorInstanceId;

	private readonly COpenDoor _cOpenDoor = new COpenDoor();

	private readonly CCloseDoor _cCloseDoor = new CCloseDoor();

	public void Init()
	{
		OtherPlayerLayer = 1 << LayerMask.NameToLayer("OtherPlayerCollider");
		SBuildingStatusChange.handler = (SBuildingStatusChange.Handler)Delegate.Combine(SBuildingStatusChange.handler, new SBuildingStatusChange.Handler(OnSBuildingStatusChange));
		SPutLockOnDoor.handler = (SPutLockOnDoor.Handler)Delegate.Combine(SPutLockOnDoor.handler, new SPutLockOnDoor.Handler(OnSPutLockOnDoor));
		SInputDoorPassword.handler = (SInputDoorPassword.Handler)Delegate.Combine(SInputDoorPassword.handler, new SInputDoorPassword.Handler(OnSInputDoorPassword));
		SSetDoorPassword.handler = (SSetDoorPassword.Handler)Delegate.Combine(SSetDoorPassword.handler, new SSetDoorPassword.Handler(OnSSetDoorPassword));
		EventHandlers.OnBuildPartUpdate = (EventHandlers.BuildPartUpdate)Delegate.Combine(EventHandlers.OnBuildPartUpdate, new EventHandlers.BuildPartUpdate(OnBuildUpgrade));
		EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildFinish));
		AddOnBattlePanelShowEvent();
	}

	private void OnSSetDoorPassword(SSetDoorPassword msg)
	{
		if (_setCreateLockInstanceId.Contains(msg.id))
		{
			return;
		}
		_setOpenedLockInstanceId.Remove(msg.id);
		if (_aimDoorInstanceId == msg.id && IsClose(_aimDoorInstanceId))
		{
			if (EventHandlers.OnRemoveExtraBtn != null)
			{
				EventHandlers.OnRemoveExtraBtn(100);
			}
			if (EventHandlers.OnAddExtraBtn != null)
			{
				EventHandlers.OnAddExtraBtn(103);
			}
		}
	}

	private void OnSPutLockOnDoor(SPutLockOnDoor msg)
	{
		_setCreateLockInstanceId.Add(msg.id);
		_setOpenedLockInstanceId.Add(msg.id);
	}

	private void OnBuildFinish(long InsId, BuildPart buildPartCfg, int status, Octets extraInfoOc)
	{
		if (buildPartCfg.functionType != 4)
		{
			return;
		}
		GameObject partGoByInsID = SingletonMono<BuildManager>.Ins.GetPartGoByInsID(InsId);
		if (!partGoByInsID)
		{
			return;
		}
		DoorInfo component = partGoByInsID.GetComponent<DoorInfo>();
		if (component == null)
		{
			Debug.LogError("[DoorMgr.cs]No DoorInfo.cs get.");
			return;
		}
		component.InitDoorData(InsId, buildPartCfg.lv, status);
		AddDoorInfo(component);
		DoorCanOpenList doorCanOpenList = new DoorCanOpenList();
		doorCanOpenList.unmarshal(extraInfoOc);
		long roleId = Singleton<RoleMgr>.Ins.info.roleId;
		if (doorCanOpenList.lockRoleId == roleId)
		{
			_setOpenedLockInstanceId.Add(InsId);
			_setCreateLockInstanceId.Add(InsId);
		}
		else if (doorCanOpenList.roleIds.Contains(roleId))
		{
			_setOpenedLockInstanceId.Add(InsId);
		}
	}

	public bool IsLockOwner(long instanceId)
	{
		return _setCreateLockInstanceId.Contains(instanceId);
	}

	private void OnBuildUpgrade(long InsId, BuildPart buildPartCfg)
	{
		if (!IsDoor(InsId))
		{
			return;
		}
		DoorInfo value;
		if (_dicInstanceId2DoorObj.TryGetValue(InsId.ToString(), out value))
		{
			value.Level = buildPartCfg.lv;
			if (IsHasLock(value.State))
			{
				value.AddLock(cfg.Consts.PASSWORD_LOCK_ITEM_ID, value.State);
			}
			if (!IsClose(value.State))
			{
				value.OpenDoor(value.State, false);
			}
		}
		else
		{
			Debug.LogError("[DoorInfo.cs]Not Fount Upgrade Door.");
		}
	}

	public void AddDoorInfo(DoorInfo doorInfo)
	{
		string key = doorInfo.InstanceId.ToString();
		_dicInstanceId2DoorObj[key] = doorInfo;
		SBuildingStatusChange value;
		if (_dicInstanceId2DoorData.TryGetValue(key, out value) && GetDoorOpenState(value.status) != GetDoorOpenState(doorInfo.State))
		{
			Debug.LogError("[DoorInfo.cs]Status in SBuildingStatusChange is not the same with OnBuildPart.");
		}
	}

	private int GetDoorOpenState(int state)
	{
		if ((state & 2) <= 0)
		{
			return 0;
		}
		if ((state & 4) > 0)
		{
			return 3;
		}
		return 1;
	}

	private void AddOnBattlePanelShowEvent()
	{
		ViewMgr.Ins.AddOnShowEvent<BattlePanel>(DelayedAttachEvent);
	}

	private void DelayedAttachEvent()
	{
		EventHandlers.OnAimedPart = (Utils.LongDelegate)Delegate.Combine(EventHandlers.OnAimedPart, new Utils.LongDelegate(OnAimBuilding));
		ViewMgr.Ins.RemoveOnShowEvent("BattlePanel", DelayedAttachEvent);
	}

	private void OnAimBuilding(long arg)
	{
		_aimDoorInstanceId = arg;
		if (IsDoor(arg) && EventHandlers.OnAddExtraBtn != null)
		{
			if (IsClose(arg) && !IsLocked(arg))
			{
				EventHandlers.OnAddExtraBtn(100);
			}
			else if (!IsClose(arg))
			{
				EventHandlers.OnAddExtraBtn(101);
			}
		}
	}

	public bool IsDoor(long instanceId)
	{
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(instanceId);
		return partByInsID != null && partByInsID.MyCfg != null && partByInsID.MyCfg.functionType == 4;
	}

	private void OnSInputDoorPassword(SInputDoorPassword msg)
	{
		if (msg.success)
		{
			_setOpenedLockInstanceId.Add(msg.id);
			DoorInfo value;
			if (_dicInstanceId2DoorObj.TryGetValue(msg.id.ToString(), out value))
			{
				OpenDoor(value, IsLocked(msg.id));
			}
		}
		else
		{
			AlertBox.Show(214);
		}
	}

	private void OnSBuildingStatusChange(SBuildingStatusChange msg)
	{
		if (!IsDoor(msg.id))
		{
			return;
		}
		string key = msg.id.ToString();
		SBuildingStatusChange value;
		if (_dicInstanceId2DoorData.TryGetValue(key, out value))
		{
			value.status = msg.status;
		}
		else
		{
			value = new SBuildingStatusChange();
			value.status = msg.status;
			_dicInstanceId2DoorData.Add(key, value);
		}
		DoorInfo value2;
		if (!_dicInstanceId2DoorObj.TryGetValue(key, out value2))
		{
			return;
		}
		if (IsClose(value2.State) && !IsClose(msg.status))
		{
			value2.OpenDoor(msg.status, true);
		}
		else if (!IsClose(value2.State) && IsClose(msg.status))
		{
			value2.CloseDoor(msg.status, true);
		}
		else if (!IsHasLock(value2.State) && IsHasLock(msg.status))
		{
			value2.AddLock(cfg.Consts.PASSWORD_LOCK_ITEM_ID, msg.status);
		}
		else if (IsHasLock(value2.State) && !IsHasLock(msg.status))
		{
			value2.RemoveLock(msg.status);
			_setCreateLockInstanceId.Remove(msg.id);
		}
		if (_aimDoorInstanceId != msg.id)
		{
			return;
		}
		if (EventHandlers.OnRemoveExtraBtn != null)
		{
			EventHandlers.OnRemoveExtraBtn(101);
			EventHandlers.OnRemoveExtraBtn(100);
		}
		if (EventHandlers.OnAddExtraBtn != null && !IsLocked(_aimDoorInstanceId))
		{
			if (IsClose(value2.State))
			{
				EventHandlers.OnAddExtraBtn(100);
			}
			else
			{
				EventHandlers.OnAddExtraBtn(101);
			}
		}
	}

	public bool IsClose(int status)
	{
		return (status & 1) <= 0;
	}

	public bool IsClose(long instanceId)
	{
		DoorInfo value;
		if (_dicInstanceId2DoorObj.TryGetValue(instanceId.ToString(), out value))
		{
			return IsClose(value.State);
		}
		return false;
	}

	public bool IsHasLock(int status)
	{
		return (status & 4) > 0;
	}

	public bool IsHasLock(long instanceId)
	{
		DoorInfo value;
		if (_dicInstanceId2DoorObj.TryGetValue(instanceId.ToString(), out value))
		{
			return IsHasLock(value.State);
		}
		return false;
	}

	public bool IsLocked(long instanceId)
	{
		return IsHasLock(instanceId) && !_setOpenedLockInstanceId.Contains(instanceId);
	}

	public bool IsOpen_1(int status)
	{
		return (status & 2) > 0;
	}

	public void DestroyDoorObj(DoorInfo doorInfo)
	{
		DoorInfo value;
		if (_dicInstanceId2DoorObj.TryGetValue(doorInfo.InstanceId.ToString(), out value) && doorInfo == value)
		{
			_dicInstanceId2DoorObj.Remove(doorInfo.InstanceId.ToString());
		}
	}

	public void OpenDoor()
	{
		DoorInfo value;
		_dicInstanceId2DoorObj.TryGetValue(_aimDoorInstanceId.ToString(), out value);
		OpenDoor(value, value != null && IsLocked(value.InstanceId));
	}

	public void CloseDoor()
	{
		DoorInfo value;
		_dicInstanceId2DoorObj.TryGetValue(_aimDoorInstanceId.ToString(), out value);
		CloseDoor(value);
	}

	private void OpenDoor(DoorInfo doorInfo, bool isLocked)
	{
		if (!(doorInfo == null) && (!isLocked || _setOpenedLockInstanceId.Contains(doorInfo.InstanceId)) && doorInfo.CanOperation)
		{
			_cOpenDoor.id = doorInfo.InstanceId;
			_cOpenDoor.direction = GetOpenDirection(doorInfo.transform);
			Client2Gs.Ins.Send(_cOpenDoor);
		}
	}

	private bool GetOpenDirection(Transform door)
	{
		return door.InverseTransformPoint(Battle.Ins.SelfPlayer.Pos).z > 0f;
	}

	public void CloseDoor(DoorInfo doorInfo)
	{
		if (!(doorInfo == null) && doorInfo.CanOperation)
		{
			_cCloseDoor.id = doorInfo.InstanceId;
			Client2Gs.Ins.Send(_cCloseDoor);
		}
	}
}
