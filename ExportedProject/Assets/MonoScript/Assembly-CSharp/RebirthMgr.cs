using System;
using System.Collections.Generic;
using SC.UI;
using UnityEngine;
using cfg;
using gs.bag.scmsg;
using gs.battle.drop.scmsg;
using gs.battle.scmsg;

public class RebirthMgr : Singleton<RebirthMgr>
{
	public Dictionary<long, RebirthPos> RebirthPosDic = new Dictionary<long, RebirthPos>();

	public Dictionary<long, float> _realTime = new Dictionary<long, float>();

	private List<long> _rebirthFacilityList = new List<long>();

	public long KillerRoleId;

	public SKillPlayer KillPlayer = new SKillPlayer();

	public List<ItemInfo> DieDropItemList = new List<ItemInfo>();

	public Dictionary<int, int> DropedItemsOffline = new Dictionary<int, int>();

	public float RebirthRemainTime;

	public float RebirthRemainTimeReatime;

	private CRebirth _cRebirth = new CRebirth();

	public List<long> RebirthFacilityList
	{
		get
		{
			return _rebirthFacilityList;
		}
	}

	public void Init()
	{
		SRebirthPosInfo.handler = (SRebirthPosInfo.Handler)Delegate.Combine(SRebirthPosInfo.handler, new SRebirthPosInfo.Handler(SRebirthPosInfoHandle));
		SBuildPart.handler = (SBuildPart.Handler)Delegate.Combine(SBuildPart.handler, new SBuildPart.Handler(SBuildPartHandle));
		SLeaveBuilding.handler = (SLeaveBuilding.Handler)Delegate.Combine(SLeaveBuilding.handler, new SLeaveBuilding.Handler(SLeaveBuildingHandle));
		SKillPlayer.handler = (SKillPlayer.Handler)Delegate.Combine(SKillPlayer.handler, new SKillPlayer.Handler(SKillPlayerHandle));
		SRebirth.handler = (SRebirth.Handler)Delegate.Combine(SRebirth.handler, new SRebirth.Handler(SRebirthHandle));
		SBattleLoginFinish.handler = (SBattleLoginFinish.Handler)Delegate.Combine(SBattleLoginFinish.handler, new SBattleLoginFinish.Handler(SBattleLoginFinishHandle));
		SSelfRebirth.handler = (SSelfRebirth.Handler)Delegate.Combine(SSelfRebirth.handler, new SSelfRebirth.Handler(SSelfRebirthHandle));
		SSelfDie.handler = (SSelfDie.Handler)Delegate.Combine(SSelfDie.handler, new SSelfDie.Handler(SSelfDieHandle));
		SDestoryBuilding.handler = (SDestoryBuilding.Handler)Delegate.Combine(SDestoryBuilding.handler, new SDestoryBuilding.Handler(SDestoryBuildingHandle));
		SDropedItemsDuringOffline.handler = (SDropedItemsDuringOffline.Handler)Delegate.Combine(SDropedItemsDuringOffline.handler, new SDropedItemsDuringOffline.Handler(SDropedItemsDuringOfflineHandle));
		SChangeBuildingName.handler = (SChangeBuildingName.Handler)Delegate.Combine(SChangeBuildingName.handler, new SChangeBuildingName.Handler(SChangeBuildingNameHandle));
		ViewMgr.Ins.AddOnShowEvent<BattlePanel>(ShowRebirthPanel);
	}

	private void ShowRebirthPanel()
	{
		if ((bool)Battle.Ins)
		{
			if (Battle.Ins.SelfInfo.hp <= 0)
			{
				ViewMgr.Ins.ShowView<DeathPanel>(null, false);
			}
			else
			{
				Singleton<RebirthMgr>.Ins.ShowDropedItemsOffline();
			}
		}
	}

	private void SChangeBuildingNameHandle(SChangeBuildingName msg)
	{
		if (RebirthPosDic.ContainsKey(msg.insId))
		{
			RebirthPosDic[msg.insId].name = msg.name;
		}
	}

	private void SDropedItemsDuringOfflineHandle(SDropedItemsDuringOffline msg)
	{
		DropedItemsOffline = msg.dropedItems;
	}

	public void ShowDropedItemsOffline()
	{
		if (DropedItemsOffline.Count > 0)
		{
			ViewMgr.Ins.ShowView<DeathOutPanel>(null, false);
		}
	}

	public void ClearDropedItemsOffline()
	{
		DropedItemsOffline.Clear();
	}

