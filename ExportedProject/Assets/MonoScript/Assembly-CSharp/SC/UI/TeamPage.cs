using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using cfg;
using gs.battle.scmsg;
using gs.role.scmsg;
using gs.troop.scmsg;

namespace SC.UI
{
	public class TeamPage : MonoBehaviour, ITeamTaskPage
	{
		[CompilerGenerated]
		private sealed class _003CFillTeammateCell_003Ec__AnonStorey0
		{
			internal long roleId;

			internal GLeftTeamInfo cell;

			internal TroopPlayer info;

			internal TeamPage _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (_0024this.m_btns.activeSelf && _0024this._selectRoleId == roleId)
				{
					_0024this._selectRoleId = 0L;
					_0024this.m_btns.SetActiveBetter(false);
					Utils.TriggerEvent(TeamTaskEvent.RefreshShowBtnsDelegate);
					if (TeamTaskEvent.TeamBtnsActiveChangeDelegate != null)
					{
						TeamTaskEvent.TeamBtnsActiveChangeDelegate(false);
					}
					_0024this.m_bg.SetActiveBetter(false);
					cell.m_select.SetActiveBetter(false);
					cell.m_notselect.SetActiveBetter(true);
					_0024this._lastSelect = null;
					return;
				}
				_0024this._selectRoleId = roleId;
				_0024this.m_btns.SetActiveBetter(true);
				Utils.TriggerEvent(TeamTaskEvent.RefreshShowBtnsDelegate);
				if (TeamTaskEvent.TeamBtnsActiveChangeDelegate != null)
				{
					TeamTaskEvent.TeamBtnsActiveChangeDelegate(true);
				}
				_0024this.m_bg.SetActiveBetter(true);
				_0024this.ResetBtnsShow(info.role.roleId);
				cell.m_select.SetActiveBetter(true);
				cell.m_notselect.SetActiveBetter(false);
				if (_0024this._lastSelect != null)
				{
					_0024this._lastSelect.m_select.SetActiveBetter(false);
					_0024this._lastSelect.m_notselect.SetActiveBetter(true);
				}
				_0024this._lastSelect = cell;
			}
		}

		private long _selectRoleId;

		private RectTransform _pageRect;

		private Slider _shouYiSlider;

		private Image _sliderImageCom;

		private Color _sliderNormalColor;

		private Color _sliderMaxColor;

		private GLeftTeamInfo _lastSelect;

		private readonly Dictionary<int, Image> _dicIndex2HpImage = new Dictionary<int, Image>();

		public GameObject btn_allow;

		public GameObject btn_care;

		public GameObject btn_create;

		public GameObject btn_exit;

		public GameObject btn_infor;

		public GameObject btn_kick;

		public GameObject btn_trans_leader;

		public GameObject m_bg;

		public GameObject m_btns;

		public GameObject m_can_get;

		public GameObject m_keLingQu;

		public GameObject m_kongXiangZi;

		public GameObject m_max_get;

		public GameObject m_progress_slider;

		public GameObject m_shouyi;

		public GameObject m_team_page;

		public GameObject m_teammate;

		public List<GLeftTeamInfo> m_TeamPlayerListlist = new List<GLeftTeamInfo>();

		public GameObject[] m_TeamPlayerList;

		public GameObject m_TeamPlayerListObj;

		public GameObject txt_progress;

		public Text txt_progressText;

		public object context;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache1;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache2;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache3;

		[CompilerGenerated]
		private static Utils.VoidDelegate _003C_003Ef__am_0024cache4;

		public GameObject ThisGo
		{
			get
			{
				return base.gameObject;
			}
		}

