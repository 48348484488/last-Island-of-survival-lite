using System;
using System.Collections.Generic;
using System.IO;
using SC.UI;
using UnityEngine;
using cfg;
using gs.friends.scmsg;
using gs.intimacy.scmsg;
using gs.online.scmsg;
using gs.role.scmsg;

public class FriendScMgr : Singleton<FriendScMgr>
{
	public readonly Dictionary<long, CareRoleInfo> _careDict = new Dictionary<long, CareRoleInfo>();

	public List<CareRoleInfo> CareList = new List<CareRoleInfo>();

	public readonly Dictionary<long, FansRoleInfo> _fansDict = new Dictionary<long, FansRoleInfo>();

	public List<FansRoleInfo> FansList = new List<FansRoleInfo>();

	public readonly Dictionary<long, BlackRoleInfo> _blackDict = new Dictionary<long, BlackRoleInfo>();

	public List<BlackRoleInfo> BlackList = new List<BlackRoleInfo>();

	public int NextNearBy;

	public bool HasMoreNearBy = true;

	public SFindManitoPlayer GSRecommend;

	public readonly string RecCareIdentification = "care";

	public readonly string RecFansIdentification = "fans";

	public readonly char RecSeparator = '|';

	public byte NewRecData;

	private string recFilePath;

	private readonly bool isEncryption;

	public bool IsRequested;

	private float _nextTimeToRequest;

	private readonly List<RoleInfo> _searchResultInfo = new List<RoleInfo>();

	private HashSet<long> _newFansSet = new HashSet<long>();

	private HashSet<long> _careSet;

	private HashSet<long> _fansSet;

	private HashSet<long> _blackSet;

	private CFriendRoleIds _cFriendRoleIds = new CFriendRoleIds();

	private bool _isRequestIds;

	private CFriendInfo _cFriendInfo = new CFriendInfo();

	private GetBatchRecommendFriends _cGetBatchRecommendFriends = new GetBatchRecommendFriends();

	public float RefreshRecommendCountDown;

	private RefreshRecommendFriends _cRefreshRecommendFriends = new RefreshRecommendFriends();

	private long _curAddBlackId;

	private readonly CCare _cCare = new CCare();

	private readonly CCareCancel _cCareCancel = new CCareCancel();

	public List<RoleInfo> GetSearchResult
	{
		get
		{
			return _searchResultInfo;
		}
	}

	public int NewFansCount
	{
		get
		{
			if (_newFansSet.Count > 99)
			{
				return 99;
			}
			return _newFansSet.Count;
		}
	}

	public void Init()
	{
		SFriendInfo.handler = (SFriendInfo.Handler)Delegate.Combine(SFriendInfo.handler, new SFriendInfo.Handler(OnSFrindInfo));
		SCare.handler = (SCare.Handler)Delegate.Combine(SCare.handler, new SCare.Handler(OnSCare));
		SCareCancel.handler = (SCareCancel.Handler)Delegate.Combine(SCareCancel.handler, new SCareCancel.Handler(OnSCareCancel));
		SFansAdd.handler = (SFansAdd.Handler)Delegate.Combine(SFansAdd.handler, new SFansAdd.Handler(OnSFansAdd));
		SFansCut.handler = (SFansCut.Handler)Delegate.Combine(SFansCut.handler, new SFansCut.Handler(OnSFansCut));
		SBlack.handler = (SBlack.Handler)Delegate.Combine(SBlack.handler, new SBlack.Handler(OnSBlack));
		SBlackDelete.handler = (SBlackDelete.Handler)Delegate.Combine(SBlackDelete.handler, new SBlackDelete.Handler(OnSBlackDelete));
		SFriendError.handler = (SFriendError.Handler)Delegate.Combine(SFriendError.handler, new SFriendError.Handler(OnSFriendError));
		SSearchByRoleId.handler = (SSearchByRoleId.Handler)Delegate.Combine(SSearchByRoleId.handler, new SSearchByRoleId.Handler(OnSSearchByRoleId));
		SNewFans.handler = (SNewFans.Handler)Delegate.Combine(SNewFans.handler, new SNewFans.Handler(OnSNewFans));
		SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Combine(SLoginFinished.handler, new SLoginFinished.Handler(OnSLoginFinished));
		SFindManitoPlayer.handler = (SFindManitoPlayer.Handler)Delegate.Combine(SFindManitoPlayer.handler, new SFindManitoPlayer.Handler(OnSFindManitoPlayer));
		SFriendRoleIds.handler = (SFriendRoleIds.Handler)Delegate.Combine(SFriendRoleIds.handler, new SFriendRoleIds.Handler(OnSFriendRoleIds));
		SFriendIntimacyChange.handler = (SFriendIntimacyChange.Handler)Delegate.Combine(SFriendIntimacyChange.handler, new SFriendIntimacyChange.Handler(OnSFriendIntimacyChange));
	}

