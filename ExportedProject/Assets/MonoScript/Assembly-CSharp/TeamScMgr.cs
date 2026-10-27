using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SC.UI;
using UnityEngine;
using cfg;
using gs.role.scmsg;
using gs.troop.scmsg;

public class TeamScMgr : Singleton<TeamScMgr>
{
	[CompilerGenerated]
	private sealed class _003COnSTroopInvite_003Ec__AnonStorey0
	{
		internal STroopInvite msg;

		internal bool _003C_003Em__0(STroopInvite invite)
		{
			return invite.role.roleId == msg.role.roleId;
		}
	}

	public long leaderId;

	public TroopPlayer MyInfo = new TroopPlayer();

	private List<TroopPlayer> partnerInfos = new List<TroopPlayer>();

	private Dictionary<long, TroopPlayer> _id2Player = new Dictionary<long, TroopPlayer>();

	private string notReadyNames;

	public List<STroopInvite> inviteInfo = new List<STroopInvite>();

	public List<SRoleApplyTroop> ApplicationList = new List<SRoleApplyTroop>();

	public Dictionary<long, SRoleApplyTroop> dicApplication = new Dictionary<long, SRoleApplyTroop>();

	private Coroutine _autoReady;

	public float NextTimeCanSendRecruit;

	public STeamEarningChange MySTeamEarningChange;

	private bool _isTriggerCreateEvent;

	private readonly CGetTeamEarningReward _cGetTeamEarningReward = new CGetTeamEarningReward();

	private readonly CAutoRefuse _cAutoRefuse = new CAutoRefuse();

	private readonly CInviteRole _cInviteRole = new CInviteRole();

	private readonly COperateCMD _cOperateCmd = new COperateCMD();

	private readonly CCreateTroop _cCreateTroop = new CCreateTroop();

	private readonly CSendInviteMsg _cSendInviteMsg = new CSendInviteMsg();

	private CApplyToTroop _cApplyToTroop = new CApplyToTroop();

	public int TeamId { get; private set; }

	public void Init()
	{
		MyInfo.isReady = false;
		MyInfo.isPower = true;
		MyInfo.isOnline = true;
		SCreateTroop.handler = (SCreateTroop.Handler)Delegate.Combine(SCreateTroop.handler, new SCreateTroop.Handler(OnSCreateTroop));
		STroopInfo.handler = (STroopInfo.Handler)Delegate.Combine(STroopInfo.handler, new STroopInfo.Handler(OnSTroopInfo));
		STroopInvite.handler = (STroopInvite.Handler)Delegate.Combine(STroopInvite.handler, new STroopInvite.Handler(OnSTroopInvite));
		SRoleApplyTroop.handler = (SRoleApplyTroop.Handler)Delegate.Combine(SRoleApplyTroop.handler, new SRoleApplyTroop.Handler(OnSRoleApplyTroop));
		SOperateCMD.handler = (SOperateCMD.Handler)Delegate.Combine(SOperateCMD.handler, new SOperateCMD.Handler(OnSOperateCMD));
		SApplyToTroop.handler = (SApplyToTroop.Handler)Delegate.Combine(SApplyToTroop.handler, new SApplyToTroop.Handler(OnSApplyToTroop));
		SInviteRole.handler = (SInviteRole.Handler)Delegate.Combine(SInviteRole.handler, new SInviteRole.Handler(OnSInviteRole));
		SErrorTroop.handler = (SErrorTroop.Handler)Delegate.Combine(SErrorTroop.handler, new SErrorTroop.Handler(OnSErrorTroop));
		LoginEvent.OnLoginEvent = (Utils.VoidDelegate)Delegate.Combine(LoginEvent.OnLoginEvent, new Utils.VoidDelegate(ClearCacheTeamInfo));
		SSendInviteMsg.handler = (SSendInviteMsg.Handler)Delegate.Combine(SSendInviteMsg.handler, new SSendInviteMsg.Handler(OnSSendInviteMsg));
		ViewMgr.Ins.AddOnShowEvent<BattlePanel>(ShowMessagePanel);
		ViewMgr.Ins.AddOnShowEvent<BattlePanel>(ShowTeamTaskPanel);
		ViewMgr.Ins.AddOnHideEvent<BattlePanel>(HideTeamTaskPanel);
		STeamateChangeName.handler = (STeamateChangeName.Handler)Delegate.Combine(STeamateChangeName.handler, new STeamateChangeName.Handler(OnSTeammateChangeName));
		SChangeName.handler = (SChangeName.Handler)Delegate.Combine(SChangeName.handler, new SChangeName.Handler(OnSChangeName));
		STeamEarningChange.handler = (STeamEarningChange.Handler)Delegate.Combine(STeamEarningChange.handler, new STeamEarningChange.Handler(OnSTeamEarningChange));
	}

