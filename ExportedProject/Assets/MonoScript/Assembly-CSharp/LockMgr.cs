using System;
using EasyBuildSystem.Runtimes.Events;
using SC.UI;
using cfg;
using gs.battle.scmsg;

public class LockMgr : Singleton<LockMgr>
{
	private long _curOpenInstanceId;

	private CInputDoorPassword _cInputDoorPassword = new CInputDoorPassword();

	private CSetDoorPassword _cSetDoorPassword = new CSetDoorPassword();

	private CPutLockOnDoor _cPutLockOnDoor = new CPutLockOnDoor();

	private CTakeLockOffDoor _cTakeLockOffDoor = new CTakeLockOffDoor();

	public void Init()
	{
		SBuildingStatusChange.handler = (SBuildingStatusChange.Handler)Delegate.Combine(SBuildingStatusChange.handler, new SBuildingStatusChange.Handler(OnSBuildingStatusChange));
		SLockCooling.handler = (SLockCooling.Handler)Delegate.Combine(SLockCooling.handler, new SLockCooling.Handler(OnSLockCooling));
		AddOnBattlePanelShowEvent();
	}

	private void OnSLockCooling(SLockCooling msg)
	{
		AlertBox.Show(280, Utils.GetCountDownTime(msg.remainSeconds));
	}

	private void OnSBuildingStatusChange(SBuildingStatusChange msg)
	{
		long id = msg.id;
		if (_curOpenInstanceId != id)
		{
			return;
		}
		if (EventHandlers.OnRemoveExtraBtn != null)
		{
			EventHandlers.OnRemoveExtraBtn(102);
			EventHandlers.OnRemoveExtraBtn(103);
			EventHandlers.OnRemoveExtraBtn(104);
			EventHandlers.OnRemoveExtraBtn(105);
		}
		if (EventHandlers.OnAddExtraBtn == null)
		{
			return;
		}
		if (IsHasLock(id))
		{
			if (Singleton<DoorMgr>.Ins.IsLockOwner(id))
			{
				EventHandlers.OnAddExtraBtn(102);
				EventHandlers.OnAddExtraBtn(105);
			}
			if (Singleton<DoorMgr>.Ins.IsLocked(id))
			{
				EventHandlers.OnAddExtraBtn(103);
			}
		}
		else if (CanAddLock(id) && Singleton<FriendPermitMgr>.Ins.HavePermitByInstanceId(id))
		{
			EventHandlers.OnAddExtraBtn(104);
		}
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

	private bool CanAddLock(long instanceId)
	{
		return Singleton<DoorMgr>.Ins.IsDoor(instanceId);
	}

	private bool IsHasLock(long instanceId)
	{
		return Singleton<DoorMgr>.Ins.IsHasLock(instanceId);
	}

	private void OnAimBuilding(long arg)
	{
		if (!CanAddLock(arg))
		{
			return;
		}
		_curOpenInstanceId = arg;
		if (EventHandlers.OnAddExtraBtn == null)
		{
			return;
		}
		if (IsHasLock(arg))
		{
			if (Singleton<DoorMgr>.Ins.IsLockOwner(arg))
			{
				EventHandlers.OnAddExtraBtn(102);
				EventHandlers.OnAddExtraBtn(105);
			}
			if (Singleton<DoorMgr>.Ins.IsLocked(arg))
			{
				EventHandlers.OnAddExtraBtn(103);
			}
		}
		else if (CanAddLock(arg) && Singleton<FriendPermitMgr>.Ins.HavePermitByInstanceId(arg))
		{
			EventHandlers.OnAddExtraBtn(104);
		}
	}

	public void ShowInputPassword()
	{
		if (IsHasLock(_curOpenInstanceId))
		{
			ViewMgr.Ins.ShowView<LockPanel>(103, false);
		}
	}

	public void ShowAddLock()
	{
		if (Singleton<BagMgr>.Ins.GetItemNum(cfg.Consts.PASSWORD_LOCK_ITEM_ID) <= 0)
		{
			AlertBox.Show(247);
		}
		else if (!IsHasLock(_curOpenInstanceId))
		{
			ViewMgr.Ins.ShowView<LockPanel>(104, false);
		}
	}

	public void ShowChangePassword()
	{
		if (IsHasLock(_curOpenInstanceId))
		{
			ViewMgr.Ins.ShowView<LockPanel>(102, false);
		}
	}

	public void SendInputPasswordMsg(string password)
	{
		_cInputDoorPassword.id = _curOpenInstanceId;
		_cInputDoorPassword.password = password;
		Client2Gs.Ins.Send(_cInputDoorPassword);
	}

	public void SendChangePasswordMsg(string password)
	{
		_cSetDoorPassword.id = _curOpenInstanceId;
		_cSetDoorPassword.password = password;
		Client2Gs.Ins.Send(_cSetDoorPassword);
	}

	public void SendPutLockOnDoor(string password)
	{
		_cPutLockOnDoor.id = _curOpenInstanceId;
		_cPutLockOnDoor.password = password;
		Client2Gs.Ins.Send(_cPutLockOnDoor);
	}

	public void SendRemoveLock()
	{
		_cTakeLockOffDoor.id = _curOpenInstanceId;
		Client2Gs.Ins.Send(_cTakeLockOffDoor);
	}
}