	private void OnSFriendIntimacyChange(SFriendIntimacyChange msg)
	{
		CareRoleInfo value;
		if (_careDict.TryGetValue(msg.targetRoleId, out value))
		{
			value.intimacyLevel = msg.intimacyLevel;
			value.intimacyValue = msg.intimacyValue;
		}
	}

	public bool GetFavorabilityLevelAndValue(long otherId, out int lv, out int value)
	{
		lv = 0;
		value = 0;
		if (!IsCareEachother(otherId))
		{
			return false;
		}
		CareRoleInfo value2;
		if (_careDict.TryGetValue(otherId, out value2))
		{
			lv = value2.intimacyLevel;
			value = value2.intimacyValue;
			return true;
		}
		return false;
	}

	public void SendRequestIdsMsg()
	{
		if (!_isRequestIds)
		{
			Client2Gs.Ins.Send(_cFriendRoleIds);
		}
	}

	private void OnSFriendRoleIds(SFriendRoleIds msg)
	{
		_careSet = msg.cares;
		_fansSet = msg.fans;
		_blackSet = msg.blacks;
		_isRequestIds = true;
	}

	public void SendRequestMsg()
	{
		if (!IsRequested && !(Time.realtimeSinceStartup < _nextTimeToRequest))
		{
			_nextTimeToRequest = Time.realtimeSinceStartup + 5f;
			Client2Gs.Ins.Send(_cFriendInfo);
		}
	}

	public void SendGetBatchRecommendFriends()
	{
		Client2Gs.Ins.Send(_cGetBatchRecommendFriends);
	}

	public void SendRefreshRecommendFriends()
	{
		Client2Gs.Ins.Send(_cRefreshRecommendFriends);
	}

	private void OnSFindManitoPlayer(SFindManitoPlayer msg)
	{
		GSRecommend = msg;
		RefreshRecommendCountDown = Time.realtimeSinceStartup + (float)msg.lostCdTime;
		List<RoleInfo> players = msg.players;
		int i = 0;
		for (int num = players.Count; i < num; i++)
		{
			if (players[i].info.roleId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				players.RemoveAt(i);
				i--;
				num--;
			}
			else
			{
				Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(players[i].info, players[i].onlineStatus);
			}
		}
	}

	private void OnSLoginFinished(SLoginFinished msg)
	{
		recFilePath = Utils.GetPersistentPath(Singleton<RoleMgr>.Ins.info.roleId + "_FriendRec.txt");
		Singleton<FriendRecDataSc>.Ins.CleanAllNews();
		Singleton<FriendRecDataReadAndWriteSc>.Ins.LoadTextFile(recFilePath, isEncryption);
		SendRequestIdsMsg();
	}

	private void OnSFrindInfo(SFriendInfo info)
	{
		IsRequested = true;
		Clear();
		CareList = info.cares;
		FansList = info.fans;
		BlackList = info.blacks;
		SortCareList();
		SortFansList();
		foreach (CareRoleInfo care in CareList)
		{
			BasicRoleInfo info2 = care.roleInfo.info;
			_careDict.Add(info2.roleId, care);
			Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(care.roleInfo.info, care.roleInfo.onlineStatus);
		}
		foreach (FansRoleInfo fans in FansList)
		{
			BasicRoleInfo info2 = fans.roleInfo.info;
			_fansDict.Add(info2.roleId, fans);
			Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(fans.roleInfo.info, fans.roleInfo.onlineStatus);
		}
		foreach (BlackRoleInfo black in BlackList)
		{
			BasicRoleInfo info2 = black.roleInfo.info;
			_blackDict.Add(info2.roleId, black);
			Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(black.roleInfo.info, black.roleInfo.onlineStatus);
		}
		Utils.TriggerEvent(FriendEventSc.UpdateFriend);
	}

