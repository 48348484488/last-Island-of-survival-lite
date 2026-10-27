using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.friends.scmsg;
using gs.intimacy.scmsg;
using gs.role.scmsg;

namespace SC.UI
{
	public class FriendPanel : View
	{
		public enum Tab2List
		{
			Attention = 0,
			Fans = 1,
			Blacklist = 2,
			Nearby = 3,
			Skilled = 4,
			Recommend = 5,
			Invite = 6,
			Search = 7,
			TeaStu_classmate = 8,
			TeaStu_student = 9
		}

		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey0
		{
			internal InputField inp;

			internal FriendPanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this.Hide();
			}

			internal void _003C_003Em__1(GameObject go)
			{
				if (!_0024this.m_ralationship.activeSelf)
				{
					_0024this.m_ralationship.SetActiveBetter(true);
					_0024this.m_find_friend.SetActiveBetter(false);
					_0024this.eCurTab = Tab2List.Attention;
					_0024this.UpdateTabStatus();
					_0024this.OnAttentionNumChange();
					_0024this.OnFansNumChange();
					_0024this.UpdateRelationshipScroll(_0024this.eCurTab);
					_0024this.UpdateRecRedDot();
				}
			}

			internal void _003C_003Em__2(GameObject go)
			{
				_0024this.eCurTab = Tab2List.Attention;
				_0024this.m_attention.SetActiveBetter(true);
				_0024this.m_fans.SetActiveBetter(false);
				_0024this.UpdateRelationshipScroll(_0024this.eCurTab);
				_0024this.OnAttentionNumChange();
			}

			internal void _003C_003Em__3(GameObject go)
			{
				_0024this.eCurTab = Tab2List.Fans;
				_0024this.m_attention.SetActiveBetter(false);
				_0024this.m_fans.SetActiveBetter(true);
				_0024this.UpdateRelationshipScroll(_0024this.eCurTab);
				Singleton<FriendScMgr>.Ins.ClearNewFans();
				_0024this.OnFansNumChange();
				_0024this.OnNewFansNumChange();
			}

			internal void _003C_003Em__4(GameObject go)
			{
				_0024this.eCurTab = Tab2List.Blacklist;
				_0024this.m_attention.SetActiveBetter(false);
				_0024this.m_fans.SetActiveBetter(false);
				_0024this.UpdateRelationshipScroll(_0024this.eCurTab);
			}

			internal void _003C_003Em__5(GameObject go)
			{
				_0024this.OnShowFindFriend();
			}

			internal void _003C_003Em__6(GameObject go)
			{
				_0024this.eCurTab = Tab2List.Recommend;
				_0024this.UpdateRelationshipScroll(_0024this.eCurTab);
			}

			internal void _003C_003Em__7(GameObject go)
			{
				string text = inp.text.Trim();
				if (string.IsNullOrEmpty(text))
				{
					AlertBox.Show(Utils.GetString(45));
					return;
				}
				long roleIdByKey = Singleton<RoleMgr>.Ins.getRoleIdByKey(text.ToUpper());
				_0024this._cSearchByRoleId.name = ((roleIdByKey <= 0) ? text : roleIdByKey.ToString());
				Client2Gs.Ins.Send(_0024this._cSearchByRoleId);
				inp.text = string.Empty;
			}

			internal void _003C_003Em__8(GameObject go)
			{
				_0024this.UpdateRecScroll();
				Singleton<FriendScMgr>.Ins.NewRecData = 0;
				_0024this.m_recording.SetActiveBetter(true);
				_0024this.m_red_dot_rec.SetActiveBetter(false);
				if (FriendEventSc.UpdateFriendRedDot != null)
				{
					FriendEventSc.UpdateFriendRedDot();
				}
			}

			internal void _003C_003Em__9(GameObject go)
			{
				_0024this.m_recording.SetActiveBetter(false);
			}

			internal void _003C_003Em__A(GameObject go)
			{
				_0024this.m_recording.SetActiveBetter(false);
			}