		public void OnInit()
		{
			m_btns.SetActiveBetter(false);
			Utils.TriggerEvent(TeamTaskEvent.RefreshShowBtnsDelegate);
			m_bg.SetActiveBetter(false);
			_pageRect = m_team_page.GetComponent<RectTransform>();
			_shouYiSlider = m_progress_slider.GetComponent<Slider>();
			_shouYiSlider.maxValue = cfg.Consts.TEAM_EARNING_MAX_VALUE;
			_sliderImageCom = _shouYiSlider.fillRect.GetComponent<Image>();
			_sliderNormalColor = _sliderImageCom.color;
			ColorUtility.TryParseHtmlString("C1CC48", out _sliderMaxColor);
			ClickListener clickListener = ClickListener.Get(btn_create, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnInit_003Em__0;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			ClickListener clickListener2 = ClickListener.Get(btn_exit, string.Empty);
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003COnInit_003Em__1;
			}
			clickListener2.onClick = _003C_003Ef__am_0024cache1;
			ClickListener.Get(btn_kick, string.Empty).onClick = _003COnInit_003Em__2;
			ClickListener.Get(btn_trans_leader, string.Empty).onClick = _003COnInit_003Em__3;
			ClickListener.Get(btn_allow, string.Empty).onClick = _003COnInit_003Em__4;
			ClickListener.Get(btn_care, string.Empty).onClick = _003COnInit_003Em__5;
			ClickListener.Get(btn_infor, string.Empty).onClick = _003COnInit_003Em__6;
			ClickListener.Get(m_bg, string.Empty).onClick = _003COnInit_003Em__7;
			ClickListener clickListener3 = ClickListener.Get(m_shouyi, string.Empty);
			if (_003C_003Ef__am_0024cache2 == null)
			{
				_003C_003Ef__am_0024cache2 = _003COnInit_003Em__8;
			}
			clickListener3.onClick = _003C_003Ef__am_0024cache2;
		}

		public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
		{
			List<RaycastResult> list = new List<RaycastResult>();
			EventSystem.current.RaycastAll(data, list);
			GameObject gameObject = data.pointerCurrentRaycast.gameObject;
			for (int i = 0; i < list.Count; i++)
			{
				if (gameObject != list[i].gameObject)
				{
					ClickListener componentInParent = list[i].gameObject.GetComponentInParent<ClickListener>();
					if (componentInParent != null)
					{
						componentInParent.onClick(componentInParent.gameObject);
					}
					break;
				}
			}
		}

		public void OnShow(object param)
		{
			UpdateTeammatesInfo();
			m_teammate.SetActiveBetter(true);
			Vector2 vector = default(Vector2);
			vector.x = 0f;
			vector.y = _pageRect.anchoredPosition.y;
			Vector2 anchoredPosition = vector;
			_pageRect.anchoredPosition = anchoredPosition;
			if (Singleton<TeamScMgr>.Ins.MySTeamEarningChange != null)
			{
				SetLingQu(Singleton<TeamScMgr>.Ins.MySTeamEarningChange.earningValue, Singleton<TeamScMgr>.Ins.MySTeamEarningChange.isCanGet);
			}
			else
			{
				SetLingQu(0, false);
			}
			SetShouYiDianShow();
			STroopInfo.handler = (STroopInfo.Handler)Delegate.Combine(STroopInfo.handler, new STroopInfo.Handler(OnSTroopInfo));
			TeamScEvent.LeaveTeamEvent = (Utils.VoidDelegate)Delegate.Combine(TeamScEvent.LeaveTeamEvent, new Utils.VoidDelegate(OnLeaveTeam));
			SOperateCMD.handler = (SOperateCMD.Handler)Delegate.Combine(SOperateCMD.handler, new SOperateCMD.Handler(OnSOperateCMD));
			TeamScEvent.TeammateChangeNameAction = (Action)Delegate.Combine(TeamScEvent.TeammateChangeNameAction, new Action(UpdateTeammatesInfo));
			SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Combine(SPlayerDie.handler, new SPlayerDie.Handler(OnSPlayerDie));
			SMakeMark.handler = (SMakeMark.Handler)Delegate.Combine(SMakeMark.handler, new SMakeMark.Handler(OnSMakeMark));
			SRemoveMark.handler = (SRemoveMark.Handler)Delegate.Combine(SRemoveMark.handler, new SRemoveMark.Handler(OnSRemoveMark));
			SGetInVehicle.handler = (SGetInVehicle.Handler)Delegate.Combine(SGetInVehicle.handler, new SGetInVehicle.Handler(OnSGetInVehicle));
			SGetOutVehicle.handler = (SGetOutVehicle.Handler)Delegate.Combine(SGetOutVehicle.handler, new SGetOutVehicle.Handler(OnSGetOutVehicle));
			SHpChange.handler = (SHpChange.Handler)Delegate.Combine(SHpChange.handler, new SHpChange.Handler(OnSHpChange));
			SRebirth.handler = (SRebirth.Handler)Delegate.Combine(SRebirth.handler, new SRebirth.Handler(OnSRebirth));
			STeamateInfo.handler = (STeamateInfo.Handler)Delegate.Combine(STeamateInfo.handler, new STeamateInfo.Handler(OnSTeamateInfo));
			TeamScEvent.UpdateOnlineStatusEvent = (Action<long, bool>)Delegate.Combine(TeamScEvent.UpdateOnlineStatusEvent, new Action<long, bool>(UpdateOnlineStatus));
			STeamEarningChange.handler = (STeamEarningChange.Handler)Delegate.Combine(STeamEarningChange.handler, new STeamEarningChange.Handler(OnSTeamEarningChange));
			TeamScEvent.TeammateCutDownAction = (Action<long>)Delegate.Combine(TeamScEvent.TeammateCutDownAction, new Action<long>(RefreshShouYiDian));
		}