	private void OnSCare(SCare info)
	{
		if (!_careDict.ContainsKey(info.info.roleInfo.info.roleId))
		{
			AlertBox.Show(160);
			_careDict.Add(info.info.roleInfo.info.roleId, info.info);
			CareList.Add(info.info);
			_careSet.Add(info.info.roleInfo.info.roleId);
			Singleton<FriendRecDataReadAndWriteSc>.Ins.CreateTextFile(recFilePath, RecCareIdentification + RecSeparator + info.info.careTime + RecSeparator + info.info.roleInfo.info.name, isEncryption, !File.Exists(recFilePath));
			if (NewRecData < cfg.Consts.MAX_FRIEND_REC_NUM)
			{
				NewRecData++;
			}
			Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(info.info.roleInfo.info, info.info.roleInfo.onlineStatus);
			Utils.TriggerEvent(FriendEventSc.UpdateFriendRedDot);
		}
	}

	private void OnSCareCancel(SCareCancel info)
	{
		_careSet.Remove(info.roleId);
		if (_careDict.ContainsKey(info.roleId))
		{
			if (_curAddBlackId == info.roleId)
			{
				_curAddBlackId = -1L;
			}
			else
			{
				AlertBox.Show(161);
			}
			CareList.Remove(_careDict[info.roleId]);
			_careDict.Remove(info.roleId);
		}
	}

	private void OnSFansAdd(SFansAdd info)
	{
		if (!_fansDict.ContainsKey(info.info.roleInfo.info.roleId))
		{
			FansList.Add(info.info);
			_fansSet.Add(info.info.roleInfo.info.roleId);
			_fansDict.Add(info.info.roleInfo.info.roleId, info.info);
			_newFansSet.Add(info.info.roleInfo.info.roleId);
			Singleton<FriendRecDataReadAndWriteSc>.Ins.CreateTextFile(recFilePath, RecFansIdentification + RecSeparator + info.info.fansTime + RecSeparator + info.info.roleInfo.info.name, isEncryption, !File.Exists(recFilePath));
			if (NewRecData < cfg.Consts.MAX_FRIEND_REC_NUM)
			{
				NewRecData++;
			}
			Utils.TriggerEvent(FriendEventSc.UpdateFriendRedDot);
		}
		Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(info.info.roleInfo.info, info.info.roleInfo.onlineStatus);
	}

	private void OnSFansCut(SFansCut info)
	{
		_fansSet.Remove(info.roleId);
		if (_fansDict.ContainsKey(info.roleId))
		{
			FansList.Remove(_fansDict[info.roleId]);
			_fansDict.Remove(info.roleId);
			if (_newFansSet.Contains(info.roleId))
			{
				_newFansSet.Remove(info.roleId);
				Utils.TriggerEvent(FriendEventSc.OnNewFansNumChangeEvent);
			}
			Utils.TriggerEvent(FriendEventSc.UpdateFriendRedDot);
		}
	}

	private void OnSBlack(SBlack info)
	{
		if (!_blackDict.ContainsKey(info.info.roleInfo.info.roleId))
		{
			AlertBox.Show(159);
			_curAddBlackId = info.info.roleInfo.info.roleId;
			BlackList.Add(info.info);
			_blackSet.Add(info.info.roleInfo.info.roleId);
			_blackDict.Add(info.info.roleInfo.info.roleId, info.info);
			Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(info.info.roleInfo.info, info.info.roleInfo.onlineStatus);
		}
	}

	private void OnSBlackDelete(SBlackDelete info)
	{
		_blackSet.Remove(info.roleId);
		if (_blackDict.ContainsKey(info.roleId))
		{
			AlertBox.Show(162);
			BlackList.Remove(_blackDict[info.roleId]);
			_blackDict.Remove(info.roleId);
		}
	}

	private void OnSNewFans(SNewFans info)
	{
		_newFansSet = info.newRoleIds;
		Utils.TriggerEvent(FriendEventSc.UpdateFriendRedDot);
		Utils.TriggerEvent(FriendEventSc.OnNewFansNumChangeEvent);
	}

	private void OnSFriendError(SFriendError info)
	{
		MessageBoxPanel.ShowConfirm((info.code != 1) ? Utils.GetString(14) : Utils.GetString(13));
	}

	private void OnSSearchByRoleId(SSearchByRoleId msg)
	{
		_searchResultInfo.Clear();
		_searchResultInfo.Add(msg.roleInfo);
	}