			internal void _003C_003Em__B(GameObject go)
			{
				if (_0024this._countDown > Time.realtimeSinceStartup)
				{
					AlertBox.Show(415);
				}
				else
				{
					Singleton<FriendScMgr>.Ins.SendRefreshRecommendFriends();
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillCommonThings_003Ec__AnonStorey1
		{
			internal AllBasicInfo info;

			internal int status;

			internal void _003C_003Em__0(GameObject go)
			{
				Singleton<TeamScMgr>.Ins.JoinOrInviteToTeam(info.basicRoleInfo.roleId, status);
			}

			internal void _003C_003Em__1(GameObject o)
			{
				Singleton<RoleMgr>.Ins.GetRoleMoreInformation(info.basicRoleInfo.roleId);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillSearchCell_003Ec__AnonStorey2
		{
			internal RoleInfo info;

			internal void _003C_003Em__0(GameObject o)
			{
				Singleton<RoleMgr>.Ins.GetRoleMoreInformation(info.info.roleId);
			}

			internal void _003C_003Em__1(GameObject o)
			{
				if (Singleton<FriendScMgr>.Ins.IsInCareList(info.info.roleId))
				{
					AlertBox.Show(Utils.GetString(72));
				}
				else
				{
					Singleton<FriendScMgr>.Ins.AddCare(info.info.roleId);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillRecommendCell_003Ec__AnonStorey3
		{
			internal RoleInfo info;

			internal void _003C_003Em__0(GameObject o)
			{
				Singleton<RoleMgr>.Ins.GetRoleMoreInformation(info.info.roleId);
			}

			internal void _003C_003Em__1(GameObject o)
			{
				if (Singleton<FriendScMgr>.Ins.IsInCareList(info.info.roleId))
				{
					AlertBox.Show(Utils.GetString(72));
				}
				else
				{
					Singleton<FriendScMgr>.Ins.AddCare(info.info.roleId);
				}
			}
		}

		public const int None = 0;

		public const int ResetPanel = 1;

		private Tab2List eCurTab;

		private Tab2List eTeaStuTab;

		private UIScrollPanel relationshipScroll;

		private UIScrollPanel findFriendScroll;

		private UIScrollPanel recScroll;

		private UIScrollPanel teastuMemberScroll;

		private List<CareRoleInfo> careList;

		private List<FansRoleInfo> fansList;

		private List<BlackRoleInfo> blackList;

		private List<RoleInfo> searchResultList;

		private Dictionary<int, long> index2roleId = new Dictionary<int, long>();

		private CSearchByRoleId _cSearchByRoleId = new CSearchByRoleId();

		private bool _shouldSortCare = true;

		private bool _shouldSortFans = true;

		private float _countDown;

		private const float IntervalTime = 1f;

		private float _passTime;

		private GameObject btn_relationship;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject m_red_dot_gx;

		private GameObject btn_find_friend;

		private GameObject m_ralationship;

		private GameObject btn_attention;

		private GameObject btn_fans;

		private GameObject m_red_dot_fans;

		private GameObject txt_new_fans_num;

		private Text txt_new_fans_numText;

		private GameObject btn_black;

		private GameObject btn_recording;

		private GameObject m_red_dot_rec;

		private GameObject txt_new_rec;

		private Text txt_new_recText;

		private GameObject scp_relationship;

		private GRalationshipCell m_cell;

		private GameObject m_attention;

		private GameObject txt_attention_num;

		private Text txt_attention_numText;

		private GameObject m_fans;

		private GameObject txt_fans_num;

		private Text txt_fans_numText;

		private GameObject m_find_friend;

		private GameObject btn_recommend;

		private GameObject inp_search;

		private GameObject btn_search;

		private GameObject scp_find_friend;

		private GameObject btn_back;

		private GameObject m_recording;

		private GameObject m_rec_bg;

		private GameObject btn_close_rec;

		private GameObject scp_recording;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GameObject btn_ling_di_gui;

		private GameObject m_search;

		private GameObject m_care_all;

		private GameObject btn_care_all;

		private GameObject m_change;

		private GameObject btn_change;

		private GameObject txt_change;

		private Text txt_changeText;

		private GameObject txt_count_down;

		private Text txt_count_downText;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		private GameObject txt_on_1;

		private Text txt_on_1Text;

		private GameObject txt_off_1;

		private Text txt_off_1Text;

		private GameObject txt_on_2;

		private Text txt_on_2Text;

		private GameObject txt_off_2;

		private Text txt_off_2Text;

		private GameObject txt_on_3;

		private Text txt_on_3Text;

		private GameObject txt_off_3;

		private Text txt_off_3Text;

		private GFindFriendCell m_cell_0;

		private GameObject txt_on_4;

		private Text txt_on_4Text;

		private GFriendRecordCell m_cell_1;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache1;

		protected override void onInit()
		{
			_003ConInit_003Ec__AnonStorey0 _003ConInit_003Ec__AnonStorey = new _003ConInit_003Ec__AnonStorey0();
			_003ConInit_003Ec__AnonStorey._0024this = this;
			base.onInit();
			if (!Singleton<FriendScMgr>.Ins.IsRequested)
			{
				Singleton<FriendScMgr>.Ins.SendRequestMsg();
			}
			m_cell.gameObject.SetActiveBetter(false);
			m_cell_0.gameObject.SetActiveBetter(false);
			m_cell_1.gameObject.SetActiveBetter(false);
			m_recording.SetActiveBetter(false);
			relationshipScroll = scp_relationship.GetComponent<UIScrollPanel>();
			findFriendScroll = scp_find_friend.GetComponent<UIScrollPanel>();
			recScroll = scp_recording.GetComponent<UIScrollPanel>();
			ClickListener clickListener = ClickListener.Get(btn_ling_di_gui, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003ConInit_003Em__0;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			ClickListener.Get(btn_back, "ui_close").onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__0;
			ClickListener.Get(btn_relationship, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__1;
			ClickListener.Get(btn_attention, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__2;
			ClickListener.Get(btn_fans, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__3;
			ClickListener.Get(btn_black, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__4;
			ClickListener.Get(btn_find_friend, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__5;
			ClickListener.Get(btn_recommend, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__6;
			_003ConInit_003Ec__AnonStorey.inp = inp_search.GetComponent<InputField>();
			ClickListener.Get(btn_search, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__7;
			ClickListener.Get(btn_recording, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__8;
			ClickListener.Get(btn_close_rec, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__9;
			ClickListener.Get(m_rec_bg, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__A;
			ClickListener.Get(btn_change, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__B;
			ClickListener clickListener2 = ClickListener.Get(btn_care_all, string.Empty);
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003ConInit_003Em__1;
			}
			clickListener2.onClick = _003C_003Ef__am_0024cache1;
			SCareCancel.handler = (SCareCancel.Handler)Delegate.Combine(SCareCancel.handler, new SCareCancel.Handler(OnSCareCancel));
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			_countDown = Singleton<FriendScMgr>.Ins.RefreshRecommendCountDown;
			TryStartCountDown();
			careList = Singleton<FriendScMgr>.Ins.CareList;
			fansList = Singleton<FriendScMgr>.Ins.FansList;
			blackList = Singleton<FriendScMgr>.Ins.BlackList;
			_shouldSortFans = true;
			eCurTab = Tab2List.Attention;
			UpdateTabStatus();
			m_ralationship.SetActiveBetter(true);
			m_find_friend.SetActiveBetter(false);
			m_recording.SetActiveBetter(false);
			m_attention.SetActiveBetter(true);
			m_fans.SetActiveBetter(false);
			View.SetLabelText(txt_new_fans_num, Singleton<FriendScMgr>.Ins.NewFansCount);
			FriendEventSc.OnNewFansNumChangeEvent = (Utils.VoidDelegate)Delegate.Combine(FriendEventSc.OnNewFansNumChangeEvent, new Utils.VoidDelegate(OnNewFansNumChange));
			OnAttentionNumChange();
			OnFansNumChange();
			OnNewFansNumChange();
			UpdateRecRedDot();
			UpdateRelationshipScroll(eCurTab);
			if (param is BasicRoleInfo)
			{
				OnShowFindFriend();
			}
			OnMoneyChange();
			SSearchByRoleId.handler = (SSearchByRoleId.Handler)Delegate.Combine(SSearchByRoleId.handler, new SSearchByRoleId.Handler(OnSSearchByRoleId));
			SCare.handler = (SCare.Handler)Delegate.Combine(SCare.handler, new SCare.Handler(OnSCare));
			SFansAdd.handler = (SFansAdd.Handler)Delegate.Combine(SFansAdd.handler, new SFansAdd.Handler(OnSFansAdd));
			SFindManitoPlayer.handler = (SFindManitoPlayer.Handler)Delegate.Combine(SFindManitoPlayer.handler, new SFindManitoPlayer.Handler(OnSFindManitoPlayer));
			BasicInfoScMgr ins = Singleton<BasicInfoScMgr>.Ins;
			ins.UpdateBasicInfo = (Utils.LongDelegate)Delegate.Combine(ins.UpdateBasicInfo, new Utils.LongDelegate(OnUpdateBasicInfo));
			SFriendInfo.handler = (SFriendInfo.Handler)Delegate.Combine(SFriendInfo.handler, new SFriendInfo.Handler(OnSFriendInfo));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			SFansCut.handler = (SFansCut.Handler)Delegate.Combine(SFansCut.handler, new SFansCut.Handler(OnSFansCut));
			SFriendIntimacyChange.handler = (SFriendIntimacyChange.Handler)Delegate.Combine(SFriendIntimacyChange.handler, new SFriendIntimacyChange.Handler(NeedRefresh));
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		private void OnSFansCut(SFansCut msg)
		{
			_shouldSortFans = true;
			if (Singleton<FriendScMgr>.Ins.IsInCareList(msg.roleId))
			{
				_shouldSortCare = true;
			}
		}

		private void OnSCare(SCare msg)
		{
			if (eCurTab == Tab2List.Attention)
			{
				UpdateRelationshipScroll(eCurTab);
			}
			_shouldSortCare = true;
		}

		private void OnSFriendInfo(SFriendInfo msg)
		{
			careList = msg.cares;
			fansList = msg.fans;
			blackList = msg.blacks;
			UpdateRelationshipScroll(eCurTab);
			OnAttentionNumChange();
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			FriendEventSc.OnNewFansNumChangeEvent = (Utils.VoidDelegate)Delegate.Remove(FriendEventSc.OnNewFansNumChangeEvent, new Utils.VoidDelegate(OnNewFansNumChange));
			SSearchByRoleId.handler = (SSearchByRoleId.Handler)Delegate.Remove(SSearchByRoleId.handler, new SSearchByRoleId.Handler(OnSSearchByRoleId));
			SCare.handler = (SCare.Handler)Delegate.Remove(SCare.handler, new SCare.Handler(OnSCare));
			SFansCut.handler = (SFansCut.Handler)Delegate.Remove(SFansCut.handler, new SFansCut.Handler(OnSFansCut));
			SFansAdd.handler = (SFansAdd.Handler)Delegate.Remove(SFansAdd.handler, new SFansAdd.Handler(OnSFansAdd));
			SFindManitoPlayer.handler = (SFindManitoPlayer.Handler)Delegate.Remove(SFindManitoPlayer.handler, new SFindManitoPlayer.Handler(OnSFindManitoPlayer));
			BasicInfoScMgr ins = Singleton<BasicInfoScMgr>.Ins;
			ins.UpdateBasicInfo = (Utils.LongDelegate)Delegate.Remove(ins.UpdateBasicInfo, new Utils.LongDelegate(OnUpdateBasicInfo));
			SFriendInfo.handler = (SFriendInfo.Handler)Delegate.Remove(SFriendInfo.handler, new SFriendInfo.Handler(OnSFriendInfo));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			SFriendIntimacyChange.handler = (SFriendIntimacyChange.Handler)Delegate.Remove(SFriendIntimacyChange.handler, new SFriendIntimacyChange.Handler(NeedRefresh));
		}

		private void NeedRefresh(SFriendIntimacyChange msg)
		{
			if (eCurTab < Tab2List.Nearby)
			{
				UpdateRelationshipScroll(eCurTab, true);
			}
		}

		private void OnUpdateBasicInfo(long arg)
		{
			UpdateRelationshipScroll(eCurTab, true);
			_shouldSortCare = true;
			_shouldSortFans = true;
		}

		private void OnSFindManitoPlayer(SFindManitoPlayer msg)
		{
			_countDown = Singleton<FriendScMgr>.Ins.RefreshRecommendCountDown;
			TryStartCountDown();
			if (eCurTab == Tab2List.Recommend)
			{
				UpdateRelationshipScroll(eCurTab);
			}
		}

		private void TryStartCountDown()
		{
			bool flag = _countDown > Time.realtimeSinceStartup;
			txt_count_down.SetActiveBetter(flag);
			txt_change.SetActiveBetter(!flag);
		}

		protected void Update()
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup < _countDown)
			{
				_passTime += Time.realtimeSinceStartup;
				if (_passTime > 1f)
				{
					_passTime = 0f;
					View.SetLabelText(txt_count_downText, Utils.GetString(116, (int)(_countDown - realtimeSinceStartup)));
				}
			}
			else if (txt_count_down.activeSelf)
			{
				txt_count_down.SetActiveBetter(false);
				txt_change.SetActiveBetter(true);
			}
		}

		private void OnSFansAdd(SFansAdd msg)
		{
			OnNewFansNumChange();
			if (eCurTab == Tab2List.Fans)
			{
				UpdateRelationshipScroll(eCurTab, true);
			}
			_shouldSortFans = true;
		}

		private void OnShowFindFriend()
		{
			if (!m_find_friend.activeSelf)
			{
				m_find_friend.SetActiveBetter(true);
				m_ralationship.SetActiveBetter(false);
				eCurTab = Tab2List.Recommend;
				inp_search.GetComponent<InputField>().text = string.Empty;
				UpdateTabStatus();
				UpdateRelationshipScroll(eCurTab);
			}
		}

		private void UpdateRecRedDot()
		{
			m_red_dot_rec.SetActiveBetter(Singleton<FriendScMgr>.Ins.NewRecData > 0);
			View.SetLabelText(txt_new_rec, Singleton<FriendScMgr>.Ins.NewRecData);
			if (m_red_dot_gx.activeSelf != m_red_dot_fans.activeSelf)
			{
				m_red_dot_gx.SetActiveBetter(m_red_dot_rec.activeSelf);
			}
		}

		private void UpdateTabStatus()
		{
			btn_relationship.GetComponent<RadioButton>().isChecked = eCurTab <= Tab2List.Blacklist;
			btn_find_friend.GetComponent<RadioButton>().isChecked = eCurTab >= Tab2List.Nearby;
			btn_attention.GetComponent<RadioButton>().isChecked = eCurTab == Tab2List.Attention;
			btn_fans.GetComponent<RadioButton>().isChecked = eCurTab == Tab2List.Fans;
			btn_black.GetComponent<RadioButton>().isChecked = eCurTab == Tab2List.Blacklist;
		}

		private void OnSSearchByRoleId(SSearchByRoleId msg)
		{
			searchResultList = Singleton<FriendScMgr>.Ins.GetSearchResult;
			eCurTab = Tab2List.Search;
			UpdateTabStatus();
			findFriendScroll.Reset(searchResultList.Count, FillSearchCell);
		}

		private void OnAttentionNumChange()
		{
			View.SetLabelText(txt_attention_num, careList.Count);
		}

		private void OnFansNumChange()
		{
			View.SetLabelText(txt_fans_num, fansList.Count);
		}

		private void OnNewFansNumChange()
		{
			View.SetLabelText(txt_new_fans_num, Singleton<FriendScMgr>.Ins.NewFansCount);
			if (Singleton<FriendScMgr>.Ins.NewFansCount <= 0 && m_red_dot_fans.activeSelf)
			{
				m_red_dot_fans.SetActiveBetter(false);
			}
			else if (Singleton<FriendScMgr>.Ins.NewFansCount > 0 && !m_red_dot_fans.activeSelf)
			{
				m_red_dot_fans.SetActiveBetter(true);
			}
			if (m_red_dot_gx.activeSelf != m_red_dot_fans.activeSelf)
			{
				m_red_dot_gx.SetActiveBetter(m_red_dot_fans.activeSelf);
			}
		}

		private void UpdateRelationshipScroll(Tab2List curTab, bool isResetNoPos = false)
		{
			switch (curTab)
			{
			case Tab2List.Attention:
				if (_shouldSortCare)
				{
					_shouldSortCare = false;
					Singleton<FriendScMgr>.Ins.SortCareList();
				}
				if (isResetNoPos)
				{
					relationshipScroll.ResetNoPosClear(careList.Count, FillAttentionCell);
				}
				else
				{
					relationshipScroll.Reset(careList.Count, FillAttentionCell);
				}
				break;
			case Tab2List.Fans:
				if (_shouldSortFans)
				{
					_shouldSortFans = false;
					Singleton<FriendScMgr>.Ins.SortFansList();
				}
				if (isResetNoPos)
				{
					relationshipScroll.ResetNoPosClear(fansList.Count, FillFansCell);
				}
				else
				{
					relationshipScroll.Reset(fansList.Count, FillFansCell);
				}
				break;
			case Tab2List.Blacklist:
				relationshipScroll.Reset(blackList.Count, FillBlackCell);
				break;
			case Tab2List.Recommend:
				if (Singleton<FriendScMgr>.Ins.GSRecommend == null)
				{
					Singleton<FriendScMgr>.Ins.SendGetBatchRecommendFriends();
					findFriendScroll.Reset(0, FillRecommendCell);
				}
				else
				{
					findFriendScroll.Reset(Singleton<FriendScMgr>.Ins.GSRecommend.players.Count, FillRecommendCell);
				}
				break;
			case Tab2List.Nearby:
			case Tab2List.Skilled:
				break;
			}
		}

		private void SetCareSprite(GameObject parent, GameObject go, long id)
		{
			parent.SetActiveBetter(true);
			go.SetActiveBetter(true);
			if (Singleton<FriendScMgr>.Ins.IsCareEachother(id))
			{
				View.SetItemSprite(go, cfg.Consts.FRIEND_EACH_ICON);
				return;
			}
			if (Singleton<FriendScMgr>.Ins.IsInCareList(id))
			{
				View.SetItemSprite(go, cfg.Consts.FRIEND_CARE_ICON);
				return;
			}
			if (Singleton<FriendScMgr>.Ins.IsInFansList(id))
			{
				View.SetItemSprite(go, cfg.Consts.FRIEND_FANS_ICON);
				return;
			}
			parent.SetActive(false);
			go.SetActive(false);
		}

		private void FillAttentionCell(GameObject go, int index)
		{
			GRalationshipCell component = go.GetComponent<GRalationshipCell>();
			FillCommonThings(component, careList[index].roleInfo);
		}

		private void FillFansCell(GameObject go, int index)
		{
			GRalationshipCell component = go.GetComponent<GRalationshipCell>();
			FillCommonThings(component, fansList[index].roleInfo);
		}

		private void FillBlackCell(GameObject go, int index)
		{
			GRalationshipCell component = go.GetComponent<GRalationshipCell>();
			FillCommonThings(component, blackList[index].roleInfo);
		}

		private void FillCommonThings(GRalationshipCell cell, RoleInfo basicRole)
		{
			_003CFillCommonThings_003Ec__AnonStorey1 _003CFillCommonThings_003Ec__AnonStorey = new _003CFillCommonThings_003Ec__AnonStorey1();
			_003CFillCommonThings_003Ec__AnonStorey.info = Singleton<BasicInfoScMgr>.Ins.GetProperty(basicRole.info.roleId, basicRole.info.version, basicRole.info.name);
			RoleHead component = cell.m_icon.GetComponent<RoleHead>();
			component.Fill(basicRole.info);
			cell.m_qinMiDu.SetActiveBetter(false);
			_003CFillCommonThings_003Ec__AnonStorey.status = _003CFillCommonThings_003Ec__AnonStorey.info.status;
			cell.m_status.SetActiveBetter(_003CFillCommonThings_003Ec__AnonStorey.status <= 0);
			if ((_003CFillCommonThings_003Ec__AnonStorey.status & 4) > 0)
			{
				cell.txt_invity_online_stute_battle.SetActiveBetter(true);
				cell.txt_invity_online_stute_match.SetActiveBetter(false);
				cell.txt_status.SetActiveBetter(false);
				cell.txt_in_team.SetActiveBetter(false);
			}
			else if ((_003CFillCommonThings_003Ec__AnonStorey.status & 0x10) > 0)
			{
				cell.txt_invity_online_stute_battle.SetActiveBetter(false);
				cell.txt_invity_online_stute_match.SetActiveBetter(true);
				cell.txt_status.SetActiveBetter(false);
				cell.txt_in_team.SetActiveBetter(false);
			}
			else if ((_003CFillCommonThings_003Ec__AnonStorey.status & 2) > 0)
			{
				cell.txt_invity_online_stute_battle.SetActiveBetter(false);
				cell.txt_invity_online_stute_match.SetActiveBetter(false);
				cell.txt_status.SetActiveBetter(false);
				cell.txt_in_team.SetActiveBetter(true);
			}
			else if ((_003CFillCommonThings_003Ec__AnonStorey.status & 1) > 0)
			{
				cell.txt_invity_online_stute_battle.SetActiveBetter(false);
				cell.txt_invity_online_stute_match.SetActiveBetter(false);
				cell.txt_status.SetActiveBetter(true);
				cell.txt_in_team.SetActiveBetter(false);
			}
			else if (_003CFillCommonThings_003Ec__AnonStorey.status <= 0)
			{
				cell.txt_invity_online_stute_battle.SetActiveBetter(false);
				cell.txt_invity_online_stute_match.SetActiveBetter(false);
				cell.txt_status.SetActiveBetter(false);
				cell.txt_in_team.SetActiveBetter(false);
			}
			long roleId = _003CFillCommonThings_003Ec__AnonStorey.info.basicRoleInfo.roleId;
			SetCareSprite(cell.m_care_parent, cell.m_care_eachother, roleId);
			View.SetLabelText(cell.txt_roleIdText, Singleton<RoleMgr>.Ins.getIDKey(roleId));
			ClickListener.Get(cell.btn_team, string.Empty).onClick = _003CFillCommonThings_003Ec__AnonStorey._003C_003Em__0;
			ClickListener.Get(cell.btn_look_detailInfo, string.Empty).onClick = _003CFillCommonThings_003Ec__AnonStorey._003C_003Em__1;
		}

		private void FillSearchCell(GameObject go, int index)
		{
			_003CFillSearchCell_003Ec__AnonStorey2 _003CFillSearchCell_003Ec__AnonStorey = new _003CFillSearchCell_003Ec__AnonStorey2();
			GFindFriendCell component = go.GetComponent<GFindFriendCell>();
			_003CFillSearchCell_003Ec__AnonStorey.info = searchResultList[index];
			SetCareSprite(component.m_care_parent, component.m_care_eachother, _003CFillSearchCell_003Ec__AnonStorey.info.info.roleId);
			RoleHead component2 = component.m_icon.GetComponent<RoleHead>();
			component2.Fill(_003CFillSearchCell_003Ec__AnonStorey.info.info);
			View.SetLabelText(component.txt_roleIdText, Singleton<RoleMgr>.Ins.getIDKey(_003CFillSearchCell_003Ec__AnonStorey.info.info.roleId));
			ClickListener.Get(component.btn_look_detailInfo, string.Empty).onClick = _003CFillSearchCell_003Ec__AnonStorey._003C_003Em__0;
			component.btn_care.SetActiveBetter(_003CFillSearchCell_003Ec__AnonStorey.info.info.roleId != Singleton<RoleMgr>.Ins.info.roleId);
			ClickListener.Get(component.btn_care, string.Empty).onClick = _003CFillSearchCell_003Ec__AnonStorey._003C_003Em__1;
		}

		private void FillRecommendCell(GameObject go, int index)
		{
			_003CFillRecommendCell_003Ec__AnonStorey3 _003CFillRecommendCell_003Ec__AnonStorey = new _003CFillRecommendCell_003Ec__AnonStorey3();
			GFindFriendCell component = go.GetComponent<GFindFriendCell>();
			_003CFillRecommendCell_003Ec__AnonStorey.info = Singleton<FriendScMgr>.Ins.GSRecommend.players[index];
			SetCareSprite(component.m_care_parent, component.m_care_eachother, _003CFillRecommendCell_003Ec__AnonStorey.info.info.roleId);
			RoleHead component2 = component.m_icon.GetComponent<RoleHead>();
			component2.Fill(_003CFillRecommendCell_003Ec__AnonStorey.info.info);
			View.SetLabelText(component.txt_roleIdText, Singleton<RoleMgr>.Ins.getIDKey(_003CFillRecommendCell_003Ec__AnonStorey.info.info.roleId));
			ClickListener.Get(component.btn_look_detailInfo, string.Empty).onClick = _003CFillRecommendCell_003Ec__AnonStorey._003C_003Em__0;
			ClickListener.Get(component.btn_care, string.Empty).onClick = _003CFillRecommendCell_003Ec__AnonStorey._003C_003Em__1;
		}

		private void UpdateRecScroll()
		{
			recScroll.Reset(Singleton<FriendRecDataSc>.Ins.GetNewsCount(), FillRecScroll);
		}

		private void FillRecScroll(GameObject go, int index)
		{
			GFriendRecordCell component = go.GetComponent<GFriendRecordCell>();
			string news = Singleton<FriendRecDataSc>.Ins.GetNews(index);
			if (string.IsNullOrEmpty(news))
			{
				go.SetActiveBetter(false);
				return;
			}
			string[] array = news.Split(Singleton<FriendScMgr>.Ins.RecSeparator);
			string text = ((!array[0].Equals(Singleton<FriendScMgr>.Ins.RecCareIdentification)) ? Utils.GetString(164, array[2]) : Utils.GetString(163, array[2]));
			string text2 = DateTime.Parse(DateTime.UtcNow.ToString("1970-01-01 00:00:00")).AddSeconds(Convert.ToInt64(array[1])).ToLocalTime()
				.ToString("MM/dd HH:MM");
			View.SetLabelText(component.txt_date, text2);
			View.SetLabelText(component.txt_content, text);
		}

		protected override void onDestroy()
		{
			Singleton<FriendScMgr>.Ins.NextNearBy = 0;
			Singleton<FriendScMgr>.Ins.HasMoreNearBy = true;
			SCareCancel.handler = (SCareCancel.Handler)Delegate.Remove(SCareCancel.handler, new SCareCancel.Handler(OnSCareCancel));
		}

		private void OnSCareCancel(SCareCancel msg)
		{
			_shouldSortCare = true;
			if (Singleton<FriendScMgr>.Ins.IsInFansList(msg.roleId))
			{
				_shouldSortFans = true;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_relationship = component.GameObjects[0].gameObject;
			txt_on = component.GameObjects[1].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[2].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			m_red_dot_gx = component.GameObjects[3].gameObject;
			btn_find_friend = component.GameObjects[4].gameObject;
			m_ralationship = component.GameObjects[5].gameObject;
			btn_attention = component.GameObjects[6].gameObject;
			btn_fans = component.GameObjects[7].gameObject;
			m_red_dot_fans = component.GameObjects[8].gameObject;
			txt_new_fans_num = component.GameObjects[9].gameObject;
			txt_new_fans_numText = txt_new_fans_num.GetComponent<Text>();
			btn_black = component.GameObjects[10].gameObject;
			btn_recording = component.GameObjects[11].gameObject;
			m_red_dot_rec = component.GameObjects[12].gameObject;
			txt_new_rec = component.GameObjects[13].gameObject;
			txt_new_recText = txt_new_rec.GetComponent<Text>();
			scp_relationship = component.GameObjects[14].gameObject;
			m_cell = View.AddComponentIfNotExist<GRalationshipCell>(component.GameObjects[15].gameObject);
			m_attention = component.GameObjects[16].gameObject;
			txt_attention_num = component.GameObjects[17].gameObject;
			txt_attention_numText = txt_attention_num.GetComponent<Text>();
			m_fans = component.GameObjects[18].gameObject;
			txt_fans_num = component.GameObjects[19].gameObject;
			txt_fans_numText = txt_fans_num.GetComponent<Text>();
			m_find_friend = component.GameObjects[20].gameObject;
			btn_recommend = component.GameObjects[21].gameObject;
			inp_search = component.GameObjects[22].gameObject;
			btn_search = component.GameObjects[23].gameObject;
			scp_find_friend = component.GameObjects[24].gameObject;
			btn_back = component.GameObjects[25].gameObject;
			m_recording = component.GameObjects[26].gameObject;
			m_rec_bg = component.GameObjects[27].gameObject;
			btn_close_rec = component.GameObjects[28].gameObject;
			scp_recording = component.GameObjects[29].gameObject;
			txt_blood = component.GameObjects[30].gameObject;
			txt_bloodText = txt_blood.GetComponent<Text>();
			txt_hunger = component.GameObjects[31].gameObject;
			txt_hungerText = txt_hunger.GetComponent<Text>();
			txt_thirst = component.GameObjects[32].gameObject;
			txt_thirstText = txt_thirst.GetComponent<Text>();
			btn_ling_di_gui = component.GameObjects[33].gameObject;
			m_search = component.GameObjects[34].gameObject;
			m_care_all = component.GameObjects[35].gameObject;
			btn_care_all = component.GameObjects[36].gameObject;
			m_change = component.GameObjects[37].gameObject;
			btn_change = component.GameObjects[38].gameObject;
			txt_change = component.GameObjects[39].gameObject;
			txt_changeText = txt_change.GetComponent<Text>();
			txt_count_down = component.GameObjects[40].gameObject;
			txt_count_downText = txt_count_down.GetComponent<Text>();
			txt_on_0 = component.GameObjects[41].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[42].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_on_1 = component.GameObjects[43].gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_off_1 = component.GameObjects[44].gameObject;
			txt_off_1Text = txt_off_1.GetComponent<Text>();
			txt_on_2 = component.GameObjects[45].gameObject;
			txt_on_2Text = txt_on_2.GetComponent<Text>();
			txt_off_2 = component.GameObjects[46].gameObject;
			txt_off_2Text = txt_off_2.GetComponent<Text>();
			txt_on_3 = component.GameObjects[47].gameObject;
			txt_on_3Text = txt_on_3.GetComponent<Text>();
			txt_off_3 = component.GameObjects[48].gameObject;
			txt_off_3Text = txt_off_3.GetComponent<Text>();
			m_cell_0 = View.AddComponentIfNotExist<GFindFriendCell>(component.GameObjects[49].gameObject);
			txt_on_4 = component.GameObjects[50].gameObject;
			txt_on_4Text = txt_on_4.GetComponent<Text>();
			m_cell_1 = View.AddComponentIfNotExist<GFriendRecordCell>(component.GameObjects[51].gameObject);
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__0(GameObject go)
		{
			if (Singleton<FriendPermitMgr>.Ins.PermitInfo.ChestPermits.Count <= 0)
			{
				AlertBox.Show(242);
			}
			else
			{
				ViewMgr.Ins.ShowView<FriendPermitPanel>(null, false);
			}
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__1(GameObject go)
		{
			bool flag = true;
			List<RoleInfo> players = Singleton<FriendScMgr>.Ins.GSRecommend.players;
			int i = 0;
			for (int count = players.Count; i < count; i++)
			{
				long roleId = players[i].info.roleId;
				if (!Singleton<FriendScMgr>.Ins.IsInCareList(roleId))
				{
					Singleton<FriendScMgr>.Ins.AddCare(roleId);
					flag = false;
				}
			}
			if (flag)
			{
				AlertBox.Show(416);
			}
		}
	}
}
