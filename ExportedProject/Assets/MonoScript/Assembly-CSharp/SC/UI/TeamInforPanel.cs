using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;
using gs.troop.scmsg;

namespace SC.UI
{
	public class TeamInforPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillApplicationCell_003Ec__AnonStorey0
		{
			internal int index;

			internal TeamInforPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				CHandleRoleApply msg = new CHandleRoleApply
				{
					otherId = _0024this._applicationList[index].role.roleId,
					agree = true
				};
				Client2Gs.Ins.Send(msg);
				Singleton<TeamScMgr>.Ins.dicApplication.Remove(_0024this._applicationList[index].role.roleId);
				Singleton<TeamScMgr>.Ins.ApplicationList.Remove(_0024this._applicationList[index]);
				if (_0024this._applicationList.Count <= 0)
				{
					_0024this.Hide();
				}
				else
				{
					_0024this._inviteScroll.ResetNoPosClear(_0024this._applicationList.Count, _0024this.FillApplicationCell);
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillInviteCell_003Ec__AnonStorey1
		{
			internal int index;

			internal TeamInforPanel _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				CHandleTroopInvite msg = new CHandleTroopInvite
				{
					accept = true,
					troopId = _0024this._inviteInfo[index].troopId,
					otherId = _0024this._inviteInfo[index].role.roleId
				};
				Client2Gs.Ins.Send(msg);
				Singleton<TeamScMgr>.Ins.inviteInfo.Clear();
				_0024this.Hide();
			}
		}

		private UIScrollPanel _inviteScroll;

		private List<STroopInvite> _inviteInfo;

		private List<SRoleApplyTroop> _applicationList;

		private Dictionary<long, int> _id2Invite;

		private GameObject btn_close_team_invite;

		private GameObject scp_invite;

		private GTeamInviteCell m_cell;

		protected override void onInit()
		{
			base.onInit();
			_inviteScroll = scp_invite.GetComponent<UIScrollPanel>();
			_inviteInfo = Singleton<TeamScMgr>.Ins.inviteInfo;
			_applicationList = Singleton<TeamScMgr>.Ins.ApplicationList;
			_id2Invite = new Dictionary<long, int>();
			ClickListener.Get(btn_close_team_invite, string.Empty).onClick = _003ConInit_003Em__0;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			UpdateApplicationList();
		}

		private void UpdateApplicationList()
		{
			if (Singleton<TeamScMgr>.Ins.IsLeader())
			{
				_inviteScroll.Reset(_applicationList.Count, FillApplicationCell, ClearInviteCell);
			}
			else
			{
				_inviteScroll.Reset(_inviteInfo.Count, FillInviteCell, ClearInviteCell);
			}
		}

		private void ClearInviteCell(GameObject go)
		{
			_id2Invite.Remove(UIContext.Get<long>(go));
		}

		private void FillApplicationCell(GameObject go, int index)
		{
			_003CFillApplicationCell_003Ec__AnonStorey0 _003CFillApplicationCell_003Ec__AnonStorey = new _003CFillApplicationCell_003Ec__AnonStorey0();
			_003CFillApplicationCell_003Ec__AnonStorey.index = index;
			_003CFillApplicationCell_003Ec__AnonStorey._0024this = this;
			GTeamInviteCell component = go.GetComponent<GTeamInviteCell>();
			AllBasicInfo property = Singleton<BasicInfoScMgr>.Ins.GetProperty(_applicationList[_003CFillApplicationCell_003Ec__AnonStorey.index].role.roleId, _applicationList[_003CFillApplicationCell_003Ec__AnonStorey.index].role.version, _applicationList[_003CFillApplicationCell_003Ec__AnonStorey.index].role.name);
			if (property.basicRoleInfo.frameId > 0)
			{
				View.SetItemSprite(component.m_frame, RoleHeadFrameCfg.Get(property.basicRoleInfo.frameId).frame);
			}
			View.SetLabelText(component.txt_nameText, property.basicRoleInfo.name, false);
			View.SetLabelText(component.txt_roleIdText, Singleton<RoleMgr>.Ins.getIDKey(property.basicRoleInfo.roleId));
			component.m_female.SetActive(!property.basicRoleInfo.sex);
			component.m_male.SetActive(property.basicRoleInfo.sex);
			int status = property.status;
			component.m_status.SetActive(status <= 0);
			if ((status & 4) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(true);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(false);
				component.txt_in_team.SetActiveBetter(false);
			}
			else if ((status & 0x10) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(true);
				component.txt_status.SetActiveBetter(false);
				component.txt_in_team.SetActiveBetter(false);
			}
			else if ((status & 2) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(false);
				component.txt_in_team.SetActiveBetter(true);
			}
			else if ((status & 1) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(true);
				component.txt_in_team.SetActiveBetter(false);
			}
			else if (status <= 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(false);
				component.txt_in_team.SetActiveBetter(false);
			}
			ClickListener.Get(component.btn_invite, string.Empty).onClick = _003CFillApplicationCell_003Ec__AnonStorey._003C_003Em__0;
			View.SetItemSprite(component.m_icon, RoleHeadCfg.Get(property.basicRoleInfo.headId).icon);
			_id2Invite[property.basicRoleInfo.roleId] = _003CFillApplicationCell_003Ec__AnonStorey.index;
			UIContext.Attach(go, property.basicRoleInfo.roleId);
		}