	public void Clear()
	{
		_careDict.Clear();
		_fansDict.Clear();
		_blackDict.Clear();
		_newFansSet.Clear();
		if (CareList != null)
		{
			CareList.Clear();
			BlackList.Clear();
			FansList.Clear();
		}
	}

	public bool IsInCareList(long roleid)
	{
		return _careDict.ContainsKey(roleid) || (_careSet != null && _careSet.Contains(roleid));
	}

	public bool IsInBlackList(long roleid)
	{
		return _blackDict.ContainsKey(roleid) || (_blackSet != null && _blackSet.Contains(roleid));
	}

	public bool IsInFansList(long roleid)
	{
		return _fansDict.ContainsKey(roleid) || (_fansSet != null && _fansSet.Contains(roleid));
	}

	public void SortCareList()
	{
		CareList.Sort(SortCareList);
	}

	private int SortCareList(CareRoleInfo info1, CareRoleInfo info2)
	{
		BasicRoleInfo info3 = info1.roleInfo.info;
		BasicRoleInfo info4 = info2.roleInfo.info;
		if (info1.roleInfo.onlineStatus > 0 == info2.roleInfo.onlineStatus > 0)
		{
			if (IsInFansList(info3.roleId) == IsInFansList(info4.roleId))
			{
				if (info4.level == info3.level)
				{
					return (int)(info3.roleId - info4.roleId);
				}
				return info4.level - info3.level;
			}
			return (!IsInFansList(info3.roleId)) ? 1 : (-1);
		}
		return (info1.roleInfo.onlineStatus <= 0) ? 1 : (-1);
	}

	public void SortFansList()
	{
		FansList.Sort(SortFansList);
	}

	private int SortFansList(FansRoleInfo info1, FansRoleInfo info2)
	{
		RoleInfo roleInfo = info1.roleInfo;
		RoleInfo roleInfo2 = info2.roleInfo;
		if (roleInfo.onlineStatus > 0 == roleInfo2.onlineStatus > 0)
		{
			if (IsInFansList(roleInfo.info.roleId) == IsInFansList(roleInfo2.info.roleId))
			{
				return roleInfo2.info.level - roleInfo.info.level;
			}
			return (!IsInFansList(roleInfo.info.roleId)) ? 1 : (-1);
		}
		return (roleInfo.onlineStatus <= 0) ? 1 : (-1);
	}

	public void ClearNewFans()
	{
		if (_newFansSet != null && _newFansSet.Count > 0)
		{
			CClearNewFans msg = new CClearNewFans();
			Client2Gs.Ins.Send(msg);
			_newFansSet.Clear();
			Utils.TriggerEvent(FriendEventSc.UpdateFriendRedDot);
			Utils.TriggerEvent(FriendEventSc.OnNewFansNumChangeEvent);
		}
	}

	public bool IsCareEachother(long roleId)
	{
		return (_careDict.ContainsKey(roleId) && _fansDict.ContainsKey(roleId)) || (_careSet != null && _careSet.Contains(roleId) && _fansSet != null && _fansSet.Contains(roleId));
	}

	private void OnUserLogoff(long id)
	{
		Clear();
	}

	public void AddCare(long roleId)
	{
		if (IsInBlackList(roleId))
		{
			MessageBoxPanel.ShowConfirm(Utils.GetString(13));
		}
		else if (_careSet != null && _careSet.Count >= cfg.Consts.FRIEND_MAX_NUM)
		{
			AlertBox.Show(Utils.GetString(158, cfg.Consts.FRIEND_MAX_NUM));
		}
		else
		{
			_cCare.roleId = roleId;
			Client2Gs.Ins.Send(_cCare);
		}
	}

	public void RemoveCare(long roleId)
	{
		if (IsInCareList(roleId))
		{
			_cCareCancel.roleId = roleId;
			Client2Gs.Ins.Send(_cCareCancel);
		}
	}

	public void AddBlack(long roleid)
	{
		if (IsInBlackList(roleid))
		{
			AlertBox.Show(159);
			return;
		}
		CBlack cBlack = new CBlack();
		cBlack.roleId = roleid;
		Client2Gs.Ins.Send(cBlack);
	}

	public void RemoveBlack(long roleId)
	{
		CBlackDelete cBlackDelete = new CBlackDelete();
		cBlackDelete.roleId = roleId;
		Client2Gs.Ins.Send(cBlackDelete);
	}

	public bool IsShowRedDot()
	{
		return _newFansSet.Count > 0 || NewRecData > 0;
	}
}