	private void SDestoryBuildingHandle(SDestoryBuilding msg)
	{
		RebirthPosDic.Remove(msg.instanceId);
		_realTime.Remove(msg.instanceId);
		_rebirthFacilityList.Remove(msg.instanceId);
		_rebirthFacilityList.Sort(Sort);
	}

	private void SSelfDieHandle(SSelfDie msg)
	{
		DieDropItemList = new List<ItemInfo>(msg.dropItems);
		RebirthRemainTime = ConstsBs.AUTO_REBIRTH_TIME;
		RebirthRemainTimeReatime = Time.realtimeSinceStartup;
		ViewMgr.Ins.ShowView<DeathPanel>(null, false);
	}

	private void SSelfRebirthHandle(SSelfRebirth msg)
	{
		if (msg.buildingId > 0)
		{
			OpenCd(msg.buildingId);
		}
	}

	private void SBattleLoginFinishHandle(SBattleLoginFinish msg)
	{
		RebirthRemainTime = msg.timeToRebirth;
		RebirthRemainTimeReatime = Time.realtimeSinceStartup;
	}

	private void SRebirthHandle(SRebirth msg)
	{
	}

	public void OpenCd(long instanceId)
	{
		RebirthPos rebirthPos = RebirthPosDic[instanceId];
		ItemCfg itemCfg = ItemCfg.Get(rebirthPos.typeId);
		int cdTime = 0;
		if (itemCfg.childType == 103)
		{
			cdTime = cfg.Consts.SLEEPING_BED_CD_TIME;
		}
		else if (itemCfg.childType == 102)
		{
			cdTime = cfg.Consts.BED_CD_TIME;
		}
		rebirthPos.cdTime = cdTime;
		_realTime[instanceId] = Time.realtimeSinceStartup;
	}

	private void SKillPlayerHandle(SKillPlayer msg)
	{
		if (msg.die.roleId == Singleton<RoleMgr>.Ins.info.roleId)
		{
			KillPlayer = msg;
			KillerRoleId = msg.killer.roleId;
		}
	}

	private void SLeaveBuildingHandle(SLeaveBuilding msg)
	{
	}

	private void SBuildPartHandle(SBuildPart msg)
	{
		if (msg.buildPartInfo.roleId != Singleton<RoleMgr>.Ins.info.roleId || msg.buildPartInfo.typeId < 1)
		{
			return;
		}
		ItemCfg itemCfg = ItemCfg.Get(msg.buildPartInfo.typeId);
		if (itemCfg != null && (itemCfg.childType == 102 || itemCfg.childType == 103))
		{
			RebirthPos rebirthPos = new RebirthPos();
			rebirthPos.typeId = msg.buildPartInfo.typeId;
			rebirthPos.cdTime = 0;
			rebirthPos.pos = msg.buildPartInfo.pos;
			RebirthPosDic[msg.buildPartInfo.instanceId] = rebirthPos;
			if (_rebirthFacilityList.Contains(msg.buildPartInfo.instanceId))
			{
				_rebirthFacilityList.Remove(msg.buildPartInfo.instanceId);
			}
			_rebirthFacilityList.Add(msg.buildPartInfo.instanceId);
			_rebirthFacilityList.Sort(Sort);
			_realTime[msg.buildPartInfo.instanceId] = Time.realtimeSinceStartup;
		}
	}

	private void SRebirthPosInfoHandle(SRebirthPosInfo msg)
	{
		RebirthPosDic = msg.info;
		foreach (KeyValuePair<long, RebirthPos> item in RebirthPosDic)
		{
			_rebirthFacilityList.Add(item.Key);
			_rebirthFacilityList.Sort(Sort);
			_realTime[item.Key] = Time.realtimeSinceStartup;
		}
	}

	private int Sort(long l1, long l2)
	{
		RebirthPos rebirthPos = RebirthPosDic[l1];
		RebirthPos rebirthPos2 = RebirthPosDic[l2];
		if (rebirthPos.typeId == rebirthPos2.typeId)
		{
			return (int)(l1 - l2);
		}
		return rebirthPos.typeId - rebirthPos2.typeId;
	}

	public float GetRealTime(long instanceId)
	{
		return _realTime[instanceId];
	}

	public void Rebirth(int type, long rebirthPosId)
	{
		_cRebirth.type = type;
		_cRebirth.rebirthPosId = rebirthPosId;
		Client2Gs.Ins.Send(_cRebirth);
	}
}