		private void SetShouYiDianShow()
		{
			bool flag = Singleton<TeamScMgr>.Ins.MySTeamEarningChange != null && Singleton<TeamScMgr>.Ins.MySTeamEarningChange.getCount >= cfg.Consts.TEAM_EARNINGREWARD_MAX_GETCOUNT;
			m_can_get.SetActiveBetter(!flag);
			m_max_get.SetActiveBetter(flag);
		}

		private void RefreshShouYiDian(long otherId)
		{
			if (otherId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				SetLingQu(0, false);
			}
		}

		private void OnSTeamEarningChange(STeamEarningChange msg)
		{
			SetLingQu(msg.earningValue, msg.isCanGet);
			SetShouYiDianShow();
		}

		private void SetLingQu(int progress, bool canGet)
		{
			if (canGet)
			{
				_shouYiSlider.value = cfg.Consts.TEAM_EARNING_MAX_VALUE;
				_sliderImageCom.color = _sliderMaxColor;
			}
			else
			{
				_shouYiSlider.value = progress;
				_sliderImageCom.color = _sliderNormalColor;
			}
			View.SetLabelText(txt_progressText, Utils.GetString(9, _shouYiSlider.value, _shouYiSlider.maxValue));
			m_keLingQu.SetActiveBetter(canGet);
			m_kongXiangZi.SetActiveBetter(!canGet);
		}

		private void OnSTeamateInfo(STeamateInfo msg)
		{
			UpdateTeammatesInfo();
		}

		private void OnSRebirth(SRebirth msg)
		{
			StatusChange(msg.playerInfo.roleId);
		}

		private void OnSHpChange(SHpChange msg)
		{
			StatusChange(msg.roleId);
		}

		private void UpdateOnlineStatus(long roleId, bool isOnline)
		{
			RefreshTeammateInfo();
		}

		private void OnSGetOutVehicle(SGetOutVehicle msg)
		{
			StatusChange(msg.roleId);
		}

		private void OnSGetInVehicle(SGetInVehicle msg)
		{
			StatusChange(msg.roleId);
		}

		private void OnSRemoveMark(SRemoveMark msg)
		{
			StatusChange(msg.roleId);
		}

		private void OnSMakeMark(SMakeMark msg)
		{
			StatusChange(msg.roleId);
		}

		private void OnSPlayerDie(SPlayerDie msg)
		{
			StatusChange(msg.roleId);
		}

