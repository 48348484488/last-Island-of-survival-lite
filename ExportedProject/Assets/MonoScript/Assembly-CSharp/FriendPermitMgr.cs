using System;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using Share;
using UnityEngine;
using cfg;
using gs.battle.scmsg;
using gs.friends.scmsg;

public class FriendPermitMgr : Singleton<FriendPermitMgr>
{
	private HashSet<long> _havePermitManorChests;

	private readonly Dictionary<long, OneChestPermit> _dicInstanceId2ChestInfo = new Dictionary<long, OneChestPermit>();

	public SPermitInfo PermitInfo;

	private CGivePermit _cGivePermit = new CGivePermit();

	public Dictionary<long, OneChestPermit> DicInstanceId2ChestInfo
	{
		get
		{
			return _dicInstanceId2ChestInfo;
		}
	}

	public void Init()
	{
		SRolePermissions.handler = (SRolePermissions.Handler)Delegate.Combine(SRolePermissions.handler, new SRolePermissions.Handler(OnSRolePermissions));
		SPermitInfo.handler = (SPermitInfo.Handler)Delegate.Combine(SPermitInfo.handler, new SPermitInfo.Handler(OnSPermitInfo));
		SGivePermit.handler = (SGivePermit.Handler)Delegate.Combine(SGivePermit.handler, new SGivePermit.Handler(OnSGivePermit));
		EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildFinish));
		SDestoryBuilding.handler = (SDestoryBuilding.Handler)Delegate.Combine(SDestoryBuilding.handler, new SDestoryBuilding.Handler(OnSDestoryBuilding));
		SChangeBuildingName.handler = (SChangeBuildingName.Handler)Delegate.Combine(SChangeBuildingName.handler, new SChangeBuildingName.Handler(OnSChangeBuildingName));
	}

	private void OnSChangeBuildingName(SChangeBuildingName msg)
	{
		OneChestPermit value;
		if (_dicInstanceId2ChestInfo.TryGetValue(msg.insId, out value))
		{
			value.name = msg.name;
		}
	}

	private void OnSDestoryBuilding(SDestoryBuilding msg)
	{
		long instanceId = msg.instanceId;
		if (_dicInstanceId2ChestInfo.ContainsKey(instanceId))
		{
			PermitInfo.ChestPermits.Remove(_dicInstanceId2ChestInfo[instanceId]);
			_dicInstanceId2ChestInfo.Remove(instanceId);
		}
	}

	private void OnBuildFinish(long InsId, BuildPart buildPartCfg, int status, Octets extraInfoOc)
	{
		ItemCfg itemCfg = ItemCfg.Get(buildPartCfg.id);
		if (itemCfg != null && itemCfg.childType == 104 && PermitInfo != null && SingletonMono<BuildManager>.Ins.GetPartByInsID(InsId).OwnerRoleId == Singleton<RoleMgr>.Ins.info.roleId)
		{
			OneChestPermit oneChestPermit = new OneChestPermit();
			PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(InsId);
			Vector3 position = partByInsID.transform.position;
			oneChestPermit.posX = position.x;
			oneChestPermit.posY = position.y;
			oneChestPermit.posZ = position.z;
			oneChestPermit.instanceId = InsId;
			PermitInfo.ChestPermits.Add(oneChestPermit);
			_dicInstanceId2ChestInfo[InsId] = oneChestPermit;
		}
	}

	private void OnSGivePermit(SGivePermit msg)
	{
		OneChestPermit value;
		if (_dicInstanceId2ChestInfo.TryGetValue(msg.instanceId, out value))
		{
			if (msg.addOrRemove)
			{
				value.roleIds.Add(msg.roleId);
			}
			else
			{
				value.roleIds.Remove(msg.roleId);
			}
		}
		else if (msg.addOrRemove)
		{
			_havePermitManorChests.Add(msg.instanceId);
		}
		else
		{
			_havePermitManorChests.Remove(msg.instanceId);
		}
	}

	private void OnSPermitInfo(SPermitInfo msg)
	{
		PermitInfo = msg;
		_dicInstanceId2ChestInfo.Clear();
		List<OneChestPermit> chestPermits = PermitInfo.ChestPermits;
		int i = 0;
		for (int count = chestPermits.Count; i < count; i++)
		{
			_dicInstanceId2ChestInfo[chestPermits[i].instanceId] = chestPermits[i];
		}
	}

	private void OnSRolePermissions(SRolePermissions msg)
	{
		_havePermitManorChests = msg.permissions;
	}

	public bool HavePermit(long chestInstanceId)
	{
		return (_havePermitManorChests != null && _havePermitManorChests.Contains(chestInstanceId)) || _dicInstanceId2ChestInfo.ContainsKey(chestInstanceId);
	}

	public bool HavePermitByInstanceId(long partInstanceId)
	{
		long toolBoxId = SingletonMono<BuildManager>.Ins.GetPartByInsID(partInstanceId).ToolBoxId;
		return toolBoxId == 0 || HavePermit(toolBoxId);
	}

	public void SendGivePermitMsg(long instanceId, long roleId, bool isAdd)
	{
		_cGivePermit.instanceId = instanceId;
		_cGivePermit.roleId = roleId;
		_cGivePermit.addOrRemove = isAdd;
		Client2Gs.Ins.Send(_cGivePermit);
	}

	public bool IsMine(long instanceId)
	{
		return _dicInstanceId2ChestInfo.ContainsKey(instanceId);
	}
}
