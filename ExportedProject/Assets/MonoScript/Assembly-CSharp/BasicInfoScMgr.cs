using System;
using System.Collections.Generic;
using UnityEngine;
using gs.friends.scmsg;
using gs.role.scmsg;

public class BasicInfoScMgr : Singleton<BasicInfoScMgr>
{
	private LRU<long, AllBasicInfo> AllRoleInfos;

	private CBasicInfo msg;

	private HashSet<long> askedId;

	public Utils.LongDelegate UpdateBasicInfo;

	public AllBasicInfo GetOnePlayerInfo()
	{
		List<AllBasicInfo> values = AllRoleInfos.GetValues();
		return values[UnityEngine.Random.Range(0, values.Count)];
	}

	public void Init()
	{
		AllRoleInfos = new LRU<long, AllBasicInfo>(500);
		AllRoleInfos.onRemoveEntry = OnRemoveEntry;
		msg = new CBasicInfo();
		askedId = new HashSet<long>();
		SBasicInfo.handler = (SBasicInfo.Handler)Delegate.Combine(SBasicInfo.handler, new SBasicInfo.Handler(OnSBasicInfo));
		SSyncOnline.handler = (SSyncOnline.Handler)Delegate.Combine(SSyncOnline.handler, new SSyncOnline.Handler(OnSSyncOnline));
	}

	private bool OnRemoveEntry(long roleId, AllBasicInfo info)
	{
		if (Singleton<TeamScMgr>.Ins.IsInMyTeam(roleId) || roleId == Singleton<RoleMgr>.Ins.info.roleId)
		{
			return false;
		}
		return true;
	}

	private void OnSSyncOnline(SSyncOnline sMsg)
	{
		if (sMsg.status > 0)
		{
			SingletonMono<AudioManager>.Ins.Play2D(349);
		}
		StatusChange(sMsg.roleId, sMsg.status);
	}

	public void StatusChange(long roleId, byte status)
	{
		try
		{
			CareRoleInfo value;
			if (Singleton<FriendScMgr>.Ins._careDict.TryGetValue(roleId, out value))
			{
				value.roleInfo.onlineStatus = status;
			}
			FansRoleInfo value2;
			if (Singleton<FriendScMgr>.Ins._fansDict.TryGetValue(roleId, out value2))
			{
				value2.roleInfo.onlineStatus = status;
			}
			BlackRoleInfo value3;
			if (Singleton<FriendScMgr>.Ins._blackDict.TryGetValue(roleId, out value3))
			{
				value3.roleInfo.onlineStatus = status;
			}
		}
		catch (Exception)
		{
		}
		AllBasicInfo value4;
		if (!AllRoleInfos.TryGetValue(roleId, out value4))
		{
			value4 = GetEmptyBasicInfo(roleId, string.Empty, status);
			AllRoleInfos.Set(roleId, value4);
		}
		value4.status = status;
		askedId.Remove(roleId);
		if (UpdateBasicInfo != null)
		{
			UpdateBasicInfo(roleId);
		}
	}

	private void OnSBasicInfo(SBasicInfo sMsg)
	{
		AllBasicInfo value;
		if (!AllRoleInfos.TryGetValue(sMsg.info.roleId, out value))
		{
			value = GetEmptyBasicInfo(sMsg.info.roleId, sMsg.info.name, 0);
			AllRoleInfos.Set(sMsg.info.roleId, value);
		}
		if (sMsg.info.version > value.basicRoleInfo.version)
		{
			value.basicRoleInfo = sMsg.info;
			if (value.basicRoleInfo.frameId <= 0)
			{
				value.basicRoleInfo.frameId = 1;
			}
			askedId.Remove(value.basicRoleInfo.roleId);
			if (UpdateBasicInfo != null)
			{
				UpdateBasicInfo(sMsg.info.roleId);
			}
		}
	}

