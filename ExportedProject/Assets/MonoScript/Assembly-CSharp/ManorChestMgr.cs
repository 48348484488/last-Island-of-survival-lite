using System;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using SC.UI;
using Share;
using cfg;
using gs.battle.scmsg;

public class ManorChestMgr : Singleton<ManorChestMgr>
{
	private readonly Dictionary<long, HashSet<PartBehaviour>> _dicManorId2Part = new Dictionary<long, HashSet<PartBehaviour>>();

	private long _curOpenInstanceId;

	private CRequireManorChestInfo _cRequireManorChestInfo = new CRequireManorChestInfo();

	private CPutInManorChestItem _cPutInManorChestItem = new CPutInManorChestItem();

	private CGetManorChestItem _cGetManorChestItem = new CGetManorChestItem();

	private readonly CFixAllBuilding _cFix = new CFixAllBuilding();

	private readonly CUpgradeAllBuilding _cUpgrade = new CUpgradeAllBuilding();

	public void Init()
	{
		Singleton<StructureMenuMgr>.Ins.AddPermitFunc(104, Singleton<FriendPermitMgr>.Ins.HavePermit);
		BattleEvent.OnClickUseBuild = (Utils.IntLongDelegate)Delegate.Combine(BattleEvent.OnClickUseBuild, new Utils.IntLongDelegate(OnClickOpenManorChest));
		SManorChestInfo.handler = (SManorChestInfo.Handler)Delegate.Combine(SManorChestInfo.handler, new SManorChestInfo.Handler(OnSManorChestInfo));
		EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildFinish));
		SBuildingToolBoxIdChanged.handler = (SBuildingToolBoxIdChanged.Handler)Delegate.Combine(SBuildingToolBoxIdChanged.handler, new SBuildingToolBoxIdChanged.Handler(OnSBuildingToolBoxIdChanged));
		EventHandlers.OnDestroyedPart += OnBuildDestroy;
	}

	private void OnBuildDestroy(PartBehaviour part)
	{
		HashSet<PartBehaviour> value;
		if (_dicManorId2Part.TryGetValue(part.ToolBoxId, out value))
		{
			value.Remove(part);
		}
	}

	private void OnBuildFinish(long insId, BuildPart buildPartCfg, int status, Octets extraInfoOc)
	{
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(insId);
		if ((bool)partByInsID && partByInsID.ToolBoxId > 0)
		{
			HashSet<PartBehaviour> value;
			if (!_dicManorId2Part.TryGetValue(partByInsID.ToolBoxId, out value))
			{
				value = new HashSet<PartBehaviour>();
				_dicManorId2Part.Add(partByInsID.ToolBoxId, value);
			}
			value.Add(partByInsID);
		}
	}

	private void OnSBuildingToolBoxIdChanged(SBuildingToolBoxIdChanged msg)
	{
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(msg.buildingId);
		if (!partByInsID)
		{
			return;
		}
		HashSet<PartBehaviour> value;
		if (partByInsID.ToolBoxId > 0 && _dicManorId2Part.TryGetValue(partByInsID.ToolBoxId, out value))
		{
			value.Remove(partByInsID);
		}
		if (msg.toolBoxId > 0)
		{
			if (!_dicManorId2Part.TryGetValue(msg.toolBoxId, out value))
			{
				value = new HashSet<PartBehaviour>();
				_dicManorId2Part.Add(msg.toolBoxId, value);
			}
			value.Add(partByInsID);
		}
		if (ManorChestEvent.OnBuildingToolBoxIdChangeEvent != null)
		{
			ManorChestEvent.OnBuildingToolBoxIdChangeEvent(msg.buildingId, partByInsID.ToolBoxId, msg.toolBoxId);
		}
	}

	public bool GetToolBoxAllPart(long toolBoxId, out HashSet<PartBehaviour> parts)
	{
		return _dicManorId2Part.TryGetValue(toolBoxId, out parts);
	}

	private void OnSManorChestInfo(SManorChestInfo msg)
	{
		if (msg.instanceId == _curOpenInstanceId)
		{
			ViewMgr.Ins.ShowView<ToolboxPanel>(msg, false);
		}
	}

	private void OnClickOpenManorChest(int itemId, long instanceId)
	{
		if (ItemCfg.Get(itemId).childType == 104)
		{
			_curOpenInstanceId = instanceId;
			SendRequireManorChestInfoMsg();
		}
	}

	private void SendRequireManorChestInfoMsg()
	{
		_cRequireManorChestInfo.instanceId = _curOpenInstanceId;
		Client2Gs.Ins.Send(_cRequireManorChestInfo);
	}

	public void SendPutInManorChestItemMsg(int itemInstanceId, Dictionary<int, int> dicIndex2Num)
	{
		_cPutInManorChestItem.itemInstanceId = itemInstanceId;
		_cPutInManorChestItem.items = dicIndex2Num;
		_cPutInManorChestItem.chestInstanceId = _curOpenInstanceId;
		Client2Gs.Ins.Send(_cPutInManorChestItem);
	}

	public void SendGetManorChestItemMsg(long chestInstanceId, int index, int num, int itemId)
	{
		_cGetManorChestItem.chestInstanceId = chestInstanceId;
		_cGetManorChestItem.index = index;
		_cGetManorChestItem.number = num;
		_cGetManorChestItem.itemId = itemId;
		Client2Gs.Ins.Send(_cGetManorChestItem);
	}

	public void SendFixAllBuildingMsg(long manorChestInsId)
	{
		_cFix.toolBoxId = manorChestInsId;
		_cFix.level = -1;
		Client2Gs.Ins.Send(_cFix);
	}

	public void SendUpgradeAllBuildingMsg(long manorChestInsId, int level)
	{
		_cUpgrade.toolBoxId = manorChestInsId;
		_cUpgrade.level = level;
		Client2Gs.Ins.Send(_cUpgrade);
	}
}
