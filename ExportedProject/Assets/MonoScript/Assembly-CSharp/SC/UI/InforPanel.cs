using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.friends.scmsg;
using gs.role.scmsg;
using gs.troop.scmsg;

namespace SC.UI
{
	public class InforPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillUnlockDrawItemCallBack_003Ec__AnonStorey0
		{
			internal ItemCfg itemCfg;

			internal void _003C_003Em__0(GameObject o)
			{
				if (itemCfg != null)
				{
					ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemCfg);
				}
			}
		}

		private SGetRoleMoreInformation _sGetRoleMoreInformation = new SGetRoleMoreInformation();

		private UIScrollPanel _uIScrollPanel;

		private bool _isOwn;

		private Camera _camera;

		private RenderTexture _renderTexture;

		private HeroSkin _heroSkin;

		private List<ItemCfg> _drawItemCfgs = new List<ItemCfg>();

		private GameObject btn_back;

		private GameObject m_head_icon;

		private GameObject m_head_frame;

		private GameObject txt_role_name;

		private Text txt_role_nameText;

		private GameObject txt_roleId;

		private Text txt_roleIdText;

		private GameObject txt_guild_name;

		private Text txt_guild_nameText;

		private GameObject txt_guild_job;

		private Text txt_guild_jobText;

		private GameObject txt_level;

		private Text txt_levelText;

		private GameObject txt_all_time;

		private Text txt_all_timeText;

		private GameObject scp_unlock_item;

		private GInforPanelUnlockItem m_cell;

		private GameObject m_arrow_right;

		private GameObject m_arrow_left;

		private GameObject m_other;

		private GameObject btn_chat;

		private GameObject btn_black;

		private GameObject btn_care;

		private GameObject btn_cancel_care;

		private GameObject btn_activite_team;

		private GameObject btn_apply_team;

		private GameObject m_me;

		private GameObject btn_individuation;

		private GameObject btn_rename;

		private GameObject m_model_root;

		private GameObject m_role_bg;

		private GameObject m_hero_tex;

		private GameObject m_model_role;

		private GameObject m_camera;

		private GameObject m_model;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache1;

		protected override void onInit()
		{
			m_arrow_left.SetActiveBetter(false);
			m_arrow_right.SetActiveBetter(false);
			_camera = m_camera.GetComponent<Camera>();
			_renderTexture = HeroTools.CreatShowHero(_camera, m_hero_tex);
			_drawItemCfgs.Clear();
			foreach (ItemCfg all in ItemCfg.GetAllList())
			{
				if (all.type == 79)
				{
					DrawingCfg drawingCfg = DrawingCfg.Get(all.id);
					if (drawingCfg != null)
					{
						_drawItemCfgs.Add(all);
					}
				}
			}
			_heroSkin = new HeroSkin(m_model, Singleton<RoleMgr>.Ins.info.sex);
			_uIScrollPanel = scp_unlock_item.GetComponent<UIScrollPanel>();
			ClickListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_chat, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_black, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_care, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_cancel_care, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener.Get(btn_activite_team, string.Empty).onClick = _003ConInit_003Em__5;
			ClickListener.Get(btn_apply_team, string.Empty).onClick = _003ConInit_003Em__6;
			ClickListener clickListener = ClickListener.Get(btn_individuation, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003ConInit_003Em__7;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			ClickListener clickListener2 = ClickListener.Get(btn_rename, string.Empty);
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003ConInit_003Em__8;
			}
			clickListener2.onClick = _003C_003Ef__am_0024cache1;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			SCare.handler = (SCare.Handler)Delegate.Combine(SCare.handler, new SCare.Handler(OnSCare));
			SCareCancel.handler = (SCareCancel.Handler)Delegate.Combine(SCareCancel.handler, new SCareCancel.Handler(OnSCareCancel));
			SBlack.handler = (SBlack.Handler)Delegate.Combine(SBlack.handler, new SBlack.Handler(OnSBlack));
			SBlackDelete.handler = (SBlackDelete.Handler)Delegate.Combine(SBlackDelete.handler, new SBlackDelete.Handler(OnSBlackDelete));
			BasicInfoScMgr ins = Singleton<BasicInfoScMgr>.Ins;
			ins.UpdateBasicInfo = (Utils.LongDelegate)Delegate.Combine(ins.UpdateBasicInfo, new Utils.LongDelegate(UpdateBasicInfoEvent));
			SErrorTroop.handler = (SErrorTroop.Handler)Delegate.Combine(SErrorTroop.handler, new SErrorTroop.Handler(OnSErrorTroop));
			SInviteRole.handler = (SInviteRole.Handler)Delegate.Combine(SInviteRole.handler, new SInviteRole.Handler(OnSInviteRole));
			SChangeName.handler = (SChangeName.Handler)Delegate.Combine(SChangeName.handler, new SChangeName.Handler(OnSChangeName));
			SSetFrameId.handler = (SSetFrameId.Handler)Delegate.Combine(SSetFrameId.handler, new SSetFrameId.Handler(OnSetFrameId));
			SSetHeadId.handler = (SSetHeadId.Handler)Delegate.Combine(SSetHeadId.handler, new SSetHeadId.Handler(OnSSetHeadId));
			if (param != null)
			{
				_sGetRoleMoreInformation = param as SGetRoleMoreInformation;
				_isOwn = _sGetRoleMoreInformation.roleId == Singleton<RoleMgr>.Ins.info.roleId;
			}
			m_other.SetActiveBetter(!_isOwn);
			m_me.SetActiveBetter(_isOwn);
			RefreshBtns();
			SetBasicInfo();
			SetUnlockDraws();
			ShowHeroSkinMode();
		}

		private void RefreshBtns()
		{
			if (!_isOwn)
			{
				RefreshFriendBtns();
				RefreshTeamBtns();
			}
		}

		private void RefreshFriendBtns()
		{
			long roleId = _sGetRoleMoreInformation.roleId;
			btn_black.SetActiveBetter(!Singleton<FriendScMgr>.Ins.IsInBlackList(roleId));
			btn_care.SetActiveBetter(!Singleton<FriendScMgr>.Ins.IsInCareList(roleId));
			btn_cancel_care.SetActiveBetter(Singleton<FriendScMgr>.Ins.IsInCareList(roleId));
		}

		private void RefreshTeamBtns()
		{
			long roleId = _sGetRoleMoreInformation.roleId;
			if (Singleton<TeamScMgr>.Ins.IsInMyTeam(roleId))
			{
				btn_activite_team.SetActiveBetter(false);
				btn_apply_team.SetActiveBetter(false);
			}
			else if (Singleton<TeamScMgr>.Ins.IsInTeam())
			{
				btn_activite_team.SetActiveBetter(true);
				btn_apply_team.SetActiveBetter(false);
			}
			else if ((Singleton<BasicInfoScMgr>.Ins.GetStatus(roleId) & 2) > 0)
			{
				btn_activite_team.SetActiveBetter(false);
				btn_apply_team.SetActiveBetter(true);
			}
		}

		protected override void onHide(string childView = null)
		{
			SCare.handler = (SCare.Handler)Delegate.Remove(SCare.handler, new SCare.Handler(OnSCare));
			SCareCancel.handler = (SCareCancel.Handler)Delegate.Remove(SCareCancel.handler, new SCareCancel.Handler(OnSCareCancel));
			SBlack.handler = (SBlack.Handler)Delegate.Remove(SBlack.handler, new SBlack.Handler(OnSBlack));
			SBlackDelete.handler = (SBlackDelete.Handler)Delegate.Remove(SBlackDelete.handler, new SBlackDelete.Handler(OnSBlackDelete));
			BasicInfoScMgr ins = Singleton<BasicInfoScMgr>.Ins;
			ins.UpdateBasicInfo = (Utils.LongDelegate)Delegate.Remove(ins.UpdateBasicInfo, new Utils.LongDelegate(UpdateBasicInfoEvent));
			SErrorTroop.handler = (SErrorTroop.Handler)Delegate.Remove(SErrorTroop.handler, new SErrorTroop.Handler(OnSErrorTroop));
			SInviteRole.handler = (SInviteRole.Handler)Delegate.Remove(SInviteRole.handler, new SInviteRole.Handler(OnSInviteRole));
			SChangeName.handler = (SChangeName.Handler)Delegate.Remove(SChangeName.handler, new SChangeName.Handler(OnSChangeName));
			SSetFrameId.handler = (SSetFrameId.Handler)Delegate.Remove(SSetFrameId.handler, new SSetFrameId.Handler(OnSetFrameId));
			SSetHeadId.handler = (SSetHeadId.Handler)Delegate.Remove(SSetHeadId.handler, new SSetHeadId.Handler(OnSSetHeadId));
		}

		private void OnSSetHeadId(SSetHeadId msg)
		{
			_sGetRoleMoreInformation.headId = msg.headId;
			SetHead();
		}

		private void OnSetFrameId(SSetFrameId msg)
		{
			_sGetRoleMoreInformation.frameId = msg.frameId;
			SetHeadFrame();
		}

		private void OnSChangeName(SChangeName msg)
		{
			_sGetRoleMoreInformation.roleName = msg.name;
			SetName();
		}

		private void OnSInviteRole(SInviteRole msg)
		{
			AlertBox.Show(Utils.GetString(278));
		}

		private void OnSErrorTroop(SErrorTroop msg)
		{
			int code = msg.code;
			if (code == 5 || code == 10)
			{
				RefreshTeamBtns();
			}
		}

		private void OnSCare(SCare msg)
		{
			RefreshFriendBtns();
		}

		private void OnSCareCancel(SCareCancel msg)
		{
			RefreshFriendBtns();
		}

		private void OnSBlack(SBlack msg)
		{
			RefreshFriendBtns();
		}

		private void OnSBlackDelete(SBlackDelete msg)
		{
			RefreshFriendBtns();
		}

		private void UpdateBasicInfoEvent(long arg)
		{
			RefreshTeamBtns();
		}

		protected override void onDestroy()
		{
			_camera.targetTexture = null;
			UnityEngine.Object.DestroyImmediate(_renderTexture, true);
			_heroSkin.Clear();
		}

		private void SetBasicInfo()
		{
			View.SetLabelText(txt_levelText, _sGetRoleMoreInformation.level);
			SetHead();
			SetHeadFrame();
			SetName();
			SetGuildInfo();
			View.SetLabelText(txt_all_timeText, Utils.GetTimeString(_sGetRoleMoreInformation.totalTime));
			string iDKey = Singleton<RoleMgr>.Ins.getIDKey(_sGetRoleMoreInformation.roleId);
			View.SetLabelText(txt_roleIdText, iDKey);
		}

		private void SetName()
		{
			View.SetLabelText(txt_role_nameText, _sGetRoleMoreInformation.roleName);
		}

		private void SetHead()
		{
			if (_sGetRoleMoreInformation.headId < 1)
			{
				_sGetRoleMoreInformation.headId = 1;
			}
			RoleHeadCfg roleHeadCfg = RoleHeadCfg.Get(_sGetRoleMoreInformation.headId);
			if (roleHeadCfg != null)
			{
				View.SetItemSprite(m_head_icon, roleHeadCfg.icon);
			}
		}

		private void SetHeadFrame()
		{
			RoleHeadFrameCfg roleHeadFrameCfg = RoleHeadFrameCfg.Get(_sGetRoleMoreInformation.frameId);
			if (roleHeadFrameCfg != null)
			{
				m_head_frame.SetActiveBetter(true);
				View.SetItemSprite(m_head_frame, roleHeadFrameCfg.frame);
			}
			else
			{
				m_head_frame.SetActiveBetter(false);
			}
		}

		private void SetGuildInfo()
		{
		}

		private void SetUnlockDraws()
		{
			_uIScrollPanel.Clear();
			_uIScrollPanel.Reset(_drawItemCfgs.Count, FillUnlockDrawItemCallBack);
		}

		private void FillUnlockDrawItemCallBack(GameObject go, int index)
		{
			_003CFillUnlockDrawItemCallBack_003Ec__AnonStorey0 _003CFillUnlockDrawItemCallBack_003Ec__AnonStorey = new _003CFillUnlockDrawItemCallBack_003Ec__AnonStorey0();
			GInforPanelUnlockItem component = go.GetComponent<GInforPanelUnlockItem>();
			ItemCfg itemCfg = _drawItemCfgs[index];
			DrawingCfg drawingCfg = DrawingCfg.Get(itemCfg.id);
			component.m_lock_item.SetActiveBetter(!_sGetRoleMoreInformation.learnedDrawings.Contains(itemCfg.id));
			if (drawingCfg != null)
			{
				_003CFillUnlockDrawItemCallBack_003Ec__AnonStorey.itemCfg = ItemCfg.Get(drawingCfg.targetItemId);
				if (_003CFillUnlockDrawItemCallBack_003Ec__AnonStorey.itemCfg != null)
				{
					View.SetItemSprite(component.m_lock_icon, _003CFillUnlockDrawItemCallBack_003Ec__AnonStorey.itemCfg.icon);
					View.SetLabelText(component.txt_lock_nameText, _003CFillUnlockDrawItemCallBack_003Ec__AnonStorey.itemCfg.name);
				}
				ClickListener.Get(go, string.Empty).onClick = _003CFillUnlockDrawItemCallBack_003Ec__AnonStorey._003C_003Em__0;
			}
		}

		public void ShowHeroSkinMode()
		{
			_heroSkin.SetSex(_sGetRoleMoreInformation.sex);
			_heroSkin.LoadBasicHero(_sGetRoleMoreInformation.roleShow, _sGetRoleMoreInformation.roleShow, "MainPanel.kongshou_zhan_idle01");
		}

		private void OnEnable()
		{
			if (_heroSkin != null)
			{
				_heroSkin.PlayRoleAni("MainPanel.kongshou_zhan_idle01", string.Empty);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_back = component.GameObjects[0].gameObject;
			m_head_icon = component.GameObjects[1].gameObject;
			m_head_frame = component.GameObjects[2].gameObject;
			txt_role_name = component.GameObjects[3].gameObject;
			txt_role_nameText = txt_role_name.GetComponent<Text>();
			txt_roleId = component.GameObjects[4].gameObject;
			txt_roleIdText = txt_roleId.GetComponent<Text>();
			txt_guild_name = component.GameObjects[5].gameObject;
			txt_guild_nameText = txt_guild_name.GetComponent<Text>();
			txt_guild_job = component.GameObjects[6].gameObject;
			txt_guild_jobText = txt_guild_job.GetComponent<Text>();
			txt_level = component.GameObjects[7].gameObject;
			txt_levelText = txt_level.GetComponent<Text>();
			txt_all_time = component.GameObjects[8].gameObject;
			txt_all_timeText = txt_all_time.GetComponent<Text>();
			scp_unlock_item = component.GameObjects[9].gameObject;
			m_cell = View.AddComponentIfNotExist<GInforPanelUnlockItem>(component.GameObjects[10].gameObject);
			m_arrow_right = component.GameObjects[11].gameObject;
			m_arrow_left = component.GameObjects[12].gameObject;
			m_other = component.GameObjects[13].gameObject;
			btn_chat = component.GameObjects[14].gameObject;
			btn_black = component.GameObjects[15].gameObject;
			btn_care = component.GameObjects[16].gameObject;
			btn_cancel_care = component.GameObjects[17].gameObject;
			btn_activite_team = component.GameObjects[18].gameObject;
			btn_apply_team = component.GameObjects[19].gameObject;
			m_me = component.GameObjects[20].gameObject;
			btn_individuation = component.GameObjects[21].gameObject;
			btn_rename = component.GameObjects[22].gameObject;
			m_model_root = component.GameObjects[23].gameObject;
			m_role_bg = component.GameObjects[24].gameObject;
			m_hero_tex = component.GameObjects[25].gameObject;
			m_model_role = component.GameObjects[26].gameObject;
			m_camera = component.GameObjects[27].gameObject;
			m_model = component.GameObjects[28].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			RoleVersion param = new RoleVersion
			{
				roleId = _sGetRoleMoreInformation.roleId,
				version = 0
			};
			ViewMgr.Ins.ShowView<ChatPanel>(param);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			Singleton<FriendScMgr>.Ins.AddBlack(_sGetRoleMoreInformation.roleId);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			Singleton<FriendScMgr>.Ins.AddCare(_sGetRoleMoreInformation.roleId);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			Singleton<FriendScMgr>.Ins.RemoveCare(_sGetRoleMoreInformation.roleId);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			Singleton<TeamScMgr>.Ins.InviteToJoin(_sGetRoleMoreInformation.roleId);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__6(GameObject go)
		{
			Singleton<TeamScMgr>.Ins.SendApplyToTeam(_sGetRoleMoreInformation.roleId);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__7(GameObject go)
		{
			ViewMgr.Ins.ShowView<InforSelfdomPanel>(null, false);
		}

		[CompilerGenerated]
		private static void _003ConInit_003Em__8(GameObject go)
		{
			ViewMgr.Ins.ShowView<InforRenamePanel>(null, false);
		}
	}
}