	private void OnSTeamEarningChange(STeamEarningChange msg)
	{
		MySTeamEarningChange = msg;
	}

	private void OnSChangeName(SChangeName msg)
	{
		long roleId = Singleton<RoleMgr>.Ins.info.roleId;
		foreach (KeyValuePair<long, TroopPlayer> item in _id2Player)
		{
			if (item.Key == roleId)
			{
				item.Value.role.name = msg.name;
				Singleton<BasicInfoScMgr>.Ins.GetProperty(item.Key, 0, msg.name).basicRoleInfo.name = msg.name;
				if (TeamScEvent.TeammateChangeNameAction != null)
				{
					TeamScEvent.TeammateChangeNameAction();
				}
				break;
			}
		}
	}

	public void SendGetTeamEarningRewardMsg()
	{
		Client2Gs.Ins.Send(_cGetTeamEarningReward);
	}

	private void OnSTeammateChangeName(STeamateChangeName msg)
	{
		foreach (KeyValuePair<long, TroopPlayer> item in _id2Player)
		{
			if (item.Key == msg.roleId)
			{
				item.Value.role.name = msg.name;
				Singleton<BasicInfoScMgr>.Ins.GetProperty(item.Key, 0, msg.name).basicRoleInfo.name = msg.name;
				if (TeamScEvent.TeammateChangeNameAction != null)
				{
					TeamScEvent.TeammateChangeNameAction();
				}
				break;
			}
		}
	}

	private void ShowTeamTaskPanel()
	{
		ViewMgr.Ins.ShowView<TeamTaskPanel>(null, false);
	}

	private void HideTeamTaskPanel()
	{
		ViewMgr.Ins.HideView<TeamTaskPanel>();
	}

	private void ShowMessagePanel()
	{
		if ((IsLeader() && ApplicationList.Count > 0) || (!IsInTeam() && inviteInfo.Count > 0))
		{
			ViewMgr.Ins.ShowTopView<TeamMessagePanel>();
		}
	}

	private void OnSSendInviteMsg(SSendInviteMsg msg)
	{
		try
		{
			Utils.TriggerEvent(TeamScEvent.SendInviteMsgSuccessEvent);
		}
		catch (Exception)
		{
		}
	}

	private void ClearCacheTeamInfo()
	{
		OnSKickTroop();
	}

	private void OnSErrorTroop(SErrorTroop msg)
	{
		switch (msg.code)
		{
		case 0:
			AlertBox.Show(Utils.GetString(47));
			break;
		case 1:
			AlertBox.Show(Utils.GetString(48));
			break;
		case 2:
			AlertBox.Show(Utils.GetString(49));
			break;
		case 3:
			AlertBox.Show(Utils.GetString(50));
			break;
		case 4:
			AlertBox.Show(Utils.GetString(51));
			break;
		case 5:
		{
			AllBasicInfo property3 = Singleton<BasicInfoScMgr>.Ins.GetProperty(msg.otherId, int.MinValue);
			Singleton<BasicInfoScMgr>.Ins.StatusChange(msg.otherId, (byte)(property3.status & 0xDu));
			AlertBox.Show(Utils.GetString(52));
			break;
		}
		case 6:
			AlertBox.Show(Utils.GetString(53));
			break;
		case 7:
			AlertBox.Show(Utils.GetString(54));
			break;
		case 8:
		{
			AllBasicInfo property2 = Singleton<BasicInfoScMgr>.Ins.GetProperty(msg.otherId, int.MinValue);
			Singleton<BasicInfoScMgr>.Ins.StatusChange(msg.otherId, 0);
			AlertBox.Show(Utils.GetString(55));
			break;
		}
		case 9:
			AlertBox.Show(Utils.GetString(56));
			break;
		case 10:
		{
			AllBasicInfo property = Singleton<BasicInfoScMgr>.Ins.GetProperty(msg.otherId, int.MinValue);
			Singleton<BasicInfoScMgr>.Ins.StatusChange(msg.otherId, (byte)(property.status | 2u));
			AlertBox.Show(Utils.GetString(57));
			break;
		}
		case 404:
			AlertBox.Show(Utils.GetString(58));
			break;
		case 11:
			AlertBox.Show(Utils.GetString(59));
			break;
		case 12:
		case 13:
			AlertBox.Show(Utils.GetString(60));
			break;
		case 14:
			AlertBox.Show(61);
			break;
		}
	}