		private void OnSOperateCMD(SOperateCMD msg)
		{
			UpdateTeammatesInfo();
		}

		private void OnLeaveTeam()
		{
			btn_create.SetActiveBetter(true);
			btn_exit.SetActiveBetter(false);
			m_btns.SetActiveBetter(false);
			Utils.TriggerEvent(TeamTaskEvent.RefreshShowBtnsDelegate);
			if (TeamTaskEvent.TeamBtnsActiveChangeDelegate != null)
			{
				TeamTaskEvent.TeamBtnsActiveChangeDelegate(false);
			}
			m_bg.SetActiveBetter(false);
			ResetLastSelect();
			UpdateTeammatesInfo();
		}

		private void ResetLastSelect()
		{
			if (_lastSelect != null)
			{
				_lastSelect.m_notselect.SetActiveBetter(true);
				_lastSelect.m_select.SetActiveBetter(false);
				_lastSelect = null;
			}
		}

		private void OnSTroopInfo(STroopInfo msg)
		{
			btn_create.SetActiveBetter(false);
			btn_exit.SetActiveBetter(true);
			UpdateTeammatesInfo();
		}

		public void OnHide()
		{
			STroopInfo.handler = (STroopInfo.Handler)Delegate.Remove(STroopInfo.handler, new STroopInfo.Handler(OnSTroopInfo));
			TeamScEvent.LeaveTeamEvent = (Utils.VoidDelegate)Delegate.Remove(TeamScEvent.LeaveTeamEvent, new Utils.VoidDelegate(OnLeaveTeam));
			SOperateCMD.handler = (SOperateCMD.Handler)Delegate.Remove(SOperateCMD.handler, new SOperateCMD.Handler(OnSOperateCMD));
			TeamScEvent.TeammateChangeNameAction = (Action)Delegate.Remove(TeamScEvent.TeammateChangeNameAction, new Action(UpdateTeammatesInfo));
			SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Remove(SPlayerDie.handler, new SPlayerDie.Handler(OnSPlayerDie));
			SMakeMark.handler = (SMakeMark.Handler)Delegate.Remove(SMakeMark.handler, new SMakeMark.Handler(OnSMakeMark));
			SRemoveMark.handler = (SRemoveMark.Handler)Delegate.Remove(SRemoveMark.handler, new SRemoveMark.Handler(OnSRemoveMark));
			SGetInVehicle.handler = (SGetInVehicle.Handler)Delegate.Remove(SGetInVehicle.handler, new SGetInVehicle.Handler(OnSGetInVehicle));
			SGetOutVehicle.handler = (SGetOutVehicle.Handler)Delegate.Remove(SGetOutVehicle.handler, new SGetOutVehicle.Handler(OnSGetOutVehicle));
			SHpChange.handler = (SHpChange.Handler)Delegate.Remove(SHpChange.handler, new SHpChange.Handler(OnSHpChange));
			SRebirth.handler = (SRebirth.Handler)Delegate.Remove(SRebirth.handler, new SRebirth.Handler(OnSRebirth));
			STeamateInfo.handler = (STeamateInfo.Handler)Delegate.Remove(STeamateInfo.handler, new STeamateInfo.Handler(OnSTeamateInfo));
			TeamScEvent.UpdateOnlineStatusEvent = (Action<long, bool>)Delegate.Remove(TeamScEvent.UpdateOnlineStatusEvent, new Action<long, bool>(UpdateOnlineStatus));
			STeamEarningChange.handler = (STeamEarningChange.Handler)Delegate.Remove(STeamEarningChange.handler, new STeamEarningChange.Handler(OnSTeamEarningChange));
			TeamScEvent.TeammateCutDownAction = (Action<long>)Delegate.Remove(TeamScEvent.TeammateCutDownAction, new Action<long>(RefreshShouYiDian));
		}

		private void StatusChange(long roleId)
		{
			if (Singleton<TeamScMgr>.Ins.IsInMyTeam(roleId))
			{
				RefreshTeammateInfo();
			}
		}

