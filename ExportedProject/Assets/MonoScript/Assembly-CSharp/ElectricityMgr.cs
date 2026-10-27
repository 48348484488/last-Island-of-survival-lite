using System;
using System.Collections;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using SC.UI;
using Share;
using UnityEngine;
using cfg;
using gs.battle.map.circuitry.scmsg;
using gs.battle.scmsg;

public class ElectricityMgr : Singleton<ElectricityMgr>
{
	private const string SelfGoNameStart = "Self";

	private const string OtherGoNameStart = "role";

	private const string OtherPlayerExactColLayerName = "Player";

	private const string SelfPlayerExactColLayerName = "SelfPlayer";

	private const float LineRenderEndPosDefaultZ = 3.14f;

	private readonly Vector3[] _lineRenderPositions = new Vector3[2];

	private readonly Vector3 YaLiKaiGuanHalfExtends = new Vector3(0.55f, 0.075f, 0.55f);

	private readonly float RedLineLength = 3f;

	private long _aimedInsId;

	private readonly Dictionary<long, CircuitryInfo> _dicInsId2dataInfoAll = new Dictionary<long, CircuitryInfo>();

	private readonly Dictionary<long, HashSet<long>> _dicChildId2ParentIds = new Dictionary<long, HashSet<long>>();

	private readonly Dictionary<long, byte> _dicInsId2DelayTime = new Dictionary<long, byte>();

	private readonly HashSet<long> _onSwitchIds = new HashSet<long>();

	private readonly HashSet<long> _connectBatteryIds = new HashSet<long>();

	private readonly HashSet<long> _lastInsIds = new HashSet<long>();

	public int PlayerExactColLayerMask;

	public int UnIncludePlayerExactColLayerMask;

	private readonly Dictionary<long, IElectricElement> _dicInsId2Ele = new Dictionary<long, IElectricElement>();

	private readonly Dictionary<long, KeyValuePair<Vector3, Vector3>> _dicInsId2PosForward = new Dictionary<long, KeyValuePair<Vector3, Vector3>>();

	private readonly Dictionary<long, Vector3> _dicInsId2PosYaLi = new Dictionary<long, Vector3>();

	private readonly Dictionary<long, int> _dicInsId2BuildingStatus = new Dictionary<long, int>();

	private readonly Collider[] _switchCos = new Collider[3];

	private readonly RaycastHit[] _hitInfos = new RaycastHit[1];

	private readonly RaycastHit[] _exactHitInfos = new RaycastHit[1];

	private readonly float DelayTime = 2f;

	private readonly Dictionary<long, float> _dicInsId2DelaySendMsg = new Dictionary<long, float>();

	private readonly COnOrOffElementSwitch _cOnOrOffElementSwitch = new COnOrOffElementSwitch();

	private readonly CGetTreeByElementBuildingId _cGetTreeByElementBuildingId = new CGetTreeByElementBuildingId();

	private readonly CConnection _cConnection = new CConnection();

	private readonly CDisconnect _cDisconnect = new CDisconnect();

	private readonly CRequestPowerInfo _cRequestPowerInfo = new CRequestPowerInfo();

	private readonly CGetFuel _cGetFuel = new CGetFuel();

	private readonly CAddFuel _cAddFuel = new CAddFuel();

	private readonly CRequestSwitchInfo _cRequestSwitchInfo = new CRequestSwitchInfo();

	private readonly CSetSwitchDelay _cSetSwitchDelay = new CSetSwitchDelay();

	public HashSet<long> LastInsIds
	{
		get
		{
			return _lastInsIds;
		}
	}