	private void OnSInviteRole(SInviteRole msg)
	{
		AlertBox.Show(Utils.GetString(62));
		Singleton<BasicInfoScMgr>.Ins.StatusChange(msg.otherId, 1);
	}

	private void OnSApplyToTroop(SApplyToTroop msg)
	{
		AlertBox.Show(47);
	}

	private void OnSOperateCMD(SOperateCMD msg)
	{
		if (TeamId != msg.troopId)
		{
			return;
		}
		foreach (TroopPlayer partnerInfo in partnerInfos)
		{
			if (partnerInfo.role.roleId != msg.otherId)
			{
				continue;
			}
			switch (msg.operateType)
			{
			case 1:
				partnerInfo.isOnline = true;
				if (TeamScEvent.UpdateOnlineStatusEvent != null)
				{
					TeamScEvent.UpdateOnlineStatusEvent(msg.otherId, true);
				}
				break;
			case 2:
				partnerInfo.isOnline = false;
				if (TeamScEvent.UpdateOnlineStatusEvent != null)
				{
					TeamScEvent.UpdateOnlineStatusEvent(msg.otherId, false);
				}
				break;
			case 3:
				_id2Player.Remove(msg.otherId);
				if (msg.otherId == Singleton<RoleMgr>.Ins.info.roleId)
				{
					OnSLevelTroop();
				}
				SoundEffectMgr.TeamRemoveSound();
				partnerInfos.Remove(partnerInfo);
				Utils.TriggerEvent(TeamScEvent.UpdateLeaveOrAddTroopEvent);
				if (TeamScEvent.TeammateCutDownAction != null)
				{
					TeamScEvent.TeammateCutDownAction(msg.otherId);
				}
				break;
			case 4:
				if (msg.otherId == Singleton<RoleMgr>.Ins.info.roleId)
				{
					OnSKickTroop();
				}
				partnerInfos.Remove(partnerInfo);
				Utils.TriggerEvent(TeamScEvent.UpdateLeaveOrAddTroopEvent);
				if (TeamScEvent.TeammateCutDownAction != null)
				{
					TeamScEvent.TeammateCutDownAction(msg.otherId);
				}
				break;
			case 5:
				leaderId = partnerInfo.role.roleId;
				partnerInfos.Sort(SortPlayersList);
				AlertBox.Show(64, Singleton<BasicInfoScMgr>.Ins.GetProperty(leaderId, int.MinValue, string.Empty).basicRoleInfo.name);
				Utils.TriggerEvent(TeamScEvent.ChangeLeaderEvent);
				break;
			case 8:
				partnerInfo.isPower = true;
				if (IsLeader())
				{
					AlertBox.Show(65);
				}
				else if (Singleton<RoleMgr>.Ins.info.roleId == msg.otherId)
				{
					AlertBox.Show(66);
				}
				break;
			case 9:
				partnerInfo.isPower = false;
				break;
			case 6:
			case 7:
				break;
			}
			break;
		}
	}

	private void OnSRoleApplyTroop(SRoleApplyTroop msg)
	{
		if (!SettingMgr.ZuDui)
		{
			AlertBox.Show(417, msg.role.name);
			_cAutoRefuse.roleId = msg.role.roleId;
			_cAutoRefuse.aletId = 419;
			Client2Gs.Ins.Send(_cAutoRefuse);
		}
		else if (!dicApplication.ContainsKey(msg.role.roleId))
		{
			Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(msg.role.roleId, msg.role.name, 1);
			if (ApplicationList.Count > cfg.Consts.MAX_TEAM_APPLY_NUM)
			{
				dicApplication.Remove(ApplicationList[0].role.roleId);
				ApplicationList.RemoveAt(0);
			}
			ApplicationList.Add(msg);
			dicApplication.Add(msg.role.roleId, msg);
			ViewMgr.Ins.ShowTopView<TeamMessagePanel>();
		}
	}

