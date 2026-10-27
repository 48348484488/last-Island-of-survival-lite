using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using cfg;
using gs.chat.scmsg;
using gs.role.scmsg;

public sealed class ChatScMgr : Singleton<ChatScMgr>
{
	private class ZhanDuiIdAndOverdueTime
	{
		public long ZhanDuiId;

		public float OverdueTime;
	}

	public const int MAX_CACHE_MSG_NUM = 100;

	private Coroutine _closeBigHornTick;

	private int _allNewMsgNum;

	private int _allNewPMsgNum;

	public Dictionary<int, NewChatInfoData<MsgBean>> tab2msgList = new Dictionary<int, NewChatInfoData<MsgBean>>();

	public Dictionary<long, NewChatInfoData<MsgBean>> id2PMsgList = new Dictionary<long, NewChatInfoData<MsgBean>>();

	public List<RoleVersion> IdList = new List<RoleVersion>();

	public MsgBean HornBean;

	public MsgBean LastBean;

	public Dictionary<int, int> Type2Tab = new Dictionary<int, int>();

	private Dictionary<long, ZhanDuiIdAndOverdueTime> _dicId2Zhandui = new Dictionary<long, ZhanDuiIdAndOverdueTime>();

	private const float _cacheContainTime = 1800f;

	public void Init()
	{
		Type2Tab.Clear();
		Type2Tab[1] = 1;
		Type2Tab[6] = 1;
		Type2Tab[5] = 1;
		Type2Tab[7] = 2;
		Type2Tab[3] = 3;
		Type2Tab[8] = 8;
		Type2Tab[4] = 4;
		Type2Tab[9] = 9;
		tab2msgList.Clear();
		tab2msgList[1] = new NewChatInfoData<MsgBean>(100);
		tab2msgList[2] = new NewChatInfoData<MsgBean>(100);
		tab2msgList[3] = new NewChatInfoData<MsgBean>(100);
		tab2msgList[8] = new NewChatInfoData<MsgBean>(100);
		tab2msgList[4] = new NewChatInfoData<MsgBean>(100);
		tab2msgList[9] = new NewChatInfoData<MsgBean>(100);
		SPrivateMsg.handler = (SPrivateMsg.Handler)Delegate.Combine(SPrivateMsg.handler, new SPrivateMsg.Handler(OnSPrivateMsg));
		SPublicMsg.handler = (SPublicMsg.Handler)Delegate.Combine(SPublicMsg.handler, new SPublicMsg.Handler(OnSPublicMsg));
		SBigHorn.handler = (SBigHorn.Handler)Delegate.Combine(SBigHorn.handler, new SBigHorn.Handler(_003CInit_003Em__0));
	}

	private IEnumerator onTickCloseBigHorn(int remainTime)
	{
		yield return new WaitForSeconds(remainTime);
		HornBean = null;
		Utils.TriggerEvent(ChatEventSc.CloseBigHorn);
	}

	private void OnSPublicMsg(SPublicMsg msg)
	{
		if (msg.info.text.Length > 25 && !Regex.IsMatch(msg.info.text, "\\[(\\-{0,1}\\d{0,})#(.+?)\\]"))
		{
			msg.info.text = msg.info.text.Insert(25, "\n");
		}
		if (Singleton<RoleMgr>.Ins.info != null)
		{
			addMsgBean(msg.info, msg.info.role.roleId != Singleton<RoleMgr>.Ins.info.roleId);
		}
		else
		{
			addMsgBean(msg.info, true);
		}
	}

	private void addMsgBean(MsgBean bean, bool isCount)
	{
		if (Type2Tab[bean.msgType] == 1 || !Singleton<FriendScMgr>.Ins.IsInBlackList(bean.role.roleId))
		{
			Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(bean.role.roleId, bean.role.name, 1);
			tab2msgList[Type2Tab[bean.msgType]].add(bean, isCount);
			if (bean.msgType == 5 || bean.msgType == 1 || bean.msgType == 8)
			{
				LastBean = bean;
			}
			_allNewMsgNum += (isCount ? 1 : 0);
		}
	}

	private void OnSPrivateMsg(SPrivateMsg msg)
	{
		if (msg.bean.text.Length > 15)
		{
			msg.bean.text = msg.bean.text.Insert(15, "\n");
		}
		addPrivateMsgBean(msg.bean, true);
	}