		private void RefreshTeammateInfo()
		{
			bool flag = Singleton<TeamScMgr>.Ins.IsInTeam();
			btn_create.SetActiveBetter(!flag);
			btn_exit.SetActiveBetter(flag);
			if (flag)
			{
				int i = 0;
				for (int num = m_TeamPlayerList.Length; i < num; i++)
				{
					m_TeamPlayerListlist[i].m_notselect.SetActiveBetter(true);
					m_TeamPlayerListlist[i].m_select.SetActiveBetter(false);
				}
			}
			int j = 0;
			for (int num2 = m_TeamPlayerList.Length; j < num2; j++)
			{
				RefreshTeammateInfo(m_TeamPlayerListlist[j], j);
			}
		}

		private void RefreshTeammateInfo(GLeftTeamInfo cell, int index)
		{
			List<TroopPlayer> teamates = Singleton<TeamScMgr>.Ins.GetTeamates();
			if (teamates.Count <= index)
			{
				cell.m_teamate.SetActiveBetter(false);
				cell.m_add.SetActiveBetter(true);
				return;
			}
			cell.m_teamate.SetActiveBetter(true);
			cell.m_add.SetActiveBetter(false);
			TroopPlayer troopPlayer = teamates[index];
			long roleId = troopPlayer.role.roleId;
			bool flag = Singleton<TeamateMgr>.Ins.IsDead(roleId);
			bool flag2 = !Singleton<TeamScMgr>.Ins.IsMyTeammateOnline(roleId);
			cell.m_offline_dead.SetActiveBetter(flag || flag2);
			bool trueOrFalse = false;
			bool trueOrFalse2 = false;
			BasePlayerController player = Battle.Ins.GetPlayer(roleId);
			if (player != null)
			{
				PlayerInfo playerInfo = player.PlayerInfo;
				if (playerInfo != null)
				{
					trueOrFalse = playerInfo.vehicleId > 0;
					trueOrFalse2 = ((roleId != Singleton<RoleMgr>.Ins.info.roleId) ? (Singleton<TeamateMgr>.Ins.GetTeamInfo(roleId).markList.Count > 0) : (Battle.Ins.GetMark() != null));
				}
				Image value;
				if (!_dicIndex2HpImage.TryGetValue(index, out value))
				{
					value = cell.m_hp.GetComponent<Image>();
					_dicIndex2HpImage[index] = value;
				}
				value.fillAmount = (float)player.HP / (float)ConstsBs.HpPlayer;
			}
			cell.m_waring.SetActiveBetter(flag2);
			cell.m_die.SetActiveBetter(flag);
			cell.m_mark.SetActiveBetter(trueOrFalse2);
			cell.m_car.SetActiveBetter(trueOrFalse);
		}

		private void UpdateTeammatesInfo()
		{
			m_btns.SetActiveBetter(false);
			Utils.TriggerEvent(TeamTaskEvent.RefreshShowBtnsDelegate);
			if (TeamTaskEvent.TeamBtnsActiveChangeDelegate != null)
			{
				TeamTaskEvent.TeamBtnsActiveChangeDelegate(false);
			}
			m_bg.SetActiveBetter(false);
			bool flag = Singleton<TeamScMgr>.Ins.IsInTeam();
			btn_create.SetActiveBetter(!flag);
			btn_exit.SetActiveBetter(flag);
			if (flag)
			{
				_selectRoleId = 0L;
				ResetLastSelect();
				int i = 0;
				for (int num = m_TeamPlayerList.Length; i < num; i++)
				{
					m_TeamPlayerListlist[i].m_notselect.SetActiveBetter(true);
					m_TeamPlayerListlist[i].m_select.SetActiveBetter(false);
				}
			}
			int j = 0;
			for (int num2 = m_TeamPlayerList.Length; j < num2; j++)
			{
				FillTeammateCell(m_TeamPlayerListlist[j], j);
			}
		}