	private void OnSTroopInfo(STroopInfo msg)
	{
		if (partnerInfos.Count < msg.players.Count)
		{
			SoundEffectMgr.TeamAddSound();
		}
		TeamId = msg.troopId;
		leaderId = msg.leaderId;
		if (!IsInTeam() && !IsLeader())
		{
			AlertBox.Show(67);
		}
		partnerInfos.Clear();
		_id2Player.Clear();
		foreach (TroopPlayer player in msg.players)
		{
			partnerInfos.Add(player);
			_id2Player.Add(player.role.roleId, player);
			Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(player.role.roleId, player.role.name, 3);
			if (player.role.roleId == Singleton<RoleMgr>.Ins.info.roleId)
			{
				MyInfo = player;
			}
		}
		partnerInfos.Sort(SortPlayersList);
		inviteInfo.Clear();
		if (ApplicationList.Count <= 0)
		{
			ViewMgr.Ins.HideView<TeamMessagePanel>();
		}
		Utils.TriggerEvent(TeamScEvent.UpdateLeaveOrAddTroopEvent);
	}

	private void OnSCreateTroop(SCreateTroop msg)
	{
		_isTriggerCreateEvent = true;
	}

	private void OnSLevelTroop()
	{
		MySTeamEarningChange = null;
		MyInfo.isReady = false;
		MyInfo.isPower = true;
		leaderId = -1L;
		TeamId = -1;
		_id2Player.Clear();
		partnerInfos.Clear();
		ApplicationList.Clear();
		if (inviteInfo.Count <= 0)
		{
			ViewMgr.Ins.HideView<TeamMessagePanel>();
		}
		dicApplication.Clear();
		Utils.TriggerEvent(TeamScEvent.LeaveTeamEvent);
		AlertBox.Show(68);
	}

	private void OnSKickTroop()
	{
		MySTeamEarningChange = null;
		MyInfo.isReady = false;
		MyInfo.isPower = true;
		leaderId = -1L;
		TeamId = -1;
		partnerInfos.Clear();
		_id2Player.Clear();
		Utils.TriggerEvent(TeamScEvent.LeaveTeamEvent);
	}

	private void OnSTroopInvite(STroopInvite msg)
	{
		_003COnSTroopInvite_003Ec__AnonStorey0 _003COnSTroopInvite_003Ec__AnonStorey = new _003COnSTroopInvite_003Ec__AnonStorey0();
		_003COnSTroopInvite_003Ec__AnonStorey.msg = msg;
		if (!SettingMgr.ZuDui)
		{
			AlertBox.Show(418, _003COnSTroopInvite_003Ec__AnonStorey.msg.role.name);
			_cAutoRefuse.roleId = _003COnSTroopInvite_003Ec__AnonStorey.msg.role.roleId;
			_cAutoRefuse.aletId = 419;
			Client2Gs.Ins.Send(_cAutoRefuse);
		}
		else if (!inviteInfo.Any(_003COnSTroopInvite_003Ec__AnonStorey._003C_003Em__0))
		{
			Singleton<BasicInfoScMgr>.Ins.AddBasicInfo(_003COnSTroopInvite_003Ec__AnonStorey.msg.role.roleId, _003COnSTroopInvite_003Ec__AnonStorey.msg.role.name, 3);
			inviteInfo.Add(_003COnSTroopInvite_003Ec__AnonStorey.msg);
			ViewMgr.Ins.ShowTopView<TeamMessagePanel>();
		}
	}

	public bool IsInTeam()
	{
		return partnerInfos.Count > 0;
	}

	public bool IsLeader()
	{
		return Singleton<RoleMgr>.Ins.info.roleId == leaderId;
	}

