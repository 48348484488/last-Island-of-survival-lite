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
using gs.battle.map.steeltrap.scmsg;
using gs.battle.scmsg;

public class SteelTrapMgr : Singleton<SteelTrapMgr>
{
	private readonly Collider[] _playerCols = new Collider[2];

	private readonly Vector3 _halfExtends = new Vector3(0.3f, 0.3f, 0.3f);

	private Coroutine _check;

	private long _aimedSteelTrap;

	private readonly Dictionary<long, SteelTrap> _dicInstanceId2Anim = new Dictionary<long, SteelTrap>();

	private readonly Dictionary<long, Transform> _dicCanAttackInsId2Trans = new Dictionary<long, Transform>();

	private readonly HashSet<long> _setCanAttack = new HashSet<long>();

	private readonly WaitForSeconds _wait01 = new WaitForSeconds(0.1f);

	private readonly CRolesOnSteelTrap _cRolesOnSteelTrap = new CRolesOnSteelTrap();

	private readonly CRoleOpenSteelTrap _cRoleOpenSteelTrap = new CRoleOpenSteelTrap();

	public void Init()
	{
		Singleton<TrapMgr>.Ins.AddMonitorAct(7, new TrapMgr.MonitorDelegates
		{
			AddMonitorAct = AddToDicForAttack,
			RemoveMonitorAct = RemoveMonitor,
			CanAddMonitorFunc = CanAttack
		});
		EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildFinish));
		SBuildingStatusChange.handler = (SBuildingStatusChange.Handler)Delegate.Combine(SBuildingStatusChange.handler, new SBuildingStatusChange.Handler(OnSBuildingStatusChange));
		ViewMgr.Ins.AddOnShowEvent<BattlePanel>(StartBattle);
	}

	private void OnBuildFinish(long insId, BuildPart buildPartCfg, int status, Octets extraInfoOc)
	{
		if (IsSteelTrap(buildPartCfg.functionType))
		{
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(insId);
			SteelTrap steelTrap = null;
			if (partByInsID != null)
			{
				steelTrap = partByInsID.GetComponent<SteelTrap>();
				_dicInstanceId2Anim[insId] = steelTrap;
				steelTrap.AnimCtrl.enabled = true;
			}
			if (CanAttack(status))
			{
				_setCanAttack.Add(insId);
			}
			else if ((bool)steelTrap)
			{
				steelTrap.PlayOpenToCloseAnim();
			}
		}
	}

	private void AddToDicForAttack(long instanceId, GameObject part)
	{
		_dicCanAttackInsId2Trans[instanceId] = part.transform;
	}

	private void RemoveMonitor(long instanceId)
	{
		_dicCanAttackInsId2Trans.Remove(instanceId);
		_dicInstanceId2Anim.Remove(instanceId);
	}

	private bool CanAttack(long instanceId)
	{
		return _setCanAttack.Contains(instanceId);
	}

	private void OnSBuildingStatusChange(SBuildingStatusChange msg)
	{
		if (!IsSteelTrap(msg.id))
		{
			return;
		}
		SteelTrap value;
		_dicInstanceId2Anim.TryGetValue(msg.id, out value);
		bool flag = _aimedSteelTrap == msg.id;
		if (CanAttack(msg.status))
		{
			_setCanAttack.Add(msg.id);
			if ((bool)value)
			{
				value.PlayCloseToOpenAnim();
			}
			if (flag && EventHandlers.OnRemoveExtraBtn != null)
			{
				EventHandlers.OnRemoveExtraBtn(109);
			}
		}
		else
		{
			_setCanAttack.Remove(msg.id);
			if ((bool)value)
			{
				value.PlayOpenToCloseAnim();
			}
			if (flag && EventHandlers.OnAddExtraBtn != null)
			{
				EventHandlers.OnAddExtraBtn(109);
			}
		}
	}

	private bool IsSteelTrap(long id)
	{
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(id);
		return partByInsID != null && partByInsID.MyCfg != null && IsSteelTrap(partByInsID.MyCfg.functionType);
	}

	private bool IsSteelTrap(int functionType)
	{
		return functionType == 7;
	}

	private void StartBattle()
	{
		EventHandlers.OnAimedPart = (Utils.LongDelegate)Delegate.Combine(EventHandlers.OnAimedPart, new Utils.LongDelegate(OnAimBuilding));
		_check = Battle.StartConroutine(CheckForLandMine());
		ViewMgr.Ins.RemoveOnShowEvent("BattlePanel", StartBattle);
	}

	private void OnAimBuilding(long arg)
	{
		_aimedSteelTrap = arg;
		if (!IsSteelTrap(arg))
		{
			return;
		}
		if (!CanAttack(arg))
		{
			if (EventHandlers.OnAddExtraBtn != null)
			{
				EventHandlers.OnAddExtraBtn(109);
			}
		}
		else if (EventHandlers.OnRemoveExtraBtn != null)
		{
			EventHandlers.OnRemoveExtraBtn(109);
		}
	}

	private IEnumerator CheckForLandMine()
	{
		while (true)
		{
			yield return _wait01;
			foreach (KeyValuePair<long, Transform> dicCanAttackInsId2Tran in _dicCanAttackInsId2Trans)
			{
				long roleId;
				if (!dicCanAttackInsId2Tran.Value)
				{
					_setCanAttack.Remove(dicCanAttackInsId2Tran.Key);
				}
				else if (CanAttack(dicCanAttackInsId2Tran.Key) && CubeCast(dicCanAttackInsId2Tran.Value.position, dicCanAttackInsId2Tran.Value.rotation, out roleId))
				{
					SendRolesOnSteelTrap(dicCanAttackInsId2Tran.Key, roleId);
				}
			}
		}
	}

	private bool CubeCast(Vector3 center, Quaternion rotation, out long roleId)
	{
		roleId = 0L;
		int num = Physics.OverlapBoxNonAlloc(center, _halfExtends, _playerCols, rotation, Singleton<LandMineMgr>.Ins.PlayerLayerMask);
		for (int i = 0; i < num; i++)
		{
			BasePlayerController componentInParent = _playerCols[i].GetComponentInParent<BasePlayerController>();
			if ((bool)componentInParent)
			{
				roleId = componentInParent.RoleId;
				return true;
			}
		}
		return false;
	}

	private bool CanAttack(int state)
	{
		return state == 0;
	}

	private void SendRolesOnSteelTrap(long instanceId, long roleId)
	{
		_cRolesOnSteelTrap.steelTrapId = instanceId;
		_cRolesOnSteelTrap.onRoleId = roleId;
		Client2Gs.Ins.Send(_cRolesOnSteelTrap);
	}

	public void SendRoleOpenSteelTrap(long instanceId)
	{
		_cRoleOpenSteelTrap.steelTrapId = instanceId;
		Client2Gs.Ins.Send(_cRoleOpenSteelTrap);
	}
}