	public void AddBasicInfo(BasicRoleInfo info, byte status = 0)
	{
		if (info.frameId <= 0)
		{
			info.frameId = 1;
		}
		AllBasicInfo value;
		if (AllRoleInfos.TryGetValue(info.roleId, out value))
		{
			if (value.basicRoleInfo.version < info.version)
			{
				value.basicRoleInfo = info;
			}
		}
		else
		{
			value = GetEmptyBasicInfo(info.roleId, info.name, status);
			value.basicRoleInfo = info;
			AllRoleInfos.Set(info.roleId, value);
		}
	}

	public void AddBasicInfo(long id, string name, byte status, int version = 0)
	{
		AllBasicInfo value;
		if (AllRoleInfos.TryGetValue(id, out value))
		{
			value.status = status;
			return;
		}
		value = GetEmptyBasicInfo(id, name, status);
		AllRoleInfos.Set(id, value);
	}

	private AllBasicInfo GetEmptyBasicInfo(long roleId, string name, byte status)
	{
		AllBasicInfo allBasicInfo = new AllBasicInfo();
		allBasicInfo.basicRoleInfo = new BasicRoleInfo();
		allBasicInfo.basicRoleInfo.roleId = roleId;
		allBasicInfo.basicRoleInfo.frameId = 1;
		allBasicInfo.basicRoleInfo.name = name ?? roleId.ToString();
		allBasicInfo.status = status;
		return allBasicInfo;
	}

	public AllBasicInfo GetProperty(long _roleId, int version, string name = null)
	{
		AllBasicInfo value;
		if (!AllRoleInfos.TryGetValue(_roleId, out value))
		{
			AllRoleInfos.Set(_roleId, GetEmptyBasicInfo(_roleId, name, 0));
			AllRoleInfos.TryGetValue(_roleId, out value);
		}
		if (value.basicRoleInfo == null)
		{
			value.basicRoleInfo = new BasicRoleInfo
			{
				roleId = _roleId
			};
		}
		if (value.basicRoleInfo.headId == 0)
		{
			value.basicRoleInfo.headId = 1;
		}
		try
		{
			CareRoleInfo value2;
			if (Singleton<FriendScMgr>.Ins._careDict.TryGetValue(_roleId, out value2))
			{
				value.status = value2.roleInfo.onlineStatus;
			}
			FansRoleInfo value3;
			if (Singleton<FriendScMgr>.Ins._fansDict.TryGetValue(_roleId, out value3))
			{
				value.status = value3.roleInfo.onlineStatus;
			}
			BlackRoleInfo value4;
			if (Singleton<FriendScMgr>.Ins._blackDict.TryGetValue(_roleId, out value4))
			{
				value.status = value4.roleInfo.onlineStatus;
			}
		}
		catch (Exception)
		{
		}
		if (askedId.Contains(_roleId) || version <= value.basicRoleInfo.version)
		{
			return value;
		}
		askedId.Add(_roleId);
		msg.otherId = _roleId;
		Client2Gs.Ins.Send(msg);
		return value;
	}

	public byte GetStatus(long roleId)
	{
		AllBasicInfo value;
		if (AllRoleInfos.TryGetValue(roleId, out value))
		{
			return value.status;
		}
		try
		{
			CareRoleInfo value2;
			if (Singleton<FriendScMgr>.Ins._careDict.TryGetValue(roleId, out value2))
			{
				return value2.roleInfo.onlineStatus;
			}
			FansRoleInfo value3;
			if (Singleton<FriendScMgr>.Ins._fansDict.TryGetValue(roleId, out value3))
			{
				return value3.roleInfo.onlineStatus;
			}
			BlackRoleInfo value4;
			if (Singleton<FriendScMgr>.Ins._blackDict.TryGetValue(roleId, out value4))
			{
				return value4.roleInfo.onlineStatus;
			}
		}
		catch (Exception)
		{
		}
		if (askedId.Contains(roleId))
		{
			return 1;
		}
		askedId.Add(roleId);
		msg.otherId = roleId;
		Client2Gs.Ins.Send(msg);
		return 1;
	}
}