	public bool IsInMyTeam(long roleId)
	{
		int i = 0;
		for (int count = partnerInfos.Count; i < count; i++)
		{
			if (partnerInfos[i].role.roleId == roleId)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanInvite()
	{
		return MyInfo.isPower;
	}

	public bool IsMyTeammateOnline(long roleId, out bool isOnline)
	{
		isOnline = false;
		TroopPlayer value;
		if (IsInTeam() && _id2Player.TryGetValue(roleId, out value))
		{
			isOnline = value.isOnline;
			return true;
		}
		return false;
	}

	public bool IsMyTeammateOnline(long roleId)
	{
		TroopPlayer value;
		if (IsInTeam() && _id2Player.TryGetValue(roleId, out value))
		{
			return value.isOnline;
		}
		return false;
	}

	public List<TroopPlayer> GetTeamates()
	{
		return partnerInfos;
	}

	public void InviteToJoin(long otherId)
	{
		if (otherId == Singleton<RoleMgr>.Ins.info.roleId)
		{
			AlertBox.Show(69);
			return;
		}
		_cInviteRole.otherId = otherId;
		Client2Gs.Ins.Send(_cInviteRole);
	}

	public void KickOther(long roleId)
	{
		_cOperateCmd.otherId = roleId;
		_cOperateCmd.operateType = 4;
		Client2Gs.Ins.Send(_cOperateCmd);
	}

	public void TransferLeader(long roleId)
	{
		_cOperateCmd.otherId = roleId;
		_cOperateCmd.operateType = 5;
		Client2Gs.Ins.Send(_cOperateCmd);
	}

	public void GivePermision(long roleId)
	{
		_cOperateCmd.otherId = roleId;
		_cOperateCmd.operateType = 8;
		Client2Gs.Ins.Send(_cOperateCmd);
	}

	public void LeaveTeam()
	{
		if (IsLeader() && partnerInfos.Count > 1)
		{
			TransferLeader(partnerInfos[1].role.roleId);
		}
		_cOperateCmd.otherId = Singleton<RoleMgr>.Ins.info.roleId;
		_cOperateCmd.operateType = 3;
		Client2Gs.Ins.Send(_cOperateCmd);
	}

	public void CreateTeam()
	{
		Client2Gs.Ins.Send(_cCreateTroop);
	}

	public void SendInviteMsg(string str)
	{
		_cSendInviteMsg.text = str;
		Client2Gs.Ins.Send(_cSendInviteMsg);
		int tROOP_RECRUIT_TIME_LIMIT = cfg.Consts.TROOP_RECRUIT_TIME_LIMIT;
		NextTimeCanSendRecruit = Time.realtimeSinceStartup + (float)tROOP_RECRUIT_TIME_LIMIT;
	}

	public bool IsCanSendInviteMsg()
	{
		return Time.realtimeSinceStartup >= NextTimeCanSendRecruit;
	}

	public void SendApplyToTeam(long otherId)
	{
		_cApplyToTroop.otherId = otherId;
		Client2Gs.Ins.Send(_cApplyToTroop);
	}

	public void ClearInviteAndApply()
	{
		inviteInfo.Clear();
		ApplicationList.Clear();
		dicApplication.Clear();
	}

	private int SortPlayersList(TroopPlayer player1, TroopPlayer player2)
	{
		if (player1.role.roleId == leaderId)
		{
			return -1;
		}
		if (player2.role.roleId == leaderId)
		{
			return 1;
		}
		return (int)(player1.role.roleId - player2.role.roleId);
	}

	public int SortPlayersList(long roleId1, long roleId2)
	{
		if (roleId1 == leaderId)
		{
			return -1;
		}
		if (roleId2 == leaderId)
		{
			return 1;
		}
		return (int)(roleId1 - roleId2);
	}

	public void JoinOrInviteToTeam(long roleId, int status)
	{
		if (IsInMyTeam(roleId))
		{
			AlertBox.Show(70);
		}
		else if ((status & 4) > 0)
		{
			AlertBox.Show(71);
		}
		else if ((status & 2) > 0)
		{
			if (IsInTeam())
			{
				InviteToJoin(roleId);
			}
			else
			{
				SendApplyToTeam(roleId);
			}
		}
		else if (IsLeader() || CanInvite())
		{
			InviteToJoin(roleId);
		}
		else
		{
			AlertBox.Show(Utils.GetString(63));
		}
	}
}