	public void Init()
	{
		_lineRenderPositions[0] = new Vector3(0f, 0f, 0f);
		_lineRenderPositions[1] = new Vector3(0f, 0f, 3.14f);
		PlayerExactColLayerMask = (1 << LayerMask.NameToLayer("SelfPlayer")) | (1 << LayerMask.NameToLayer("Player"));
		UnIncludePlayerExactColLayerMask = -1 ^ PlayerExactColLayerMask ^ (1 << LayerMask.NameToLayer("Window"));
		Singleton<StructureMenuMgr>.Ins.AddPermitFunc(120, Singleton<FriendPermitMgr>.Ins.HavePermitByInstanceId);
		Singleton<TrapMgr>.Ins.AddMonitorAct(15, new TrapMgr.MonitorDelegates
		{
			AddMonitorAct = AddMonitorYaLiKaiGuan,
			RemoveMonitorAct = RemoveMonitorYaLiKaiGuan,
			CanAddMonitorFunc = CanAddMonitor
		});
		Singleton<TrapMgr>.Ins.AddMonitorAct(17, new TrapMgr.MonitorDelegates
		{
			AddMonitorAct = AddMonitorHongWai,
			RemoveMonitorAct = RemoveMonitorHongWai,
			CanAddMonitorFunc = null
		});
		BattleEvent.OnClickUseBuild = (Utils.IntLongDelegate)Delegate.Combine(BattleEvent.OnClickUseBuild, new Utils.IntLongDelegate(OnClickUseBtn));
		SGetTreeByElementBuildingId.handler = (SGetTreeByElementBuildingId.Handler)Delegate.Combine(SGetTreeByElementBuildingId.handler, new SGetTreeByElementBuildingId.Handler(OnSGetTreeByElementBuildingId));
		SConnection.handler = (SConnection.Handler)Delegate.Combine(SConnection.handler, new SConnection.Handler(OnSConnection));
		SDisconnect.handler = (SDisconnect.Handler)Delegate.Combine(SDisconnect.handler, new SDisconnect.Handler(OnSDisconnect));
		EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildFinish));
		EventHandlers.OnDestroyedPart += OnDestroyedPart;
		SBuildingStatusChange.handler = (SBuildingStatusChange.Handler)Delegate.Combine(SBuildingStatusChange.handler, new SBuildingStatusChange.Handler(OnSBuildingStatusChange));
		SRequestPowerInfo.handler = (SRequestPowerInfo.Handler)Delegate.Combine(SRequestPowerInfo.handler, new SRequestPowerInfo.Handler(OnSRequestPowerInfo));
		SOnOrOffElementSwitch.handler = (SOnOrOffElementSwitch.Handler)Delegate.Combine(SOnOrOffElementSwitch.handler, new SOnOrOffElementSwitch.Handler(OnSOnOrOffElementSwitch));
		SChangeBuildingName.handler = (SChangeBuildingName.Handler)Delegate.Combine(SChangeBuildingName.handler, new SChangeBuildingName.Handler(OnSChangeBuildingName));
		SceneEvent.InitLoadSceneFinish = (Utils.VoidDelegate)Delegate.Combine(SceneEvent.InitLoadSceneFinish, new Utils.VoidDelegate(OnStartBattle));
		SRequestSwitchInfo.handler = (SRequestSwitchInfo.Handler)Delegate.Combine(SRequestSwitchInfo.handler, new SRequestSwitchInfo.Handler(OnSRequestSwitchInfo));
		SSetSwitchDelay.handler = (SSetSwitchDelay.Handler)Delegate.Combine(SSetSwitchDelay.handler, new SSetSwitchDelay.Handler(OnSSetSwitchDelay));
	}

	private void OnSSetSwitchDelay(SSetSwitchDelay msg)
	{
		_dicInsId2DelayTime[msg.instanceId] = msg.delay;
		if (ElectricityEvent.OnSwitchDelayTimeChangeSuccessDelegate != null)
		{
			ElectricityEvent.OnSwitchDelayTimeChangeSuccessDelegate();
		}
	}

	private void OnSRequestSwitchInfo(SRequestSwitchInfo msg)
	{
		_dicInsId2DelayTime[msg.switchInsId] = msg.delay;
		if (ElectricityEvent.OnSwitchDelayTimeRefreshDelegate != null)
		{
			ElectricityEvent.OnSwitchDelayTimeRefreshDelegate(msg.switchInsId, msg.delay);
		}
	}

	private void OnStartBattle()
	{
		Battle.StartConroutine(CheckForSwitch());
	}

	private void AddMonitorYaLiKaiGuan(long insId, GameObject go)
	{
		_dicInsId2PosYaLi[insId] = go.transform.position;
	}

	private void RemoveMonitorYaLiKaiGuan(long insId)
	{
		_dicInsId2PosYaLi.Remove(insId);
	}

	private bool CanAddMonitor(long insId)
	{
		int value;
		return _dicInsId2BuildingStatus.TryGetValue(insId, out value) && ElectricTool.IsConnectToBattery(value);
	}

	private void AddMonitorHongWai(long insId, GameObject go)
	{
		HongWaiXianKaiGuan component = go.GetComponent<HongWaiXianKaiGuan>();
		if ((bool)component && (bool)component.Line)
		{
			_dicInsId2PosForward.Add(insId, new KeyValuePair<Vector3, Vector3>(component.Line.position, component.Line.forward));
		}
	}

	private void RemoveMonitorHongWai(long insId)
	{
		_dicInsId2PosForward.Remove(insId);
	}

	private IEnumerator CheckForSwitch()
	{
		while (true)
		{
			yield return Utils.WaitForSeconds(0.1f);
			foreach (KeyValuePair<long, Vector3> item in _dicInsId2PosYaLi)
			{
				int value;
				if (!_dicInsId2BuildingStatus.TryGetValue(item.Key, out value) || !IsSwitchClosing(value))
				{
					bool flag = Physics.OverlapBoxNonAlloc(item.Value, YaLiKaiGuanHalfExtends, _switchCos, Quaternion.identity, Singleton<LandMineMgr>.Ins.PlayerLayerMask) > 0;
					if (flag != ElectricTool.IsSwitchOn(_dicInsId2BuildingStatus[item.Key]))
					{
						SendSwitchOnOffMsg(item.Key, flag);
					}
				}
			}
			foreach (KeyValuePair<long, KeyValuePair<Vector3, Vector3>> item2 in _dicInsId2PosForward)
			{
				int value2;
				IElectricElement value3;
				if ((_dicInsId2BuildingStatus.TryGetValue(item2.Key, out value2) && IsSwitchClosing(value2)) || !_dicInsId2Ele.TryGetValue(item2.Key, out value3))
				{
					continue;
				}
				HongWaiXianKaiGuan hongWaiXianKaiGuan = value3 as HongWaiXianKaiGuan;
				if (!hongWaiXianKaiGuan || !hongWaiXianKaiGuan.LineGo || !hongWaiXianKaiGuan.LineGo.activeSelf)
				{
					continue;
				}
				bool flag2 = Physics.RaycastNonAlloc(item2.Value.Key, item2.Value.Value, _hitInfos, RedLineLength, UnIncludePlayerExactColLayerMask, QueryTriggerInteraction.Ignore) > 0;
				if (flag2)
				{
					if (!_hitInfos[0].transform.name.StartsWith("Self") && !_hitInfos[0].transform.name.StartsWith("role"))
					{
						_lineRenderPositions[1].z = hongWaiXianKaiGuan.Line.InverseTransformPoint(_hitInfos[0].point).z;
						hongWaiXianKaiGuan.LineR.SetPositions(_lineRenderPositions);
					}
					else if (Physics.SphereCastNonAlloc(item2.Value.Key, 0.05f, item2.Value.Value, _exactHitInfos, RedLineLength, PlayerExactColLayerMask) > 0)
					{
						_lineRenderPositions[1].z = hongWaiXianKaiGuan.Line.InverseTransformPoint(_exactHitInfos[0].point).z;
						hongWaiXianKaiGuan.LineR.SetPositions(_lineRenderPositions);
					}
					else
					{
						_lineRenderPositions[1].z = 3.14f;
						hongWaiXianKaiGuan.LineR.SetPositions(_lineRenderPositions);
						flag2 = false;
					}
				}
				else
				{
					_lineRenderPositions[1].z = 3.14f;
					hongWaiXianKaiGuan.LineR.SetPositions(_lineRenderPositions);
				}
				if (flag2 != ElectricTool.IsSwitchOn(_dicInsId2BuildingStatus[item2.Key]))
				{
					SendSwitchOnOffMsg(item2.Key, flag2);
				}
			}
		}
	}

	private bool IsSwitchClosing(int status)
	{
		return (status & 4) > 0;
	}

	public void SendSwitchOnOffMsg(long insId, bool isOn)
	{
		_cOnOrOffElementSwitch.instanceId = insId;
		_cOnOrOffElementSwitch.isOn = isOn;
		Client2Gs.Ins.Send(_cOnOrOffElementSwitch);
		byte value;
		_dicInsId2DelayTime.TryGetValue(insId, out value);
		if (value == 0 && IsDelaySwitch(insId))
		{
			value = (byte)cfg.Consts.ELECTRIC_DELAY_SWITCH_DEFAULT_TIME;
		}
		_dicInsId2DelaySendMsg[insId] = Time.realtimeSinceStartup + DelayTime + (float)(int)value;
	}

	public bool IsSwitchOn(long insId)
	{
		int value;
		if (_dicInsId2BuildingStatus.TryGetValue(insId, out value))
		{
			return ElectricTool.IsSwitchOn(value);
		}
		return false;
	}

	private void OnSChangeBuildingName(SChangeBuildingName msg)
	{
		CircuitryInfo info;
		if (GetInfoByInsId(msg.insId, out info))
		{
			info.name = msg.name;
		}
	}

	private void OnSOnOrOffElementSwitch(SOnOrOffElementSwitch msg)
	{
		_dicInsId2DelaySendMsg.Remove(msg.instanceId);
		if (msg.isOn)
		{
			_onSwitchIds.Add(msg.instanceId);
		}
		else
		{
			_onSwitchIds.Remove(msg.instanceId);
		}
		if (IsElectrify(msg.instanceId))
		{
			ConnectChangeOnSwitchOnOff(msg.instanceId, msg.isOn);
			if (ElectricityEvent.OnSwitchStateChangeDelegate != null)
			{
				ElectricityEvent.OnSwitchStateChangeDelegate();
			}
		}
	}

	private void OnSRequestPowerInfo(SRequestPowerInfo msg)
	{
		if (ElectricityEvent.UpdateFuelInfoDelegate != null)
		{
			ElectricityEvent.UpdateFuelInfoDelegate(msg.powerId, msg.remainTime, msg.fuels);
		}
	}

	private void OnSBuildingStatusChange(SBuildingStatusChange msg)
	{
		_dicInsId2DelaySendMsg.Remove(msg.id);
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(msg.id);
		if (!(partByInsID != null))
		{
			return;
		}
		CircuitryCfg circuitryCfg = CircuitryCfg.Get(partByInsID.LV1Id);
		if (circuitryCfg == null)
		{
			return;
		}
		_dicInsId2BuildingStatus[msg.id] = msg.status;
		IElectricElement value;
		if (_dicInsId2Ele.TryGetValue(msg.id, out value))
		{
			value.OnStatusChange(msg.status);
		}
		if (circuitryCfg.canOpenClose)
		{
			if (ElectricTool.IsSwitchOn(msg.status))
			{
				_onSwitchIds.Add(msg.id);
			}
			else
			{
				_onSwitchIds.Remove(msg.id);
			}
			if (IsElectrify(msg.id))
			{
				ConnectChangeOnSwitchOnOff(msg.id, IsSwitchOn((int)msg.status));
			}
		}
	}

	private void OnDestroyedPart(PartBehaviour part)
	{
		CircuitryCfg circuitryCfg = CircuitryCfg.Get(part.CfgId);
		if (circuitryCfg != null)
		{
			long insId = part.InsId;
			_dicInsId2BuildingStatus.Remove(insId);
			_dicInsId2Ele.Remove(insId);
			_onSwitchIds.Remove(insId);
			if (IsElectrify(insId))
			{
				ConnectChange(insId, false);
			}
			CircuitryInfo info;
			if (GetInfoByInsId(insId, out info) && info.childIds.Count > 0)
			{
				RemoveAllParent(insId);
				_dicChildId2ParentIds.Remove(insId);
				RemoveAllChild(insId);
			}
			_dicInsId2dataInfoAll.Remove(insId);
		}
	}

	private void ConnectChangeOnSwitchOnOff(long insId, bool isOn)
	{
		Queue<long> queue = new Queue<long>();
		queue.Enqueue(insId);
		while (queue.Count > 0)
		{
			long insId2 = queue.Dequeue();
			CircuitryInfo info;
			if (!GetInfoByInsId(insId2, out info))
			{
				continue;
			}
			List<long> childIds = info.childIds;
			int i = 0;
			for (int count = childIds.Count; i < count; i++)
			{
				long num = childIds[i];
				int num2 = ParentTransElectricNum(num);
				if (isOn && num2 == 1)
				{
					_connectBatteryIds.Add(num);
					queue.Enqueue(num);
				}
				else if (num2 <= 0)
				{
					_connectBatteryIds.Remove(num);
					queue.Enqueue(num);
				}
			}
		}
	}

	private void ConnectChange(long insId, bool isConnect)
	{
		Queue<long> queue = new Queue<long>();
		queue.Enqueue(insId);
		while (queue.Count > 0)
		{
			long num = queue.Dequeue();
			int num2 = ParentTransElectricNum(num);
			if (isConnect && num2 == 1)
			{
				_connectBatteryIds.Add(num);
				EnqueueChildIds(num, queue);
			}
			else if (num2 <= 0)
			{
				CircuitryInfo info;
				GetInfoByInsId(num, out info);
				BuildPart buildPart = BuildPart.Get(info.cfgId);
				if (IsDianChi(buildPart.functionType))
				{
					_connectBatteryIds.Add(num);
					EnqueueChildIds(num, queue);
				}
				else
				{
					_connectBatteryIds.Remove(num);
					EnqueueChildIds(num, queue);
				}
			}
		}
	}

	private void EnqueueChildIds(long curId, Queue<long> ids)
	{
		CircuitryInfo info;
		if (GetInfoByInsId(curId, out info))
		{
			List<long> childIds = info.childIds;
			int i = 0;
			for (int count = childIds.Count; i < count; i++)
			{
				ids.Enqueue(childIds[i]);
			}
		}
	}

	private int ParentTransElectricNum(long insId)
	{
		int num = 0;
		CircuitryInfo info;
		GetInfoByInsId(insId, out info);
		HashSet<long> parentIds;
		if (GetParentIds(insId, out parentIds))
		{
			foreach (long item in parentIds)
			{
				if (IsElectrify(item) && (!IsSwitch(item) || _onSwitchIds.Contains(item)))
				{
					num++;
				}
			}
			return num;
		}
		return num;
	}

	private void OnBuildFinish(long insId, BuildPart buildPartCfg, int status, Octets extraInfoOc)
	{
		CircuitryCfg circuitryCfg = CircuitryCfg.Get(buildPartCfg.id);
		if (circuitryCfg == null)
		{
			return;
		}
		_dicInsId2BuildingStatus[insId] = status;
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(insId);
		if ((bool)partByInsID)
		{
			IElectricElement component = partByInsID.GetComponent<IElectricElement>();
			if (component != null)
			{
				_dicInsId2Ele[insId] = component;
				component.OnStatusChange(status);
			}
		}
		if (circuitryCfg.canOpenClose && ElectricTool.IsSwitchOn(status))
		{
			_onSwitchIds.Add(insId);
		}
	}

	private void OnSDisconnect(SDisconnect msg)
	{
		long parentId = msg.parentId;
		long childId = msg.childId;
		CircuitryInfo info;
		if (GetInfoByInsId(childId, out info))
		{
			RemoveParentFromChild(parentId, childId);
			if (IsChildElectrify(parentId))
			{
				ConnectChange(childId, false);
			}
			RemoveChildFromParent(parentId, msg.childId);
			if (ElectricityEvent.DisconnectDelegate != null)
			{
				ElectricityEvent.DisconnectDelegate(parentId, msg.childId);
			}
		}
	}

	private void AddChildToParent(long parentInsId, long childInsId)
	{
		HashSet<long> parentIds;
		CircuitryInfo info;
		if (parentInsId == 0 && GetParentIds(childInsId, out parentIds) && GetInfoByInsId(parentInsId, out info))
		{
			CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
			if (circuitryCfg != null && parentIds.Count >= circuitryCfg.parentCount)
			{
				return;
			}
		}
		if (GetInfoByInsId(parentInsId, out info))
		{
			List<long> childIds = info.childIds;
			childIds.Add(childInsId);
			CircuitryCfg circuitryCfg2 = CircuitryCfg.Get(info.cfgId);
			if (info.instanceId > 0 && childIds.Count == circuitryCfg2.childCount)
			{
				LastInsIds.Remove(parentInsId);
			}
		}
	}

	private void AddParentToChild(long parentInsId, long childInsId)
	{
		HashSet<long> parentIds;
		if (!GetParentIds(childInsId, out parentIds))
		{
			parentIds = new HashSet<long>();
			_dicChildId2ParentIds[childInsId] = parentIds;
		}
		parentIds.Add(parentInsId);
		CircuitryInfo info;
		if (GetInfoByInsId(parentInsId, out info))
		{
			CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
			if (parentIds.Count == circuitryCfg.parentCount)
			{
				RemoveChildFromParent(0L, childInsId);
				RemoveParentFromChild(0L, childInsId);
			}
		}
	}

	private void RemoveChildFromParent(long parentInsId, long childInsId)
	{
		CircuitryInfo info;
		if (GetInfoByInsId(parentInsId, out info))
		{
			List<long> childIds = info.childIds;
			childIds.Remove(childInsId);
			CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
			if (parentInsId > 0 && childIds.Count < circuitryCfg.childCount)
			{
				LastInsIds.Add(parentInsId);
			}
		}
	}

	private void RemoveParentFromChild(long parentInsId, long childInsId)
	{
		HashSet<long> parentIds;
		if (!GetParentIds(childInsId, out parentIds))
		{
			return;
		}
		parentIds.Remove(parentInsId);
		CircuitryInfo info;
		if (parentInsId > 0 && GetInfoByInsId(parentInsId, out info))
		{
			CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
			if (parentIds.Count == circuitryCfg.parentCount - 1)
			{
				AddChildToParent(0L, childInsId);
			}
		}
	}

	private void RemoveAllParent(long childId)
	{
		HashSet<long> parentIds;
		if (!GetParentIds(childId, out parentIds))
		{
			return;
		}
		foreach (long item in parentIds)
		{
			RemoveChildFromParent(childId, item);
		}
	}

	private void RemoveAllChild(long parentId)
	{
		CircuitryInfo info;
		if (GetInfoByInsId(parentId, out info))
		{
			List<long> childIds = info.childIds;
			int i = 0;
			for (int count = childIds.Count; i < count; i++)
			{
				RemoveParentFromChild(parentId, childIds[i]);
			}
		}
	}

	private void OnSConnection(SConnection msg)
	{
		if (msg.errorCode == 1)
		{
			AlertBox.Show(324);
			return;
		}
		long childId = msg.childId;
		long parentId = msg.parentId;
		AddParentToChild(parentId, childId);
		AddChildToParent(parentId, childId);
		if (IsChildElectrify(parentId))
		{
			ConnectChange(childId, true);
		}
		if (ElectricityEvent.ConnectDelegate != null)
		{
			ElectricityEvent.ConnectDelegate(parentId, childId);
		}
	}

	private void OnSGetTreeByElementBuildingId(SGetTreeByElementBuildingId msg)
	{
		List<CircuitryInfo> circuitryInfos = msg.circuitryInfos;
		CircuitryInfo circuitryInfo = new CircuitryInfo();
		circuitryInfo.instanceId = 0L;
		_dicInsId2dataInfoAll[0L] = circuitryInfo;
		HashSet<long> hashSet = new HashSet<long>();
		HashSet<long> hashSet2 = new HashSet<long>();
		HashSet<long> hashSet3 = new HashSet<long>();
		int i = 0;
		for (int count = circuitryInfos.Count; i < count; i++)
		{
			CircuitryInfo circuitryInfo2 = circuitryInfos[i];
			long instanceId = circuitryInfo2.instanceId;
			CircuitryCfg circuitryCfg = CircuitryCfg.Get(circuitryInfo2.cfgId);
			_dicInsId2dataInfoAll[instanceId] = circuitryInfo2;
			List<long> childIds = circuitryInfo2.childIds;
			BuildPart buildPart = BuildPart.Get(circuitryInfo2.cfgId);
			if (buildPart == null)
			{
				Debug.LogError("[ElectricityMgr.cs]Can't find partCfgInfo with id : " + circuitryInfo2.cfgId);
				continue;
			}
			if (!IsDianChi(buildPart.functionType))
			{
				if (!hashSet.Contains(instanceId))
				{
					AddChildToParent(0L, instanceId);
					hashSet.Add(instanceId);
				}
				foreach (long item in childIds)
				{
					AddParentToChild(instanceId, item);
					if (!CanLinkChildForChildCount(instanceId))
					{
						hashSet3.Add(instanceId);
						LastInsIds.Remove(instanceId);
					}
				}
			}
			else
			{
				hashSet2.Add(instanceId);
				foreach (long item2 in childIds)
				{
					AddParentToChild(instanceId, item2);
					if (!CanLinkChildForChildCount(instanceId))
					{
						hashSet3.Add(instanceId);
						LastInsIds.Remove(instanceId);
					}
				}
			}
			if (!hashSet3.Contains(instanceId) && circuitryCfg != null && circuitryCfg.childCount > 0)
			{
				LastInsIds.Add(instanceId);
			}
		}
		foreach (long item3 in hashSet2)
		{
			ConnectChange(item3, true);
		}
		ViewMgr.Ins.ShowView<DianluPanel>(_aimedInsId, false);
	}

	private bool IsSwitch(long insId)
	{
		CircuitryInfo info;
		if (!GetInfoByInsId(insId, out info))
		{
			return false;
		}
		CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
		if (circuitryCfg == null)
		{
			return false;
		}
		return circuitryCfg.canOpenClose;
	}

	private void OnClickUseBtn(int itemId, long insId)
	{
		_aimedInsId = insId;
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		if (itemCfg != null && itemCfg.childType == 120)
		{
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(insId);
			if (partByInsID.ToolBoxId <= 0)
			{
				AlertBox.Show(311);
			}
			else
			{
				SendGetInfoMsg(insId);
			}
		}
	}

	public void SendGetInfoMsg(long insId)
	{
		_cGetTreeByElementBuildingId.elementBuildingId = insId;
		Client2Gs.Ins.Send(_cGetTreeByElementBuildingId);
	}

	public void SendConnectMsg(long parentInsId, long childInsId)
	{
		_cConnection.parentId = parentInsId;
		_cConnection.childId = childInsId;
		Client2Gs.Ins.Send(_cConnection);
	}

	public void SendDisconnectMsg(long parentInsId, long childInsId)
	{
		_cDisconnect.parentId = parentInsId;
		_cDisconnect.childId = childInsId;
		Client2Gs.Ins.Send(_cDisconnect);
	}

	public void SendRequestPowerInfoMsg(long insId)
	{
		_cRequestPowerInfo.powerId = insId;
		Client2Gs.Ins.Send(_cRequestPowerInfo);
	}

	public void SendGetFuelMsg(long insId, byte index)
	{
		_cGetFuel.powerId = insId;
		_cGetFuel.gridIndex = index;
		Client2Gs.Ins.Send(_cGetFuel);
	}

	public void SendAddFuelMsg(long insId, byte index, int itemId, int num)
	{
		_cAddFuel.powerId = insId;
		_cAddFuel.gridIndex = index;
		_cAddFuel.fuelId = itemId;
		_cAddFuel.fuelNum = num;
		Client2Gs.Ins.Send(_cAddFuel);
	}

	public void SendRequestSwitchInfoMsg(long insId)
	{
		_cRequestSwitchInfo.switchInsId = insId;
		Client2Gs.Ins.Send(_cRequestSwitchInfo);
	}

	public void SendSetSwitchDelayMsg(long insId, byte delay)
	{
		_cSetSwitchDelay.instanceId = insId;
		_cSetSwitchDelay.delay = delay;
		Client2Gs.Ins.Send(_cSetSwitchDelay);
	}

	public void OnPanelHide()
	{
		_dicInsId2dataInfoAll.Clear();
		_dicChildId2ParentIds.Clear();
		_lastInsIds.Clear();
		_connectBatteryIds.Clear();
		_dicInsId2DelayTime.Clear();
	}

	public bool GetInfoByInsId(long insId, out CircuitryInfo info)
	{
		return _dicInsId2dataInfoAll.TryGetValue(insId, out info);
	}

	public bool ContainsInsIdDataInfo(long insId)
	{
		return _dicInsId2dataInfoAll.ContainsKey(insId);
	}

	public bool GetParentIds(long insId, out HashSet<long> parentIds)
	{
		return _dicChildId2ParentIds.TryGetValue(insId, out parentIds);
	}

	public bool CanLinkChildForChildCount(long insId)
	{
		CircuitryInfo info;
		if (GetInfoByInsId(insId, out info))
		{
			CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
			if (circuitryCfg == null || circuitryCfg.childCount <= 0)
			{
				return false;
			}
			return info.childIds.Count < circuitryCfg.childCount;
		}
		return false;
	}

	public bool IsElectrify(long insId)
	{
		return _connectBatteryIds.Contains(insId);
	}

	public bool IsChildElectrify(long insId)
	{
		return IsElectrify(insId) && (!IsSwitch(insId) || IsSwitchOn(insId));
	}

	public bool IsDianChi(int functionType)
	{
		return functionType == 13;
	}

	public bool IsDianChi(long insId)
	{
		CircuitryInfo info;
		if (GetInfoByInsId(insId, out info))
		{
			BuildPart buildPart = BuildPart.Get(info.cfgId);
			return buildPart != null && IsDianChi(buildPart.functionType);
		}
		return false;
	}

	private bool IsDelaySwitch(long insId)
	{
		CircuitryInfo info;
		if (GetInfoByInsId(insId, out info))
		{
			return IsDelaySwitch(CircuitryCfg.Get(info.cfgId));
		}
		return false;
	}

	public bool IsDelaySwitch(CircuitryCfg eleCfgInfo)
	{
		return eleCfgInfo != null && eleCfgInfo.canOpenClose && eleCfgInfo.isDelaySwitch;
	}

	public bool GetDelaySwitchTime(long insId, out byte delayTime)
	{
		return _dicInsId2DelayTime.TryGetValue(insId, out delayTime);
	}

	public int GetTotalWattByHeadInsId(long insId, bool ignoreConnection)
	{
		int num = 0;
		HashSet<long> hashSet = new HashSet<long>();
		Queue<long> queue = new Queue<long>();
		queue.Enqueue(insId);
		hashSet.Add(insId);
		while (queue.Count > 0)
		{
			long insId2 = queue.Dequeue();
			CircuitryInfo info;
			if (!GetInfoByInsId(insId2, out info))
			{
				continue;
			}
			List<long> childIds = info.childIds;
			int i = 0;
			for (int count = childIds.Count; i < count; i++)
			{
				long item = childIds[i];
				if (!hashSet.Contains(item))
				{
					hashSet.Add(item);
					queue.Enqueue(item);
				}
			}
			CircuitryCfg circuitryCfg = CircuitryCfg.Get(info.cfgId);
			BuildPart buildPart = BuildPart.Get(info.cfgId);
			if (!IsDianChi(buildPart.functionType) && (ignoreConnection || IsElectrify(insId2)))
			{
				num += circuitryCfg.power;
			}
		}
		return num;
	}
}