		private void FillInviteCell(GameObject go, int index)
		{
			_003CFillInviteCell_003Ec__AnonStorey1 _003CFillInviteCell_003Ec__AnonStorey = new _003CFillInviteCell_003Ec__AnonStorey1();
			_003CFillInviteCell_003Ec__AnonStorey.index = index;
			_003CFillInviteCell_003Ec__AnonStorey._0024this = this;
			GTeamInviteCell component = go.GetComponent<GTeamInviteCell>();
			AllBasicInfo property = Singleton<BasicInfoScMgr>.Ins.GetProperty(_inviteInfo[_003CFillInviteCell_003Ec__AnonStorey.index].role.roleId, _inviteInfo[_003CFillInviteCell_003Ec__AnonStorey.index].role.version, _inviteInfo[_003CFillInviteCell_003Ec__AnonStorey.index].role.name);
			if (property.basicRoleInfo.frameId > 0)
			{
				View.SetItemSprite(component.m_frame, RoleHeadFrameCfg.Get(property.basicRoleInfo.frameId).frame);
			}
			View.SetLabelText(component.txt_roleIdText, Singleton<RoleMgr>.Ins.getIDKey(property.basicRoleInfo.roleId));
			View.SetLabelText(component.txt_nameText, property.basicRoleInfo.name, false);
			component.m_female.SetActive(!property.basicRoleInfo.sex);
			component.m_male.SetActive(property.basicRoleInfo.sex);
			int status = property.status;
			component.m_status.SetActive(status <= 0);
			if ((status & 4) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(true);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(false);
				component.txt_in_team.SetActiveBetter(false);
			}
			else if ((status & 0x10) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(true);
				component.txt_status.SetActiveBetter(false);
				component.txt_in_team.SetActiveBetter(false);
			}
			else if ((status & 2) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(false);
				component.txt_in_team.SetActiveBetter(true);
			}
			else if ((status & 1) > 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(true);
				component.txt_in_team.SetActiveBetter(false);
			}
			else if (status <= 0)
			{
				component.txt_invity_online_stute_battle.SetActiveBetter(false);
				component.txt_invity_online_stute_match.SetActiveBetter(false);
				component.txt_status.SetActiveBetter(false);
				component.txt_in_team.SetActiveBetter(false);
			}
			ClickListener.Get(component.btn_invite, string.Empty).onClick = _003CFillInviteCell_003Ec__AnonStorey._003C_003Em__0;
			View.SetItemSprite(component.m_icon, RoleHeadCfg.Get(property.basicRoleInfo.headId).icon);
			_id2Invite[property.basicRoleInfo.roleId] = _003CFillInviteCell_003Ec__AnonStorey.index;
			UIContext.Attach(go, property.basicRoleInfo.roleId);
		}

		private void SetCareSprite(GameObject go, long id)
		{
			go.SetActive(true);
			if (Singleton<FriendScMgr>.Ins.IsCareEachother(id))
			{
				View.SetItemSprite(go, cfg.Consts.FRIEND_EACH_ICON);
			}
			else if (Singleton<FriendScMgr>.Ins.IsInCareList(id))
			{
				View.SetItemSprite(go, cfg.Consts.FRIEND_CARE_ICON);
			}
			else if (Singleton<FriendScMgr>.Ins.IsInFansList(id))
			{
				View.SetItemSprite(go, cfg.Consts.FRIEND_FANS_ICON);
			}
			else
			{
				go.SetActive(false);
			}
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close_team_invite = component.GameObjects[0].gameObject;
			scp_invite = component.GameObjects[1].gameObject;
			m_cell = View.AddComponentIfNotExist<GTeamInviteCell>(component.GameObjects[2].gameObject);
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Singleton<TeamScMgr>.Ins.ClearInviteAndApply();
			Hide();
		}
	}
}