	private void addPrivateMsgBean(PrivateBean data, bool isCount)
	{
		if (!Singleton<FriendScMgr>.Ins.IsInBlackList(data.role.roleId))
		{
			Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(data.role.roleId, data.role.name, 1);
			MsgBean msgBean = new MsgBean();
			msgBean.role = data.role;
			msgBean.msgType = 100;
			msgBean.text = data.text;
			msgBean.voiceTime = data.voiceTime;
			if (id2PMsgList.ContainsKey(data.role.roleId))
			{
				id2PMsgList[data.role.roleId].add(msgBean, isCount);
				UpdateIdList(data.role.roleId, data.role.version);
			}
			else
			{
				id2PMsgList.Add(data.role.roleId, new NewChatInfoData<MsgBean>(100));
				id2PMsgList[data.role.roleId].add(msgBean, isCount);
				IdList.Add(msgBean.role);
			}
			_allNewMsgNum += (isCount ? 1 : 0);
			_allNewPMsgNum += (isCount ? 1 : 0);
		}
	}

	public void AddPrivateChat(long otherId, MsgBean bean)
	{
		if (bean.text.Length > 15)
		{
			bean.text = bean.text.Insert(15, "\n");
		}
		if (id2PMsgList.ContainsKey(otherId))
		{
			id2PMsgList[otherId].add(bean, false);
			UpdateIdList(otherId, bean.role.version);
			return;
		}
		id2PMsgList.Add(otherId, new NewChatInfoData<MsgBean>(100));
		id2PMsgList[otherId].add(bean, false);
		bean.role.roleId = otherId;
		IdList.Add(bean.role);
	}

	private void UpdateIdList(long roleId, int version)
	{
		foreach (RoleVersion id in IdList)
		{
			if (id.roleId == roleId)
			{
				id.version = version;
				break;
			}
		}
	}

	public int NewMsgNum(int tabType)
	{
		if (tabType == 100)
		{
			return (_allNewPMsgNum >= 99) ? 99 : _allNewPMsgNum;
		}
		return tab2msgList.ContainsKey(tabType) ? ((tab2msgList[tabType].newMsgNum >= 99) ? 99 : tab2msgList[tabType].newMsgNum) : 0;
	}

	public int NewWorldMsgNum(int tabType)
	{
		return tab2msgList.ContainsKey(tabType) ? ((tab2msgList[tabType].newMsgNum >= 99) ? 99 : tab2msgList[tabType].newMsgNum) : 0;
	}

	public void ClearNewMsgNum(int tabType)
	{
		if (tab2msgList.ContainsKey(tabType))
		{
			_allNewMsgNum -= tab2msgList[tabType].newMsgNum;
			tab2msgList[tabType].newMsgNum = 0;
		}
	}

	public int newPrivateMsgNum(long roleId)
	{
		return id2PMsgList.ContainsKey(roleId) ? id2PMsgList[roleId].newMsgNum : 0;
	}

	public void ClearNewPMsgNum(long roleId)
	{
		if (id2PMsgList.ContainsKey(roleId))
		{
			_allNewMsgNum -= id2PMsgList[roleId].newMsgNum;
			_allNewPMsgNum -= id2PMsgList[roleId].newMsgNum;
			id2PMsgList[roleId].newMsgNum = 0;
		}
	}

	public void AddNewPrivateChatItem(long otherId, int version)
	{
		if (!id2PMsgList.ContainsKey(otherId))
		{
			id2PMsgList.Add(otherId, new NewChatInfoData<MsgBean>(100));
			MsgBean msgBean = new MsgBean();
			msgBean.role.roleId = otherId;
			msgBean.role.version = version;
			IdList.Add(msgBean.role);
		}
	}

	public bool IsShowRedDot()
	{
		return _allNewMsgNum > 0;
	}

	public bool IsHideChatRedDot()
	{
		return _allNewPMsgNum <= 0;
	}

	public void AddZhanDuiCache(long roleId, long zhanDuiId)
	{
		ZhanDuiIdAndOverdueTime value;
		if (!_dicId2Zhandui.TryGetValue(roleId, out value))
		{
			value = new ZhanDuiIdAndOverdueTime();
		}
		value.ZhanDuiId = zhanDuiId;
		value.OverdueTime = Time.realtimeSinceStartup + 1800f;
		_dicId2Zhandui[roleId] = value;
	}

	public bool GetZhanDuiId(long roleId, out long zhanDuiId)
	{
		zhanDuiId = -1L;
		ZhanDuiIdAndOverdueTime value;
		if (_dicId2Zhandui.TryGetValue(roleId, out value) && value.OverdueTime > Time.realtimeSinceStartup)
		{
			zhanDuiId = value.ZhanDuiId;
			return true;
		}
		_dicId2Zhandui.Remove(roleId);
		return false;
	}

	[CompilerGenerated]
	private void _003CInit_003Em__0(SBigHorn msg)
	{
		HornBean = msg.info;
		Utils.StopConroutine(_closeBigHornTick);
		_closeBigHornTick = Utils.StartConroutine(onTickCloseBigHorn(cfg.Consts.BIG_HORN_SHOW_TIME_CLIENT));
	}
}
