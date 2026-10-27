using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.chat.scmsg;
using gs.drop.scmsg;
using gs.role.scmsg;
using gs.troop.scmsg;

namespace SC.UI
{
	public class ChatPanel : View
	{
		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey1
		{
			internal KeyValuePair<int, GameObject> pair1;

			internal ChatPanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this.OnClickTab(pair1.Key);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillEmojiCell_003Ec__AnonStorey2
		{
			internal int index;

			internal ChatPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				byte[] bytes = Encoding.Default.GetBytes(_0024this._inp.text);
				if (bytes.Length > cfg.Consts.MAX_CHAT_WORD_NUM - 11)
				{
					AlertBox.Show(44);
					return;
				}
				_0024this._inp.text += string.Format("[#emoji_{0}]", index);
				_0024this.m_emoji.SetActive(false);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillRoleCell_003Ec__AnonStorey3
		{
			internal AllBasicInfo info;

			internal ChatPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this._isLockScreen = true;
				_0024this._selectInfo = info;
				_0024this.m_player_btns.SetActiveBetter(true);
				_0024this.UpdatePlayerBtns();
			}

			internal void _003C_003Em__1(GameObject o)
			{
				_0024this._nSelectId = info.basicRoleInfo.roleId;
				View.SetLabelText(_0024this.txt_select_nameText, info.basicRoleInfo.name);
				Singleton<ChatScMgr>.Ins.ClearNewPMsgNum(_0024this._nSelectId);
				_0024this.UpdateRolePanel(false);
				_0024this._msgList.Clear();
				RoundListSc<MsgBean> chats = _0024this._id2PMsg[_0024this._nSelectId].chats;
				int i = 0;
				for (int num = chats.size(); i < num; i++)
				{
					_0024this._msgList.Add(chats.get(i));
				}
				_0024this.UpdateChatMsgPanel(false, true);
				_0024this.UpdateRedDot(100);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillWorlChatCell_003Ec__AnonStorey4
		{
			internal AllBasicInfo info;

			internal ChatPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this._isLockScreen = true;
				_0024this._selectInfo = info;
				_0024this.m_player_btns.SetActiveBetter(true);
				_0024this.UpdatePlayerBtns();
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillRoleChatCell_003Ec__AnonStorey5
		{
			internal AllBasicInfo info;

			internal ChatPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this._isLockScreen = true;
				_0024this._selectInfo = info;
				_0024this.m_player_btns.SetActiveBetter(true);
				_0024this.UpdatePlayerBtns();
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillTeamInviteCell_003Ec__AnonStorey6
		{
			internal AllBasicInfo info;

			internal BasicRoleInfo basicInfo;

			internal ChatPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				_0024this._isLockScreen = true;
				_0024this._selectInfo = info;
				_0024this.m_player_btns.SetActiveBetter(true);
				_0024this.UpdatePlayerBtns();
			}

			internal void _003C_003Em__1(GameObject o)
			{
				if (Singleton<TeamScMgr>.Ins.IsInTeam())
				{
					AlertBox.Show(74);
				}
				else
				{
					Singleton<TeamScMgr>.Ins.SendApplyToTeam(basicInfo.roleId);
				}
			}
		}

		public const int DEFAULT_OPEN_TAB = 1;

		public const int CLAN = 40;

		public const int CHAT = 100;

		private int _curTabPage = -1;

		private bool _isLockScreen;

		private long _nSelectId;

		private UIScrollPanel _worldChatScroll;

		private UIScrollPanel _roleInfoScroll;

		private UIScrollPanel _roleChatScroll;

		private UIScrollPanel _teamInviteScroll;

		private UIScrollPanel _junTuanInviteScroll;

		private Dictionary<int, NewChatInfoData<MsgBean>> _tab2MsgList;

		private Dictionary<long, NewChatInfoData<MsgBean>> _id2PMsg;

		private readonly Dictionary<int, GameObject> _tab2Button = new Dictionary<int, GameObject>();

		private readonly List<MsgBean> _msgList = new List<MsgBean>();

		private readonly List<RoleVersion> _roleList = new List<RoleVersion>();

		private readonly Dictionary<int, long> _cell2RoleId = new Dictionary<int, long>();

		private readonly Dictionary<int, long> _index2RoleId = new Dictionary<int, long>();

		private InputField _inp;

		private readonly HashSet<int> _canSay = new HashSet<int>();

		private SpriteAsset _spriteAsset;

		private long _otherZhanDuiId = -1L;

		private int _onlineRoleNum;

		private int _totalRoleNum;

		private float _intervalTime = 0.4f;

		private float _timer = 0.4f;

		private Coroutine _waitToShowBigHorn;

		private AllBasicInfo _selectInfo;

		private CPrivateMsg _cPrivateMsg = new CPrivateMsg();

		private GameObject btn_back;

		private GameObject m_roleinfo_panel;

		private GameObject txt_select_name;

		private Text txt_select_nameText;

		private GameObject btn_show_btns;

		private GameObject scp_role_chatList;

		private GChatRoleChatCell m_cell;

		private GameObject txt_online_num;

		private Text txt_online_numText;

		private GameObject scp_role;

		private GameObject m_norole_panel;

		private GameObject scp_chatlist;

		private GameObject scp_team_invite;

		private GameObject scp_juntuan_invite;

		private GBigHornInfo m_big_horn_info;

		private GameObject btn_new_msg;

		private GameObject txt_new_msg;

		private Text txt_new_msgText;

		private GameObject m_cant_send;

		private GameObject m_can_send;

		private GameObject btn_emoji;

		private GameObject btn_small_horn;

		private GameObject txt_small_num;

		private Text txt_small_numText;

		private GameObject btn_big_horn;

		private GameObject txt_big_num;

		private Text txt_big_numText;

		private GameObject inp_chat;

		private GameObject btn_send;

		private GameObject txt_send;

		private Text txt_sendText;

		private GameObject txt_time;

		private Text txt_timeText;

		private GameObject m_emoji;

		private GameObject scp_emoji;

		private GameObject m_all_panel;

		private ChatBtnRedDot ckb_world;

		private ChatBtnRedDot ckb_chat;

		private ChatBtnRedDot ckb_team;

		private GameObject ckb_team_invite;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject m_team_red_dot;

		private ChatBtnRedDot ckb_corps;

		private ChatBtnRedDot ckb_system;

		private GameObject ckb_juntuan;

		private GameObject m_juntuan_red_dot;

		private GameObject m_player_btns;

		private GameObject m_player_btns_close;

		private GameObject m_player_btns_bg;

		private GameObject txt_player_name;

		private Text txt_player_nameText;

		private GameObject txt_level_select;

		private Text txt_level_selectText;

		private GameObject m_zhandui_flag_select;

		private GameObject txt_zhandui_name_select;

		private Text txt_zhandui_name_selectText;

		private GameObject btn_basicinfo_one;

		private GameObject btn_care_one;

		private GameObject btn_chat_one;

		private GameObject btn_team_one;

		private GameObject txt_team_one;

		private Text txt_team_oneText;

		private GameObject btn_zhandui_one;

		private GameObject txt_zhandui_one;

		private Text txt_zhandui_oneText;

		private GameObject btn_black_one;

		private GChatRoleInfoCell m_cell_0;

		private OneChatCell m_cell_1;

		private GChatTeamInviteCell m_cell_2;

		private GChatTeamInviteCell m_cell_3;

		private GChatEmojiCell m_cell_4;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		protected override void onInit()
		{
			base.onInit();
			m_cell.gameObject.SetActive(false);
			m_cell_0.gameObject.SetActive(false);
			m_cell_1.gameObject.SetActive(false);
			m_cell_2.gameObject.SetActive(false);
			m_cell_3.gameObject.SetActive(false);
			m_cell.m_other.SetActive(false);
			m_cell.m_self.SetActive(false);
			m_big_horn_info.gameObject.SetActive(false);
			btn_new_msg.SetActive(false);
			m_roleinfo_panel.SetActive(false);
			Transform transform = m_player_btns_bg.transform.Find("3");
			if ((bool)transform)
			{
				transform.gameObject.SetActive(false);
			}
			LoadEmoji();
			_canSay.Add(1);
			_canSay.Add(100);
			_canSay.Add(3);
			_canSay.Add(4);
			_tab2Button[4] = ckb_corps.gameObject;
			_tab2Button[100] = ckb_chat.gameObject;
			_tab2Button[3] = ckb_team.gameObject;
			_tab2Button[1] = ckb_world.gameObject;
			_tab2Button[8] = ckb_team_invite;
			_tab2Button[2] = ckb_system.gameObject;
			_tab2Button[9] = ckb_juntuan;
			foreach (KeyValuePair<int, GameObject> item in _tab2Button)
			{
				_003ConInit_003Ec__AnonStorey1 _003ConInit_003Ec__AnonStorey = new _003ConInit_003Ec__AnonStorey1();
				_003ConInit_003Ec__AnonStorey._0024this = this;
				_003ConInit_003Ec__AnonStorey.pair1 = item;
				ClickListener.Get(item.Value, string.Empty).onClick = _003ConInit_003Ec__AnonStorey._003C_003Em__0;
			}
			_tab2Button[100].GetComponent<RadioButton>().OnValueChanged = _003ConInit_003Em__0;
			_tab2MsgList = Singleton<ChatScMgr>.Ins.tab2msgList;
			_id2PMsg = Singleton<ChatScMgr>.Ins.id2PMsgList;
			_inp = inp_chat.GetComponent<InputField>();
			_worldChatScroll = scp_chatlist.GetComponent<UIScrollPanel>();
			_roleInfoScroll = scp_role.GetComponent<UIScrollPanel>();
			_roleChatScroll = scp_role_chatList.GetComponent<UIScrollPanel>();
			_teamInviteScroll = scp_team_invite.GetComponent<UIScrollPanel>();
			_junTuanInviteScroll = scp_juntuan_invite.GetComponent<UIScrollPanel>();
			_inp.onValueChanged.RemoveAllListeners();
			_inp.lineType = InputField.LineType.SingleLine;
			_inp.characterLimit = cfg.Consts.MAX_CHAT_WORD_NUM;
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__1;
			try
			{
				ClickListener.Get(base.transform.Find("Image (1)").gameObject, string.Empty).onClick = _003ConInit_003Em__2;
			}
			catch (Exception)
			{
			}
			ClickListener.Get(btn_emoji, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_new_msg, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener.Get(m_player_btns_close, string.Empty).onClick = _003ConInit_003Em__5;
			ClickListener.Get(btn_basicinfo_one, string.Empty).onClick = _003ConInit_003Em__6;
			ClickListener.Get(btn_care_one, string.Empty).onClick = _003ConInit_003Em__7;
			ClickListener.Get(btn_chat_one, string.Empty).onClick = _003ConInit_003Em__8;
			ClickListener.Get(btn_team_one, string.Empty).onClick = _003ConInit_003Em__9;
			ClickListener.Get(btn_black_one, string.Empty).onClick = _003ConInit_003Em__A;
		}

		private void LoadEmoji()
		{
			ResMgr.Ins.LoadAssetFromAB<SpriteAsset>("icon/emoji.ab", "emoji", m_emoji, _003CLoadEmoji_003Em__B);
		}

		private void OnBasicInfoUpdateEvent(long otherId)
		{
			foreach (KeyValuePair<int, long> item in _cell2RoleId)
			{
				if (item.Value == otherId)
				{
					UIScrollPanel.FillCell func = null;
					UIScrollPanel uIScrollPanel = null;
					switch (_curTabPage)
					{
					case 1:
					case 2:
						func = FillWorlChatCell;
						uIScrollPanel = _worldChatScroll;
						break;
					case 8:
						func = FillTeamInviteCell;
						uIScrollPanel = _teamInviteScroll;
						break;
					case 9:
						func = FillTeamInviteCell;
						uIScrollPanel = _junTuanInviteScroll;
						break;
					case 3:
					case 4:
					case 100:
						func = FillRoleChatCell;
						uIScrollPanel = _roleChatScroll;
						break;
					}
					if (uIScrollPanel != null)
					{
						uIScrollPanel.UpdateCell(item.Key, func);
					}
				}
			}
			foreach (KeyValuePair<int, long> item2 in _index2RoleId)
			{
				if (item2.Value == otherId)
				{
					_onlineRoleNum--;
					_totalRoleNum--;
					_roleInfoScroll.UpdateCell(item2.Key, FillRoleCell);
				}
			}
			MsgBean hornBean = Singleton<ChatScMgr>.Ins.HornBean;
			if (hornBean != null && hornBean.role.roleId == otherId)
			{
				UpdateBigHornMsg();
			}
			if (_selectInfo != null && _selectInfo.basicRoleInfo.roleId == otherId)
			{
				_selectInfo = Singleton<BasicInfoScMgr>.Ins.GetProperty(otherId, 0);
				UpdatePlayerBtns();
			}
			SetOnlineNumText();
		}

		private void UpdateNewMsgBtn()
		{
			btn_new_msg.SetActive(true);
			View.SetLabelText(txt_new_msg, Utils.GetString(76, Singleton<ChatScMgr>.Ins.NewWorldMsgNum(_curTabPage)));
		}

		private void OnSpublicMsg(SPublicMsg msg)
		{
			if (!Singleton<ChatScMgr>.Ins.Type2Tab.ContainsKey(msg.info.msgType))
			{
				return;
			}
			int num = Singleton<ChatScMgr>.Ins.Type2Tab[msg.info.msgType];
			if (num == _curTabPage)
			{
				if (!_isLockScreen)
				{
					Singleton<ChatScMgr>.Ins.ClearNewMsgNum(_curTabPage);
				}
				else
				{
					UpdateNewMsgBtn();
					UpdateRedDot(_curTabPage);
				}
				_msgList.Add(msg.info);
				UpdateChatMsgPanel(true);
			}
			else
			{
				UpdateRedDot(num);
			}
		}

		private void FillEmojiCell(GameObject go, int index)
		{
			_003CFillEmojiCell_003Ec__AnonStorey2 _003CFillEmojiCell_003Ec__AnonStorey = new _003CFillEmojiCell_003Ec__AnonStorey2();
			_003CFillEmojiCell_003Ec__AnonStorey.index = index;
			_003CFillEmojiCell_003Ec__AnonStorey._0024this = this;
			GChatEmojiCell component = go.GetComponent<GChatEmojiCell>();
			component.m_emoji.GetComponent<Image>().sprite = _spriteAsset.listSpriteGroup[_003CFillEmojiCell_003Ec__AnonStorey.index].listSpriteInfor[0].sprite;
			ClickListener.Get(component.m_emoji, string.Empty).onClick = _003CFillEmojiCell_003Ec__AnonStorey._003C_003Em__0;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow();
			m_emoji.SetActive(false);
			btn_new_msg.SetActive(false);
			m_player_btns.SetActiveBetter(false);
			_curTabPage = 1;
			RoleVersion roleVersion = param as RoleVersion;
			if (roleVersion != null && roleVersion.roleId > 0)
			{
				_curTabPage = 100;
				Singleton<ChatScMgr>.Ins.AddNewPrivateChatItem(roleVersion.roleId, roleVersion.version);
				_nSelectId = roleVersion.roleId;
			}
			if (_nSelectId > 0 && _curTabPage == 100)
			{
				Singleton<ChatScMgr>.Ins.ClearNewPMsgNum(_nSelectId);
				UpdateRolePanel(false);
			}
			if (_tab2Button.ContainsKey(_curTabPage))
			{
				RadioButton.ChooseBtn(_tab2Button[_curTabPage]);
			}
			Singleton<ChatScMgr>.Ins.ClearNewMsgNum(_curTabPage);
			UpdateBigHornMsg();
			UpdateChatInput();
			UpdateAllRedDots();
			OnClickTab(_curTabPage);
			View.SetLabelText(txt_big_numText, Singleton<BagMgr>.Ins.GetItemNum(cfg.Consts.DALABA_ITEM_ID));
			View.SetLabelText(txt_small_numText, Singleton<BagMgr>.Ins.GetItemNum(cfg.Consts.XIAOLABA_ITEM_ID));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Combine(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemNumerChange));
			BasicInfoScMgr ins = Singleton<BasicInfoScMgr>.Ins;
			ins.UpdateBasicInfo = (Utils.LongDelegate)Delegate.Combine(ins.UpdateBasicInfo, new Utils.LongDelegate(OnBasicInfoUpdateEvent));
			ChatEventSc.CloseBigHorn = (Utils.VoidDelegate)Delegate.Combine(ChatEventSc.CloseBigHorn, new Utils.VoidDelegate(OnCloseBigHorn));
			SPublicMsg.handler = (SPublicMsg.Handler)Delegate.Combine(SPublicMsg.handler, new SPublicMsg.Handler(OnSpublicMsg));
			SBigHorn.handler = (SBigHorn.Handler)Delegate.Combine(SBigHorn.handler, new SBigHorn.Handler(OnBigHornMsgHandle));
			SPrivateMsg.handler = (SPrivateMsg.Handler)Delegate.Combine(SPrivateMsg.handler, new SPrivateMsg.Handler(OnSPrivateMsg));
		}

		private void OnItemNumerChange(int itemId, int number)
		{
			OnSUseItem(itemId, number, null);
		}

		private void OnSUseItem(int itemId, int number, DropDetail dropDetail)
		{
			if (itemId == cfg.Consts.DALABA_ITEM_ID)
			{
				View.SetLabelText(txt_big_numText, number);
			}
			else if (itemId == cfg.Consts.XIAOLABA_ITEM_ID)
			{
				View.SetLabelText(txt_small_numText, number);
			}
		}

		private void UpdateAllRedDots()
		{
			UpdateRedDot(1);
			UpdateRedDot(100);
			UpdateRedDot(4);
			UpdateRedDot(3);
			UpdateRedDot(2);
			UpdateRedDot(8);
			UpdateRedDot(9);
		}

		private void OnCloseBigHorn()
		{
			m_big_horn_info.gameObject.SetActive(false);
		}

		private void OnClickTab(int tabType)
		{
			btn_show_btns.SetActiveBetter(false);
			ClearInput();
			SwitchTabPage(tabType);
		}

		private void SwitchTabPage(int tabType)
		{
			btn_big_horn.SetActiveBetter(tabType == 1);
			btn_small_horn.SetActiveBetter(tabType == 1);
			m_can_send.SetActive(_canSay.Contains(tabType));
			m_cant_send.SetActive(!m_can_send.activeSelf);
			_curTabPage = tabType;
			_isLockScreen = false;
			_msgList.Clear();
			_cell2RoleId.Clear();
			m_roleinfo_panel.SetActiveBetter(tabType == 100 || tabType == 3 || tabType == 4);
			m_norole_panel.SetActiveBetter(!m_roleinfo_panel.activeSelf);
			scp_team_invite.SetActiveBetter(m_norole_panel.activeSelf && tabType == 8);
			scp_juntuan_invite.SetActiveBetter(m_norole_panel.activeSelf && tabType == 9);
			scp_chatlist.SetActiveBetter(m_norole_panel.activeSelf && !scp_team_invite.activeSelf && !scp_juntuan_invite.activeSelf);
			if (_tab2MsgList.ContainsKey(_curTabPage))
			{
				Singleton<ChatScMgr>.Ins.ClearNewMsgNum(_curTabPage);
				RoundListSc<MsgBean> chats = _tab2MsgList[_curTabPage].chats;
				int i = 0;
				for (int num = chats.size(); i < num; i++)
				{
					_msgList.Add(chats.get(i));
				}
				if (_curTabPage == 3)
				{
					View.SetLabelText(txt_select_nameText, Utils.GetString(77));
					_roleList.Clear();
					List<TroopPlayer> teamates = Singleton<TeamScMgr>.Ins.GetTeamates();
					int j = 0;
					for (int count = teamates.Count; j < count; j++)
					{
						_roleList.Add(teamates[j].role);
					}
				}
				else if (_curTabPage != 4)
				{
				}
			}
			else
			{
				if (Singleton<ChatScMgr>.Ins.IdList.Count > 0 && _nSelectId <= 0)
				{
					_nSelectId = Singleton<ChatScMgr>.Ins.IdList[0].roleId;
				}
				if (_nSelectId > 0)
				{
					RoundListSc<MsgBean> chats2 = _id2PMsg[_nSelectId].chats;
					int k = 0;
					for (int num2 = chats2.size(); k < num2; k++)
					{
						_msgList.Add(chats2.get(k));
					}
					Singleton<ChatScMgr>.Ins.ClearNewPMsgNum(_nSelectId);
					View.SetLabelText(txt_select_nameText, Singleton<BasicInfoScMgr>.Ins.GetProperty(_nSelectId, 0).basicRoleInfo.name);
				}
				else
				{
					View.SetLabelText(txt_select_nameText, string.Empty);
				}
				_roleList.Clear();
				int l = 0;
				for (int count2 = Singleton<ChatScMgr>.Ins.IdList.Count; l < count2; l++)
				{
					_roleList.Add(Singleton<ChatScMgr>.Ins.IdList[l]);
				}
				CClearPrivateMsg msg = new CClearPrivateMsg();
				Client2Gs.Ins.Send(msg);
			}
			_index2RoleId.Clear();
			UpdateChatMsgPanel(false, true);
			UpdateRolePanel(true);
			UpdateChatInput();
			SetOnlineNumText();
			GameObject value;
			if (_tab2Button.TryGetValue(_curTabPage, out value))
			{
				RadioButton.ChooseBtn(value);
			}
			UpdateRedDot(_curTabPage);
		}

		private void UpdateRolePanel(bool isReset)
		{
			_onlineRoleNum = 0;
			_totalRoleNum = 0;
			if (isReset)
			{
				_roleInfoScroll.Reset(_roleList.Count, FillRoleCell, ClearChatInfoCell);
			}
			else
			{
				_roleInfoScroll.ResetNoPos(_roleList.Count, FillRoleCell, ClearChatInfoCell);
			}
		}

		private void SetOnlineNumText()
		{
			View.SetLabelText(txt_online_numText, Utils.GetString(78, Utils.GetString(9, _onlineRoleNum, _totalRoleNum)));
		}

		private void FillRoleCell(GameObject go, int index)
		{
			_003CFillRoleCell_003Ec__AnonStorey3 _003CFillRoleCell_003Ec__AnonStorey = new _003CFillRoleCell_003Ec__AnonStorey3();
			_003CFillRoleCell_003Ec__AnonStorey._0024this = this;
			GChatRoleInfoCell component = go.GetComponent<GChatRoleInfoCell>();
			_003CFillRoleCell_003Ec__AnonStorey.info = Singleton<BasicInfoScMgr>.Ins.GetProperty(_roleList[index].roleId, _roleList[index].version, _roleList[index].name);
			_totalRoleNum++;
			_onlineRoleNum += ((_003CFillRoleCell_003Ec__AnonStorey.info.status > 0) ? 1 : 0);
			component.m_noselect_bg.SetActive(_nSelectId == _003CFillRoleCell_003Ec__AnonStorey.info.basicRoleInfo.roleId);
			component.txt_lixian.SetActive(_003CFillRoleCell_003Ec__AnonStorey.info.status <= 0);
			component.txt_status.SetActive(_003CFillRoleCell_003Ec__AnonStorey.info.status > 0);
			BasicRoleInfo basicRoleInfo = _003CFillRoleCell_003Ec__AnonStorey.info.basicRoleInfo;
			ClickListener.Get(component.m_icon, string.Empty).onClick = _003CFillRoleCell_003Ec__AnonStorey._003C_003Em__0;
			if (RoleHeadCfg.Get(basicRoleInfo.headId) != null)
			{
				View.SetItemSprite(component.m_icon, RoleHeadCfg.Get(basicRoleInfo.headId).icon);
			}
			if (_003CFillRoleCell_003Ec__AnonStorey.info.basicRoleInfo.frameId > 0)
			{
				View.SetItemSprite(component.m_frame, RoleHeadFrameCfg.Get(_003CFillRoleCell_003Ec__AnonStorey.info.basicRoleInfo.frameId).frame);
			}
			component.m_female.SetActiveBetter(!basicRoleInfo.sex);
			component.m_male.SetActiveBetter(basicRoleInfo.sex);
			View.SetLabelText(component.txt_nameText, basicRoleInfo.name);
			View.SetLabelText(component.txt_levelText, basicRoleInfo.level);
			int status = _003CFillRoleCell_003Ec__AnonStorey.info.status;
			component.txt_lixian.SetActiveBetter(status <= 0);
			if ((status & 4) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(true);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(false);
			}
			else if ((status & 0x10) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(true);
				component.txt_status.SetActiveBetter(false);
			}
			else if ((status & 2) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(false);
			}
			else if ((status & 1) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(true);
			}
			else if (status <= 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(false);
			}
			if (!_index2RoleId.ContainsKey(index))
			{
				_index2RoleId[index] = _003CFillRoleCell_003Ec__AnonStorey.info.basicRoleInfo.roleId;
			}
			if (_curTabPage == 100)
			{
				component.m_red_dot.SetActive(_id2PMsg[_003CFillRoleCell_003Ec__AnonStorey.info.basicRoleInfo.roleId].newMsgNum > 0);
				View.SetLabelText(component.txt_new_msg_numText, _id2PMsg[_003CFillRoleCell_003Ec__AnonStorey.info.basicRoleInfo.roleId].newMsgNum);
				ClickListener.Get(go, string.Empty).onClick = _003CFillRoleCell_003Ec__AnonStorey._003C_003Em__1;
			}
			else
			{
				component.m_red_dot.SetActive(false);
				ClickListener.Get(go, string.Empty).onClick = null;
			}
			UIContext.Attach(go, "RoleIndex", index);
		}

		private void OnSPrivateMsg(SPrivateMsg msg)
		{
			if (_curTabPage != 100)
			{
				UpdateRedDot(100);
			}
			else if (_nSelectId == msg.bean.role.roleId)
			{
				MsgBean msgBean = new MsgBean();
				msgBean.role = msg.bean.role;
				msgBean.text = msg.bean.text;
				_msgList.Add(msgBean);
				UpdateChatMsgPanel(true);
				Singleton<ChatScMgr>.Ins.ClearNewPMsgNum(_nSelectId);
			}
			else
			{
				UpdateRolePanel(false);
			}
		}

		private void UpdateChatInput()
		{
			m_can_send.SetActive(_canSay.Contains(_curTabPage));
			m_cant_send.SetActive(!_canSay.Contains(_curTabPage));
			int num = _getCurSendRemainTime();
			txt_send.SetActive(num <= 0);
			txt_time.SetActive(num > 0);
			if (num <= 0)
			{
				ClickListener.Get(btn_send, string.Empty).onClick = onSendMsg;
				return;
			}
			updateSendRemainTime(num);
			View.SetLabelText(txt_time, Utils.GetString(31, num));
			ClickListener clickListener = ClickListener.Get(btn_send, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CUpdateChatInput_003Em__C;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
		}

		private int _getCurSendRemainTime()
		{
			if (_curTabPage != 1)
			{
				return 0;
			}
			NewChatInfoData<MsgBean> newChatInfoData = _tab2MsgList[_curTabPage];
			return (int)(newChatInfoData.nextTimeSendMsg - Time.realtimeSinceStartup);
		}

		private void updateSendRemainTime(int remain)
		{
			if (txt_send.activeSelf)
			{
				_SendRemainActive(false);
			}
			View.SetLabelText(txt_time, Utils.GetString(31, remain));
		}

		private void _SendRemainActive(bool active)
		{
			txt_time.SetActive(active);
			txt_send.SetActive(!active);
		}

		protected void Update()
		{
			if (_timer <= 0f)
			{
				_timer = _intervalTime;
				NewChatInfoData<MsgBean> value;
				if (txt_time.activeSelf && _tab2MsgList.TryGetValue(_curTabPage, out value))
				{
					float num = value.nextTimeSendMsg - Time.realtimeSinceStartup;
					if (num > 0f)
					{
						updateSendRemainTime((int)num);
					}
					else
					{
						UpdateChatInput();
					}
				}
			}
			else
			{
				_timer -= Time.deltaTime;
			}
		}

		private void UpdateChatMsgPanel(bool isNewMsg = false, bool isImmediatelyEnd = false)
		{
			if (_spriteAsset == null)
			{
				return;
			}
			UIScrollPanel.FillCell fillCell = null;
			UIScrollPanel uIScrollPanel = null;
			switch (_curTabPage)
			{
			case 1:
			case 2:
				fillCell = FillWorlChatCell;
				uIScrollPanel = _worldChatScroll;
				break;
			case 8:
				fillCell = FillTeamInviteCell;
				uIScrollPanel = _teamInviteScroll;
				break;
			case 9:
				fillCell = FillTeamInviteCell;
				uIScrollPanel = _junTuanInviteScroll;
				break;
			case 3:
			case 4:
			case 100:
				fillCell = FillRoleChatCell;
				uIScrollPanel = _roleChatScroll;
				break;
			}
			if (fillCell == null)
			{
				return;
			}
			if (_isLockScreen)
			{
				uIScrollPanel.ResetNoPos(_msgList.Count, fillCell, ClearChatInfoCell);
				return;
			}
			if (isNewMsg)
			{
				uIScrollPanel.ResetNoPos(_msgList.Count, fillCell, ClearChatInfoCell);
			}
			else
			{
				uIScrollPanel.Reset(_msgList.Count, fillCell, ClearChatInfoCell);
			}
			if (_msgList.Count > 6)
			{
				if (isImmediatelyEnd)
				{
					_worldChatScroll.ResetEnd();
					_teamInviteScroll.ResetEnd();
					_roleChatScroll.ResetEnd();
					_junTuanInviteScroll.ResetEnd();
				}
				else
				{
					_worldChatScroll.ResetEnd_OutExpo();
					_teamInviteScroll.ResetEnd_OutExpo();
					_roleChatScroll.ResetEnd_OutExpo();
					_junTuanInviteScroll.ResetEnd();
				}
			}
		}

		private void ClearChatInfoCell(GameObject cell)
		{
			MsgBean msgBean = UIContext.Get<MsgBean>(cell);
			if (_msgList.Count > 0 && _msgList[_msgList.Count - 1] == msgBean)
			{
				_isLockScreen = true;
			}
			_cell2RoleId.Remove(UIContext.Get<int>(cell, "ChatIndex"));
			_index2RoleId.Remove(UIContext.Get<int>(cell, "RoleIndex"));
		}

		private void UpdateBigHornMsg()
		{
			MsgBean hornBean = Singleton<ChatScMgr>.Ins.HornBean;
			if (hornBean == null || hornBean.msgType != 6 || !(_spriteAsset != null))
			{
				m_big_horn_info.gameObject.SetActiveBetter(false);
				return;
			}
			m_big_horn_info.gameObject.SetActiveBetter(true);
			View.SetLabelText(m_big_horn_info.txt_horn_msg, hornBean.text);
			InlineText component = m_big_horn_info.txt_horn_msg.GetComponent<InlineText>();
			component.enabled = false;
			component.enabled = true;
			AllBasicInfo property = Singleton<BasicInfoScMgr>.Ins.GetProperty(hornBean.role.roleId, hornBean.role.version, hornBean.role.name);
			RoleHead component2 = m_big_horn_info.m_player_icon.GetComponent<RoleHead>();
			component2.Fill(property.basicRoleInfo);
			if (property.basicRoleInfo.frameId > 0)
			{
				View.SetItemSprite(m_big_horn_info.m_player_frame, RoleHeadFrameCfg.Get(property.basicRoleInfo.frameId).frame);
			}
		}

		private IEnumerator WaitShowBigHorn()
		{
			OnCloseBigHorn();
			yield return new WaitForSeconds(cfg.Consts.WAIT_SHOW_NEW_HORN);
			UpdateBigHornMsg();
		}

		private void OnBigHornMsgHandle(SBigHorn msg)
		{
			Utils.StopConroutine(_waitToShowBigHorn);
			_waitToShowBigHorn = Utils.StartConroutine(WaitShowBigHorn());
		}

		private void UpdateRedDot(int tabType)
		{
			switch (tabType)
			{
			case 8:
				m_team_red_dot.SetActiveBetter(Singleton<ChatScMgr>.Ins.NewMsgNum(tabType) > 0);
				return;
			case 9:
				m_juntuan_red_dot.SetActiveBetter(Singleton<ChatScMgr>.Ins.NewMsgNum(tabType) > 0);
				return;
			}
			ChatBtnRedDot component = _tab2Button[tabType].GetComponent<ChatBtnRedDot>();
			if (Singleton<ChatScMgr>.Ins.NewMsgNum(tabType) <= 0)
			{
				component.m_red_dot.SetActive(false);
				return;
			}
			component.m_red_dot.SetActive(true);
			View.SetLabelText(component.txt_red_dotText, Singleton<ChatScMgr>.Ins.NewMsgNum(tabType));
		}

		private void UnlockScreen()
		{
			_isLockScreen = false;
			if (_curTabPage == 1)
			{
				Singleton<ChatScMgr>.Ins.ClearNewMsgNum(_curTabPage);
			}
			if (_curTabPage == 100)
			{
				Singleton<ChatScMgr>.Ins.ClearNewPMsgNum(_nSelectId);
				UpdateRolePanel(false);
			}
			UpdateRedDot(_curTabPage);
			btn_new_msg.SetActiveBetter(false);
		}

		private void FillWorlChatCell(GameObject go, int index)
		{
			_003CFillWorlChatCell_003Ec__AnonStorey4 _003CFillWorlChatCell_003Ec__AnonStorey = new _003CFillWorlChatCell_003Ec__AnonStorey4();
			_003CFillWorlChatCell_003Ec__AnonStorey._0024this = this;
			if (index >= _msgList.Count)
			{
				Debug.LogError("[chat]ChatPanel:FillWorldChatCell index=" + index + ",Count=" + _msgList.Count + ",curTabPage=" + _curTabPage);
				return;
			}
			if (_isLockScreen && index == _msgList.Count - 1)
			{
				UnlockScreen();
			}
			MsgBean msgBean = _msgList[index];
			OneChatCell component = go.GetComponent<OneChatCell>();
			component.m_dlb_other.gameObject.SetActiveBetter(false);
			component.m_xlb_other.gameObject.SetActiveBetter(false);
			component.m_other_mgr.gameObject.SetActiveBetter(false);
			component.m_dlb_self.gameObject.SetActiveBetter(false);
			component.m_xlb_self.gameObject.SetActiveBetter(false);
			component.m_self_mgr.gameObject.SetActiveBetter(false);
			_003CFillWorlChatCell_003Ec__AnonStorey.info = Singleton<BasicInfoScMgr>.Ins.GetProperty(msgBean.role.roleId, msgBean.role.version, msgBean.role.name);
			BasicRoleInfo basicRoleInfo = _003CFillWorlChatCell_003Ec__AnonStorey.info.basicRoleInfo;
			if (!_cell2RoleId.ContainsKey(index))
			{
				_cell2RoleId.Add(index, _003CFillWorlChatCell_003Ec__AnonStorey.info.basicRoleInfo.roleId);
			}
			bool flag = msgBean.role.roleId == Singleton<RoleMgr>.Ins.info.roleId;
			component.m_big_horn_other.SetActiveBetter(!flag && msgBean.msgType == 6);
			component.m_big_horn_self.SetActiveBetter(flag && msgBean.msgType == 6);
			component.m_small_horn_other.SetActiveBetter(!flag && msgBean.msgType == 5);
			component.m_small_horn_self.SetActiveBetter(flag && msgBean.msgType == 5);
			component.m_self.SetActiveBetter(flag);
			component.m_other.SetActiveBetter(!flag);
			if (flag)
			{
				if (RoleHeadCfg.Get(basicRoleInfo.headId) != null)
				{
					View.SetItemSprite(component.m_self_icon, RoleHeadCfg.Get(basicRoleInfo.headId).icon);
				}
				component.m_self_female.SetActiveBetter(!basicRoleInfo.sex);
				component.m_self_male.SetActiveBetter(basicRoleInfo.sex);
				View.SetLabelText(component.txt_self_nameText, basicRoleInfo.name);
				if (msgBean.msgType == 6)
				{
					component.m_dlb_self.gameObject.SetActive(true);
					View.SetLabelText(component.m_dlb_self.txt_msgText, msgBean.text);
					InlineText component2 = component.m_dlb_self.txt_msg.GetComponent<InlineText>();
					component2.enabled = false;
					component2.enabled = true;
				}
				else if (msgBean.msgType == 5)
				{
					component.m_xlb_self.gameObject.SetActive(true);
					View.SetLabelText(component.m_xlb_self.txt_msgText, msgBean.text);
					InlineText component2 = component.m_xlb_self.txt_msg.GetComponent<InlineText>();
					component2.enabled = false;
					component2.enabled = true;
				}
				else
				{
					component.m_self_mgr.gameObject.SetActive(true);
					View.SetLabelText(component.m_self_mgr.txt_msgText, msgBean.text);
					InlineText component2 = component.m_self_mgr.txt_msg.GetComponent<InlineText>();
					component2.enabled = false;
					component2.enabled = true;
				}
				if (_003CFillWorlChatCell_003Ec__AnonStorey.info.basicRoleInfo.frameId > 0)
				{
					View.SetItemSprite(component.m_self_frame, RoleHeadFrameCfg.Get(_003CFillWorlChatCell_003Ec__AnonStorey.info.basicRoleInfo.frameId).frame);
				}
			}
			else
			{
				if (RoleHeadCfg.Get(basicRoleInfo.headId) != null)
				{
					View.SetItemSprite(component.m_other_icon, RoleHeadCfg.Get(basicRoleInfo.headId).icon);
				}
				component.m_other_female.SetActiveBetter(!basicRoleInfo.sex);
				component.m_other_male.SetActiveBetter(basicRoleInfo.sex);
				View.SetLabelText(component.txt_other_nameText, basicRoleInfo.name);
				if (msgBean.msgType == 6)
				{
					component.m_dlb_other.gameObject.SetActive(true);
					View.SetLabelText(component.m_dlb_other.txt_msgText, msgBean.text);
					InlineText component2 = component.m_dlb_other.txt_msg.GetComponent<InlineText>();
					component2.enabled = false;
					component2.enabled = true;
				}
				else if (msgBean.msgType == 5)
				{
					component.m_xlb_other.gameObject.SetActive(true);
					View.SetLabelText(component.m_xlb_other.txt_msgText, msgBean.text);
					InlineText component2 = component.m_xlb_other.txt_msg.GetComponent<InlineText>();
					component2.enabled = false;
					component2.enabled = true;
				}
				else
				{
					component.m_other_mgr.gameObject.SetActive(true);
					View.SetLabelText(component.m_other_mgr.txt_msgText, msgBean.text);
					InlineText component2 = component.m_other_mgr.txt_msg.GetComponent<InlineText>();
					component2.enabled = false;
					component2.enabled = true;
				}
				try
				{
				}
				catch (Exception)
				{
				}
				if (_003CFillWorlChatCell_003Ec__AnonStorey.info.basicRoleInfo.frameId > 0)
				{
					View.SetItemSprite(component.m_other_frame, RoleHeadFrameCfg.Get(_003CFillWorlChatCell_003Ec__AnonStorey.info.basicRoleInfo.frameId).frame);
				}
			}
			ClickListener.Get(component.m_other_icon, string.Empty).onClick = _003CFillWorlChatCell_003Ec__AnonStorey._003C_003Em__0;
			UIContext.Attach(go, msgBean);
			UIContext.Attach(go, "ChatIndex", index);
		}

		private void FillRoleChatCell(GameObject go, int index)
		{
			_003CFillRoleChatCell_003Ec__AnonStorey5 _003CFillRoleChatCell_003Ec__AnonStorey = new _003CFillRoleChatCell_003Ec__AnonStorey5();
			_003CFillRoleChatCell_003Ec__AnonStorey._0024this = this;
			if (index >= _msgList.Count)
			{
				Debug.LogError("[chat]ChatPanel:FillChatInfoCell index=" + index + ",Count=" + _msgList.Count + ",curTabPage=" + _curTabPage);
				return;
			}
			if (_isLockScreen && index == _msgList.Count - 1)
			{
				UnlockScreen();
			}
			MsgBean msgBean = _msgList[index];
			GChatRoleChatCell component = go.GetComponent<GChatRoleChatCell>();
			component.m_self_mgr.gameObject.SetActiveBetter(false);
			component.m_other_mgr.gameObject.SetActiveBetter(false);
			_003CFillRoleChatCell_003Ec__AnonStorey.info = Singleton<BasicInfoScMgr>.Ins.GetProperty(msgBean.role.roleId, msgBean.role.version, msgBean.role.name);
			BasicRoleInfo basicRoleInfo = _003CFillRoleChatCell_003Ec__AnonStorey.info.basicRoleInfo;
			if (!_cell2RoleId.ContainsKey(index))
			{
				_cell2RoleId.Add(index, _003CFillRoleChatCell_003Ec__AnonStorey.info.basicRoleInfo.roleId);
			}
			bool flag = msgBean.role.roleId == Singleton<RoleMgr>.Ins.info.roleId;
			component.m_self.SetActiveBetter(flag);
			component.m_other.SetActiveBetter(!flag);
			if (flag)
			{
				if (RoleHeadCfg.Get(basicRoleInfo.headId) != null)
				{
					View.SetItemSprite(component.m_self_icon, RoleHeadCfg.Get(basicRoleInfo.headId).icon);
				}
				component.m_self_female.SetActiveBetter(!basicRoleInfo.sex);
				component.m_self_male.SetActiveBetter(basicRoleInfo.sex);
				View.SetLabelText(component.txt_self_nameText, basicRoleInfo.name);
				component.m_self_mgr.gameObject.SetActive(true);
				View.SetLabelText(component.m_self_mgr.txt_msgText, msgBean.text);
				InlineText component2 = component.m_self_mgr.txt_msg.GetComponent<InlineText>();
				component2.enabled = false;
				component2.enabled = true;
				if (_003CFillRoleChatCell_003Ec__AnonStorey.info.basicRoleInfo.frameId > 0)
				{
					View.SetItemSprite(component.m_self_frame, RoleHeadFrameCfg.Get(_003CFillRoleChatCell_003Ec__AnonStorey.info.basicRoleInfo.frameId).frame);
				}
			}
			else
			{
				if (RoleHeadCfg.Get(basicRoleInfo.headId) != null)
				{
					View.SetItemSprite(component.m_other_icon, RoleHeadCfg.Get(basicRoleInfo.headId).icon);
				}
				component.m_other_female.SetActiveBetter(!basicRoleInfo.sex);
				component.m_other_male.SetActiveBetter(basicRoleInfo.sex);
				View.SetLabelText(component.txt_other_nameText, basicRoleInfo.name);
				component.m_other_mgr.gameObject.SetActive(true);
				View.SetLabelText(component.m_other_mgr.txt_msgText, msgBean.text);
				InlineText component2 = component.m_other_mgr.txt_msg.GetComponent<InlineText>();
				component2.enabled = false;
				component2.enabled = true;
				if (_003CFillRoleChatCell_003Ec__AnonStorey.info.basicRoleInfo.frameId > 0)
				{
					View.SetItemSprite(component.m_other_frame, RoleHeadFrameCfg.Get(_003CFillRoleChatCell_003Ec__AnonStorey.info.basicRoleInfo.frameId).frame);
				}
			}
			ClickListener.Get(component.m_other_icon, string.Empty).onClick = _003CFillRoleChatCell_003Ec__AnonStorey._003C_003Em__0;
			UIContext.Attach(go, msgBean);
			UIContext.Attach(go, "ChatIndex", index);
		}

		private void FillTeamInviteCell(GameObject go, int index)
		{
			_003CFillTeamInviteCell_003Ec__AnonStorey6 _003CFillTeamInviteCell_003Ec__AnonStorey = new _003CFillTeamInviteCell_003Ec__AnonStorey6();
			_003CFillTeamInviteCell_003Ec__AnonStorey._0024this = this;
			if (index >= _msgList.Count)
			{
				Debug.LogError("[chat]ChatPanel:FillChatInfoCell index=" + index + ",Count=" + _msgList.Count + ",curTabPage=" + _curTabPage);
				return;
			}
			if (_isLockScreen && index == _msgList.Count - 1)
			{
				UnlockScreen();
			}
			MsgBean msgBean = _msgList[index];
			GChatTeamInviteCell component = go.GetComponent<GChatTeamInviteCell>();
			_003CFillTeamInviteCell_003Ec__AnonStorey.info = Singleton<BasicInfoScMgr>.Ins.GetProperty(msgBean.role.roleId, msgBean.role.version, msgBean.role.name);
			_003CFillTeamInviteCell_003Ec__AnonStorey.basicInfo = _003CFillTeamInviteCell_003Ec__AnonStorey.info.basicRoleInfo;
			if (!_cell2RoleId.ContainsKey(index))
			{
				_cell2RoleId.Add(index, _003CFillTeamInviteCell_003Ec__AnonStorey.info.basicRoleInfo.roleId);
			}
			if (RoleHeadCfg.Get(_003CFillTeamInviteCell_003Ec__AnonStorey.basicInfo.headId) != null)
			{
				View.SetItemSprite(component.m_other_icon, RoleHeadCfg.Get(_003CFillTeamInviteCell_003Ec__AnonStorey.basicInfo.headId).icon);
			}
			component.m_other_female.SetActiveBetter(!_003CFillTeamInviteCell_003Ec__AnonStorey.basicInfo.sex);
			component.m_other_male.SetActiveBetter(_003CFillTeamInviteCell_003Ec__AnonStorey.basicInfo.sex);
			View.SetLabelText(component.txt_other_nameText, _003CFillTeamInviteCell_003Ec__AnonStorey.basicInfo.name);
			View.SetLabelText(component.txt_other_msgText, msgBean.text);
			if (_003CFillTeamInviteCell_003Ec__AnonStorey.info.basicRoleInfo.frameId > 0)
			{
				View.SetItemSprite(component.m_other_frame, RoleHeadFrameCfg.Get(_003CFillTeamInviteCell_003Ec__AnonStorey.info.basicRoleInfo.frameId).frame);
			}
			ClickListener.Get(component.m_other_icon, string.Empty).onClick = _003CFillTeamInviteCell_003Ec__AnonStorey._003C_003Em__0;
			ClickListener.Get(component.btn_apply_to_troop, string.Empty).onClick = (ClickListener.Get(component.m_apply_to_troop, string.Empty).onClick = _003CFillTeamInviteCell_003Ec__AnonStorey._003C_003Em__1);
			UIContext.Attach(go, msgBean);
			UIContext.Attach(go, "ChatIndex", index);
		}

		private void UpdatePlayerBtns()
		{
			if (_selectInfo != null)
			{
				if (Singleton<TeamScMgr>.Ins.IsInMyTeam(_selectInfo.basicRoleInfo.roleId))
				{
					View.SetLabelText(txt_team_oneText, Utils.GetString(79));
				}
				else if (Singleton<TeamScMgr>.Ins.IsInTeam())
				{
					View.SetLabelText(txt_team_oneText, Utils.GetString(80));
				}
				else
				{
					View.SetLabelText(txt_team_oneText, ((_selectInfo.status & 2) <= 0) ? Utils.GetString(80) : Utils.GetString(81));
				}
				BasicRoleInfo basicRoleInfo = _selectInfo.basicRoleInfo;
				View.SetLabelText(txt_player_nameText, basicRoleInfo.name);
				View.SetLabelText(txt_level_selectText, basicRoleInfo.level);
				View.SetLabelText(txt_select_nameText, basicRoleInfo.name);
				View.SetLabelText(txt_level_selectText, basicRoleInfo.level);
			}
		}

		private void onSendMsg(GameObject go)
		{
			if (!Singleton<GuideMgr>.Ins.IsAllNoviceTaskFinish() && _curTabPage == 1)
			{
				AlertBox.Show(360);
				return;
			}
			_inp.characterLimit = cfg.Consts.MAX_CHAT_WORD_NUM;
			string text = _inp.text;
			if (text.Length == 0 || string.IsNullOrEmpty(_inp.text.Trim()))
			{
				AlertBox.Show(Utils.GetString(45));
				return;
			}
			if (_curTabPage < 100)
			{
				if (_curTabPage == 3 && !Singleton<TeamScMgr>.Ins.IsInTeam())
				{
					AlertBox.Show(75);
					return;
				}
				CPublicMsg cPublicMsg = new CPublicMsg();
				cPublicMsg.text = text;
				if (_curTabPage == 3)
				{
					List<TroopPlayer> teamates = Singleton<TeamScMgr>.Ins.GetTeamates();
					int i = 0;
					for (int count = teamates.Count; i < count; i++)
					{
						cPublicMsg.oct.push(teamates[i].role.roleId);
					}
				}
				cPublicMsg.msgType = (byte)_curTabPage;
				Client2Gs.Ins.Send(cPublicMsg);
			}
			else if (_curTabPage == 100 && _nSelectId > 0)
			{
				_cPrivateMsg.text = text;
				_cPrivateMsg.otherId = _nSelectId;
				Client2Gs.Ins.Send(_cPrivateMsg);
				MsgBean msgBean = new MsgBean();
				msgBean.role.roleId = Singleton<RoleMgr>.Ins.info.roleId;
				msgBean.role.name = Singleton<RoleMgr>.Ins.info.name;
				msgBean.role.version = int.MaxValue;
				msgBean.msgType = 100;
				msgBean.text = text;
				Singleton<ChatScMgr>.Ins.AddPrivateChat(_nSelectId, msgBean);
				_msgList.Add(msgBean);
				UpdateChatMsgPanel(true);
			}
			else
			{
				AlertBox.Show(46);
			}
			_isLockScreen = false;
			ClearInput();
			int cHAT_WORLD_MAX_TIME = cfg.Consts.CHAT_WORLD_MAX_TIME;
			if (_tab2MsgList.ContainsKey(_curTabPage))
			{
				_tab2MsgList[_curTabPage].nextTimeSendMsg = Time.realtimeSinceStartup + (float)cHAT_WORLD_MAX_TIME;
			}
			if (_curTabPage == 1)
			{
				UpdateChatInput();
			}
		}

		private void ClearInput()
		{
			_inp.text = string.Empty;
		}

		protected override void onHide(string childView = null)
		{
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Remove(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemNumerChange));
			BasicInfoScMgr ins = Singleton<BasicInfoScMgr>.Ins;
			ins.UpdateBasicInfo = (Utils.LongDelegate)Delegate.Remove(ins.UpdateBasicInfo, new Utils.LongDelegate(OnBasicInfoUpdateEvent));
			ChatEventSc.CloseBigHorn = (Utils.VoidDelegate)Delegate.Remove(ChatEventSc.CloseBigHorn, new Utils.VoidDelegate(OnCloseBigHorn));
			SPublicMsg.handler = (SPublicMsg.Handler)Delegate.Remove(SPublicMsg.handler, new SPublicMsg.Handler(OnSpublicMsg));
			SBigHorn.handler = (SBigHorn.Handler)Delegate.Remove(SBigHorn.handler, new SBigHorn.Handler(OnBigHornMsgHandle));
			SPrivateMsg.handler = (SPrivateMsg.Handler)Delegate.Remove(SPrivateMsg.handler, new SPrivateMsg.Handler(OnSPrivateMsg));
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_back = component.GameObjects[0].gameObject;
			m_roleinfo_panel = component.GameObjects[1].gameObject;
			txt_select_name = component.GameObjects[2].gameObject;
			txt_select_nameText = txt_select_name.GetComponent<Text>();
			btn_show_btns = component.GameObjects[3].gameObject;
			scp_role_chatList = component.GameObjects[4].gameObject;
			m_cell = View.AddComponentIfNotExist<GChatRoleChatCell>(component.GameObjects[5].gameObject);
			txt_online_num = component.GameObjects[6].gameObject;
			txt_online_numText = txt_online_num.GetComponent<Text>();
			scp_role = component.GameObjects[7].gameObject;
			m_norole_panel = component.GameObjects[8].gameObject;
			scp_chatlist = component.GameObjects[9].gameObject;
			scp_team_invite = component.GameObjects[10].gameObject;
			scp_juntuan_invite = component.GameObjects[11].gameObject;
			m_big_horn_info = View.AddComponentIfNotExist<GBigHornInfo>(component.GameObjects[12].gameObject);
			btn_new_msg = component.GameObjects[13].gameObject;
			txt_new_msg = component.GameObjects[14].gameObject;
			txt_new_msgText = txt_new_msg.GetComponent<Text>();
			m_cant_send = component.GameObjects[15].gameObject;
			m_can_send = component.GameObjects[16].gameObject;
			btn_emoji = component.GameObjects[17].gameObject;
			btn_small_horn = component.GameObjects[18].gameObject;
			txt_small_num = component.GameObjects[19].gameObject;
			txt_small_numText = txt_small_num.GetComponent<Text>();
			btn_big_horn = component.GameObjects[20].gameObject;
			txt_big_num = component.GameObjects[21].gameObject;
			txt_big_numText = txt_big_num.GetComponent<Text>();
			inp_chat = component.GameObjects[22].gameObject;
			btn_send = component.GameObjects[23].gameObject;
			txt_send = component.GameObjects[24].gameObject;
			txt_sendText = txt_send.GetComponent<Text>();
			txt_time = component.GameObjects[25].gameObject;
			txt_timeText = txt_time.GetComponent<Text>();
			m_emoji = component.GameObjects[26].gameObject;
			scp_emoji = component.GameObjects[27].gameObject;
			m_all_panel = component.GameObjects[28].gameObject;
			ckb_world = View.AddComponentIfNotExist<ChatBtnRedDot>(component.GameObjects[29].gameObject);
			ckb_chat = View.AddComponentIfNotExist<ChatBtnRedDot>(component.GameObjects[30].gameObject);
			ckb_team = View.AddComponentIfNotExist<ChatBtnRedDot>(component.GameObjects[31].gameObject);
			ckb_team_invite = component.GameObjects[32].gameObject;
			txt_on = component.GameObjects[33].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[34].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			m_team_red_dot = component.GameObjects[35].gameObject;
			ckb_corps = View.AddComponentIfNotExist<ChatBtnRedDot>(component.GameObjects[36].gameObject);
			ckb_system = View.AddComponentIfNotExist<ChatBtnRedDot>(component.GameObjects[37].gameObject);
			ckb_juntuan = component.GameObjects[38].gameObject;
			m_juntuan_red_dot = component.GameObjects[39].gameObject;
			m_player_btns = component.GameObjects[40].gameObject;
			m_player_btns_close = component.GameObjects[41].gameObject;
			m_player_btns_bg = component.GameObjects[42].gameObject;
			txt_player_name = component.GameObjects[43].gameObject;
			txt_player_nameText = txt_player_name.GetComponent<Text>();
			txt_level_select = component.GameObjects[44].gameObject;
			txt_level_selectText = txt_level_select.GetComponent<Text>();
			m_zhandui_flag_select = component.GameObjects[45].gameObject;
			txt_zhandui_name_select = component.GameObjects[46].gameObject;
			txt_zhandui_name_selectText = txt_zhandui_name_select.GetComponent<Text>();
			btn_basicinfo_one = component.GameObjects[47].gameObject;
			btn_care_one = component.GameObjects[48].gameObject;
			btn_chat_one = component.GameObjects[49].gameObject;
			btn_team_one = component.GameObjects[50].gameObject;
			txt_team_one = component.GameObjects[51].gameObject;
			txt_team_oneText = txt_team_one.GetComponent<Text>();
			btn_zhandui_one = component.GameObjects[52].gameObject;
			txt_zhandui_one = component.GameObjects[53].gameObject;
			txt_zhandui_oneText = txt_zhandui_one.GetComponent<Text>();
			btn_black_one = component.GameObjects[54].gameObject;
			m_cell_0 = View.AddComponentIfNotExist<GChatRoleInfoCell>(component.GameObjects[55].gameObject);
			m_cell_1 = View.AddComponentIfNotExist<OneChatCell>(component.GameObjects[56].gameObject);
			m_cell_2 = View.AddComponentIfNotExist<GChatTeamInviteCell>(component.GameObjects[57].gameObject);
			m_cell_3 = View.AddComponentIfNotExist<GChatTeamInviteCell>(component.GameObjects[58].gameObject);
			m_cell_4 = View.AddComponentIfNotExist<GChatEmojiCell>(component.GameObjects[59].gameObject);
			txt_on_0 = component.GameObjects[60].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[61].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(bool isCheck)
		{
			if (!isCheck)
			{
				_nSelectId = 0L;
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			m_emoji.SetActive(!m_emoji.activeSelf);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			UnlockScreen();
			UpdateChatMsgPanel(true);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			m_player_btns.SetActiveBetter(false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__6(GameObject go)
		{
			m_player_btns.SetActiveBetter(false);
			if (_selectInfo != null)
			{
				Singleton<RoleMgr>.Ins.GetRoleMoreInformation(_selectInfo.basicRoleInfo.roleId);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__7(GameObject go)
		{
			if (Singleton<FriendScMgr>.Ins.IsInCareList(_selectInfo.basicRoleInfo.roleId))
			{
				AlertBox.Show(72);
			}
			else
			{
				Singleton<FriendScMgr>.Ins.AddCare(_selectInfo.basicRoleInfo.roleId);
			}
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__8(GameObject go)
		{
			m_player_btns.SetActiveBetter(false);
			_curTabPage = 100;
			Singleton<ChatScMgr>.Ins.AddNewPrivateChatItem(_selectInfo.basicRoleInfo.roleId, _selectInfo.basicRoleInfo.version);
			_nSelectId = _selectInfo.basicRoleInfo.roleId;
			Singleton<ChatScMgr>.Ins.ClearNewPMsgNum(_nSelectId);
			UpdateRolePanel(false);
			if (_tab2Button.ContainsKey(_curTabPage))
			{
				RadioButton.ChooseBtn(_tab2Button[_curTabPage]);
			}
			Singleton<ChatScMgr>.Ins.ClearNewMsgNum(_curTabPage);
			UpdateChatInput();
			UpdateAllRedDots();
			OnClickTab(_curTabPage);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__9(GameObject go)
		{
			Singleton<TeamScMgr>.Ins.JoinOrInviteToTeam(_selectInfo.basicRoleInfo.roleId, _selectInfo.status);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__A(GameObject go)
		{
			if (Singleton<FriendScMgr>.Ins.IsInBlackList(_selectInfo.basicRoleInfo.roleId))
			{
				AlertBox.Show(73);
			}
			else
			{
				Singleton<FriendScMgr>.Ins.AddBlack(_selectInfo.basicRoleInfo.roleId);
			}
		}

		[CompilerGenerated]
		private void _003CLoadEmoji_003Em__B(UnityEngine.Object o)
		{
			if (o.name == "emoji")
			{
				m_emoji.gameObject.AddComponent<SpriteGraphic>().m_spriteAsset = o as SpriteAsset;
				m_cell.m_self_mgr.m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset = m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset;
				m_cell.m_other_mgr.m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset = m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset;
				m_cell_1.m_dlb_self.m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset = m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset;
				m_cell_1.m_xlb_self.m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset = m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset;
				m_cell_1.m_self_mgr.m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset = m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset;
				m_cell_1.m_dlb_other.m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset = m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset;
				m_cell_1.m_xlb_other.m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset = m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset;
				m_cell_1.m_other_mgr.m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset = m_emoji.GetComponent<SpriteGraphic>().m_spriteAsset;
				_spriteAsset = m_emoji.gameObject.GetComponent<SpriteGraphic>().m_spriteAsset;
				scp_emoji.GetComponent<UIScrollPanel>().Reset(_spriteAsset.listSpriteGroup.Count, FillEmojiCell);
				m_big_horn_info.m_emoji_horn.GetComponent<SpriteGraphic>().m_spriteAsset = _spriteAsset;
				m_cell.m_other.SetActive(true);
				m_cell.m_self.SetActive(true);
				UpdateBigHornMsg();
				UpdateChatMsgPanel(false, true);
			}
		}

		[CompilerGenerated]
		private static void _003CUpdateChatInput_003Em__C(GameObject go)
		{
			AlertBox.Show(30);
		}
	}
}