		private void FillTeammateCell(GLeftTeamInfo cell, int index)
		{
			_003CFillTeammateCell_003Ec__AnonStorey0 _003CFillTeammateCell_003Ec__AnonStorey = new _003CFillTeammateCell_003Ec__AnonStorey0();
			_003CFillTeammateCell_003Ec__AnonStorey.cell = cell;
			_003CFillTeammateCell_003Ec__AnonStorey._0024this = this;
			List<TroopPlayer> teamates = Singleton<TeamScMgr>.Ins.GetTeamates();
			if (teamates.Count <= index)
			{
				_003CFillTeammateCell_003Ec__AnonStorey.cell.m_teamate.SetActiveBetter(false);
				_003CFillTeammateCell_003Ec__AnonStorey.cell.m_add.SetActiveBetter(true);
				ClickListener clickListener = ClickListener.Get(_003CFillTeammateCell_003Ec__AnonStorey.cell.btn_add, string.Empty);
				if (_003C_003Ef__am_0024cache3 == null)
				{
					_003C_003Ef__am_0024cache3 = _003CFillTeammateCell_003Em__9;
				}
				clickListener.onClick = _003C_003Ef__am_0024cache3;
				return;
			}
			_003CFillTeammateCell_003Ec__AnonStorey.cell.m_teamate.SetActiveBetter(true);
			_003CFillTeammateCell_003Ec__AnonStorey.cell.m_add.SetActiveBetter(false);
			_003CFillTeammateCell_003Ec__AnonStorey.info = teamates[index];
			_003CFillTeammateCell_003Ec__AnonStorey.roleId = _003CFillTeammateCell_003Ec__AnonStorey.info.role.roleId;
			View.SetLabelText(_003CFillTeammateCell_003Ec__AnonStorey.cell.txt_indexText, index + 1);
			View.SetLabelText(_003CFillTeammateCell_003Ec__AnonStorey.cell.txt_onText, _003CFillTeammateCell_003Ec__AnonStorey.info.role.name);
			BasicRoleInfo basicRoleInfo = ((_003CFillTeammateCell_003Ec__AnonStorey.roleId == Singleton<RoleMgr>.Ins.info.roleId) ? Singleton<RoleMgr>.Ins.info : Singleton<BasicInfoScMgr>.Ins.GetProperty(_003CFillTeammateCell_003Ec__AnonStorey.roleId, _003CFillTeammateCell_003Ec__AnonStorey.info.role.version, _003CFillTeammateCell_003Ec__AnonStorey.info.role.name).basicRoleInfo);
			View.SetItemSprite(_003CFillTeammateCell_003Ec__AnonStorey.cell.m_icon, RoleHeadCfg.Get(basicRoleInfo.headId).icon);
			if (basicRoleInfo.frameId > 0)
			{
				View.SetItemSprite(_003CFillTeammateCell_003Ec__AnonStorey.cell.m_frame, RoleHeadFrameCfg.Get(basicRoleInfo.frameId).frame);
			}
			bool flag = Singleton<TeamScMgr>.Ins.leaderId == _003CFillTeammateCell_003Ec__AnonStorey.roleId;
			_003CFillTeammateCell_003Ec__AnonStorey.cell.m_leader.SetActiveBetter(flag);
			_003CFillTeammateCell_003Ec__AnonStorey.cell.m_quanxian.SetActiveBetter(!flag && _003CFillTeammateCell_003Ec__AnonStorey.info.isPower);
			bool flag2 = Singleton<TeamateMgr>.Ins.IsDead(_003CFillTeammateCell_003Ec__AnonStorey.roleId);
			bool flag3 = !Singleton<TeamScMgr>.Ins.IsMyTeammateOnline(_003CFillTeammateCell_003Ec__AnonStorey.roleId);
			_003CFillTeammateCell_003Ec__AnonStorey.cell.m_offline_dead.SetActiveBetter(flag2 || flag3);
			bool trueOrFalse = false;
			bool trueOrFalse2 = false;
			BasePlayerController player = Battle.Ins.GetPlayer(_003CFillTeammateCell_003Ec__AnonStorey.roleId);
			if (player != null)
			{
				PlayerInfo playerInfo = player.PlayerInfo;
				if (playerInfo != null)
				{
					trueOrFalse = playerInfo.vehicleId > 0;
					TeamateInfo teamInfo = Singleton<TeamateMgr>.Ins.GetTeamInfo(_003CFillTeammateCell_003Ec__AnonStorey.roleId);
					trueOrFalse2 = ((_003CFillTeammateCell_003Ec__AnonStorey.roleId == Singleton<RoleMgr>.Ins.info.roleId) ? (Battle.Ins.GetMark() != null) : (teamInfo != null && teamInfo.markList.Count > 0));
				}
				Image value;
				if (!_dicIndex2HpImage.TryGetValue(index, out value))
				{
					value = _003CFillTeammateCell_003Ec__AnonStorey.cell.m_hp.GetComponent<Image>();
					_dicIndex2HpImage[index] = value;
				}
				value.fillAmount = (float)player.HP / (float)ConstsBs.HpPlayer;
			}
			_003CFillTeammateCell_003Ec__AnonStorey.cell.m_waring.SetActiveBetter(flag3);
			_003CFillTeammateCell_003Ec__AnonStorey.cell.m_die.SetActiveBetter(flag2);
			_003CFillTeammateCell_003Ec__AnonStorey.cell.m_mark.SetActiveBetter(trueOrFalse2);
			_003CFillTeammateCell_003Ec__AnonStorey.cell.m_car.SetActiveBetter(trueOrFalse);
			ClickListener.Get(_003CFillTeammateCell_003Ec__AnonStorey.cell.m_teamate, string.Empty).onClick = _003CFillTeammateCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private void ResetBtnsShow(long roleId)
		{
			bool flag = Singleton<TeamScMgr>.Ins.IsLeader();
			bool flag2 = roleId == Singleton<RoleMgr>.Ins.info.roleId;
			bool trueOrFalse = flag && !flag2;
			btn_kick.SetActiveBetter(trueOrFalse);
			btn_trans_leader.SetActiveBetter(trueOrFalse);
			btn_allow.SetActiveBetter(trueOrFalse);
			btn_care.SetActiveBetter(!flag2);
		}

		private void Awake()
		{
			btn_allow = base.transform.Find("m_btns/GameObject/btn_allow").gameObject;
			btn_care = base.transform.Find("m_btns/GameObject/btn_care").gameObject;
			btn_create = base.transform.Find("m_team_page/btn_create").gameObject;
			btn_exit = base.transform.Find("m_btns/GameObject/btn_exit").gameObject;
			btn_infor = base.transform.Find("m_btns/GameObject/btn_infor").gameObject;
			btn_kick = base.transform.Find("m_btns/GameObject/btn_kick").gameObject;
			btn_trans_leader = base.transform.Find("m_btns/GameObject/btn_trans_leader").gameObject;
			m_bg = base.transform.Find("m_bg").gameObject;
			m_btns = base.transform.Find("m_btns").gameObject;
			m_can_get = base.transform.Find("m_team_page/m_teammate/m_TeamPlayerList/m_shouyi/m_can_get").gameObject;
			m_keLingQu = base.transform.Find("m_team_page/m_teammate/m_TeamPlayerList/m_shouyi/m_can_get/m_keLingQu").gameObject;
			m_kongXiangZi = base.transform.Find("m_team_page/m_teammate/m_TeamPlayerList/m_shouyi/m_can_get/m_kongXiangZi").gameObject;
			m_max_get = base.transform.Find("m_team_page/m_teammate/m_TeamPlayerList/m_shouyi/m_max_get").gameObject;
			m_progress_slider = base.transform.Find("m_team_page/m_teammate/m_TeamPlayerList/m_shouyi/m_can_get/m_progress_slider").gameObject;
			m_shouyi = base.transform.Find("m_team_page/m_teammate/m_TeamPlayerList/m_shouyi").gameObject;
			m_team_page = base.transform.Find("m_team_page").gameObject;
			m_teammate = base.transform.Find("m_team_page/m_teammate").gameObject;
			m_TeamPlayerList = base.transform.Find("m_team_page/m_teammate/m_TeamPlayerList").gameObject.GetComponent<UIGameObjectList>().objects;
			m_TeamPlayerListObj = base.transform.Find("m_team_page/m_teammate/m_TeamPlayerList").gameObject;
			if (m_TeamPlayerListlist.Count <= 0)
			{
				for (int i = 0; i < m_TeamPlayerList.Length; i++)
				{
					m_TeamPlayerListlist.Add(View.AddComponentIfNotExist<GLeftTeamInfo>(m_TeamPlayerList[i].gameObject));
				}
			}
			txt_progress = base.transform.Find("m_team_page/m_teammate/m_TeamPlayerList/m_shouyi/m_can_get/m_progress_slider/txt_progress").gameObject;
			txt_progressText = txt_progress.GetComponent<Text>();
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__0(GameObject go)
		{
			Singleton<TeamScMgr>.Ins.CreateTeam();
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__1(GameObject go)
		{
			if (_003C_003Ef__am_0024cache4 == null)
			{
				_003C_003Ef__am_0024cache4 = _003COnInit_003Em__A;
			}
			TeamTaskPopupPanel.Show(2, _003C_003Ef__am_0024cache4);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__2(GameObject go)
		{
			if (_selectRoleId > 0)
			{
				Singleton<TeamScMgr>.Ins.KickOther(_selectRoleId);
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__3(GameObject go)
		{
			if (_selectRoleId > 0)
			{
				Singleton<TeamScMgr>.Ins.TransferLeader(_selectRoleId);
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__4(GameObject go)
		{
			if (_selectRoleId > 0)
			{
				Singleton<TeamScMgr>.Ins.GivePermision(_selectRoleId);
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__5(GameObject go)
		{
			if (_selectRoleId > 0)
			{
				if (Singleton<FriendScMgr>.Ins.IsInCareList(_selectRoleId))
				{
					Singleton<FriendScMgr>.Ins.RemoveCare(_selectRoleId);
				}
				else
				{
					Singleton<FriendScMgr>.Ins.AddCare(_selectRoleId);
				}
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__6(GameObject go)
		{
			Singleton<RoleMgr>.Ins.GetRoleMoreInformation(_selectRoleId);
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__7(GameObject go)
		{
			PassEvent(ClickListener.pointEventData, ExecuteEvents.pointerClickHandler);
			ResetLastSelect();
			m_btns.SetActiveBetter(false);
			Utils.TriggerEvent(TeamTaskEvent.RefreshShowBtnsDelegate);
			if (TeamTaskEvent.TeamBtnsActiveChangeDelegate != null)
			{
				TeamTaskEvent.TeamBtnsActiveChangeDelegate(false);
			}
			m_bg.SetActiveBetter(false);
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__8(GameObject go)
		{
			if (!Singleton<TeamScMgr>.Ins.IsInTeam())
			{
				AlertBox.Show(402);
			}
			else if (Singleton<TeamScMgr>.Ins.MySTeamEarningChange == null || !Singleton<TeamScMgr>.Ins.MySTeamEarningChange.isCanGet)
			{
				AlertBox.Show(403);
			}
			else if (Singleton<TeamScMgr>.Ins.MySTeamEarningChange.getCount < cfg.Consts.TEAM_EARNINGREWARD_MAX_GETCOUNT)
			{
				Singleton<TeamScMgr>.Ins.SendGetTeamEarningRewardMsg();
			}
			else
			{
				AlertBox.Show(400);
			}
		}

		[CompilerGenerated]
		private static void _003CFillTeammateCell_003Em__9(GameObject go)
		{
			ViewMgr.Ins.ShowTopView<FriendPanel>();
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__A()
		{
			Singleton<TeamScMgr>.Ins.LeaveTeam();
		}
	}
}
