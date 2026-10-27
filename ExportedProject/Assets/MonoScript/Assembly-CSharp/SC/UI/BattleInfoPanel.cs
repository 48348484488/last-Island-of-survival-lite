using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using gs.friends.scmsg;
using gs.role.scmsg;
using gs.troop.scmsg;

namespace SC.UI
{
	public class BattleInfoPanel : View
	{
		private long _roleId;

		private GameObject btn_info;

		private GameObject btn_care;

		private GameObject btn_acticite_team;

		private GameObject btn_apply_team;

		private GameObject btn_chat;

		private GameObject m_bg;

		protected override void onInit()
		{
			ClickListener.Get(m_bg, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_chat, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_care, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_acticite_team, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_apply_team, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener.Get(btn_info, string.Empty).onClick = _003ConInit_003Em__5;
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
			_roleId = (long)param;
			RefreshBtns();
		}

		private void RefreshBtns()
		{
			RefreshFriendBtns();
			RefreshTeamBtns();
		}

		private void RefreshFriendBtns()
		{
			btn_care.SetActiveBetter(!Singleton<FriendScMgr>.Ins.IsInCareList(_roleId));
		}

		private void RefreshTeamBtns()
		{
			long roleId = _roleId;
			if (Singleton<TeamScMgr>.Ins.IsInMyTeam(roleId))
			{
				btn_acticite_team.SetActiveBetter(false);
				btn_apply_team.SetActiveBetter(false);
			}
			else if (Singleton<TeamScMgr>.Ins.IsInTeam())
			{
				btn_acticite_team.SetActiveBetter(true);
				btn_apply_team.SetActiveBetter(false);
			}
			else if ((Singleton<BasicInfoScMgr>.Ins.GetStatus(roleId) & 2) > 0)
			{
				btn_acticite_team.SetActiveBetter(false);
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

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_info = component.GameObjects[0].gameObject;
			btn_care = component.GameObjects[1].gameObject;
			btn_acticite_team = component.GameObjects[2].gameObject;
			btn_apply_team = component.GameObjects[3].gameObject;
			btn_chat = component.GameObjects[4].gameObject;
			m_bg = component.GameObjects[5].gameObject;
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
				roleId = _roleId,
				version = 0
			};
			ViewMgr.Ins.ShowView<ChatPanel>(param);
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			Singleton<FriendScMgr>.Ins.AddCare(_roleId);
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			Singleton<TeamScMgr>.Ins.InviteToJoin(_roleId);
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			Singleton<TeamScMgr>.Ins.SendApplyToTeam(_roleId);
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			Singleton<RoleMgr>.Ins.GetRoleMoreInformation(_roleId);
			Hide();
		}
	}
}
