using System;
using System.Collections.Generic;
using UnityEngine;
using gs.battle.scmsg;

public class TeamateMgr : Singleton<TeamateMgr>
{
	private readonly List<TeamateInfo> _mTeamateInfoList = new List<TeamateInfo>();

	public Dictionary<long, Vector3> PlayerDieDic = new Dictionary<long, Vector3>();

	private bool _isOnSTeamInfosError;

	private Vector3 _diePos = Vector3.zero;

	public Vector3 PlayDiePos(long id)
	{
		Vector3 value = Vector3.one;
		PlayerDieDic.TryGetValue(id, out value);
		return value;
	}

	public void Init()
	{
		SEnterRoomhandle();
	}

	private void SEnterRoomhandle()
	{
		STeamateInfos.handler = (STeamateInfos.Handler)Delegate.Combine(STeamateInfos.handler, new STeamateInfos.Handler(OnSTeamInfos));
		STeamateInfo.handler = (STeamateInfo.Handler)Delegate.Combine(STeamateInfo.handler, new STeamateInfo.Handler(OnSTeamInfo));
		TeamScEvent.TeammateCutDownAction = (Action<long>)Delegate.Combine(TeamScEvent.TeammateCutDownAction, new Action<long>(TeammateCutDownAction));
		SGetInVehicle.handler = (SGetInVehicle.Handler)Delegate.Combine(SGetInVehicle.handler, new SGetInVehicle.Handler(OnSGetInVehicle));
		SGetOutVehicle.handler = (SGetOutVehicle.Handler)Delegate.Combine(SGetOutVehicle.handler, new SGetOutVehicle.Handler(OnSGetOutVehicle));
		SMakeMark.handler = (SMakeMark.Handler)Delegate.Combine(SMakeMark.handler, new SMakeMark.Handler(OnSMakeMark));
		SRemoveMark.handler = (SRemoveMark.Handler)Delegate.Combine(SRemoveMark.handler, new SRemoveMark.Handler(OnSRemoveMark));
		SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Combine(SPlayerDie.handler, new SPlayerDie.Handler(OnSPlayerDie));
		TeamScEvent.UpdateOnlineStatusEvent = (Action<long, bool>)Delegate.Combine(TeamScEvent.UpdateOnlineStatusEvent, new Action<long, bool>(UpdateOnlineStatusEvent));
		SFlyBattlePlane.handler = (SFlyBattlePlane.Handler)Delegate.Combine(SFlyBattlePlane.handler, new SFlyBattlePlane.Handler(OnSFlyBattlePlane));
		SSyncOpenParachute.handler = (SSyncOpenParachute.Handler)Delegate.Combine(SSyncOpenParachute.handler, new SSyncOpenParachute.Handler(OnSSyncOpenParachute));
		SSyncFallGround.handler = (SSyncFallGround.Handler)Delegate.Combine(SSyncFallGround.handler, new SSyncFallGround.Handler(OnSSyncFallGround));
		SSyncLevelFlyBattlePlane.handler = (SSyncLevelFlyBattlePlane.Handler)Delegate.Combine(SSyncLevelFlyBattlePlane.handler, new SSyncLevelFlyBattlePlane.Handler(OnSSyncLevelFlyBattlePlane));
		BattleEvent.OnReciveTeamPlayerInfo = (BattleEvent.OnReciveteamPlayerInfo)Delegate.Combine(BattleEvent.OnReciveTeamPlayerInfo, new BattleEvent.OnReciveteamPlayerInfo(OnReciveTeamPlayerInfo));
		SPlayerInfo.handler = (SPlayerInfo.Handler)Delegate.Combine(SPlayerInfo.handler, new SPlayerInfo.Handler(OnSPlayerInfo));
		SRebirth.handler = (SRebirth.Handler)Delegate.Combine(SRebirth.handler, new SRebirth.Handler(SRebirthHandle));
	}

	private void TeammateCutDownAction(long obj)
	{
		if (obj == Singleton<RoleMgr>.Ins.info.roleId)
		{
			_mTeamateInfoList.Clear();
			PlayerDieDic.Clear();
		}
		else
		{
			RemoveTeamNumber(obj);
		}
		TriggerEvent();
	}

	private void OnSPlayerInfo(SPlayerInfo sPlayerInfo)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == sPlayerInfo.playerInfo.roleId)
			{
				mTeamateInfo.status = sPlayerInfo.playerInfo.curStatus;
				mTeamateInfo.isDead = sPlayerInfo.playerInfo.curStatus == 11;
				break;
			}
		}
	}

	public void Clear()
	{
		_mTeamateInfoList.Clear();
		PlayerDieDic.Clear();
		SPlayerInfo.handler = (SPlayerInfo.Handler)Delegate.Remove(SPlayerInfo.handler, new SPlayerInfo.Handler(OnSPlayerInfo));
		STeamateInfos.handler = (STeamateInfos.Handler)Delegate.Remove(STeamateInfos.handler, new STeamateInfos.Handler(OnSTeamInfos));
		STeamateInfo.handler = (STeamateInfo.Handler)Delegate.Remove(STeamateInfo.handler, new STeamateInfo.Handler(OnSTeamInfo));
		TeamScEvent.TeammateCutDownAction = (Action<long>)Delegate.Remove(TeamScEvent.TeammateCutDownAction, new Action<long>(TeammateCutDownAction));
		SGetInVehicle.handler = (SGetInVehicle.Handler)Delegate.Remove(SGetInVehicle.handler, new SGetInVehicle.Handler(OnSGetInVehicle));
		SGetOutVehicle.handler = (SGetOutVehicle.Handler)Delegate.Remove(SGetOutVehicle.handler, new SGetOutVehicle.Handler(OnSGetOutVehicle));
		SMakeMark.handler = (SMakeMark.Handler)Delegate.Remove(SMakeMark.handler, new SMakeMark.Handler(OnSMakeMark));
		SRemoveMark.handler = (SRemoveMark.Handler)Delegate.Remove(SRemoveMark.handler, new SRemoveMark.Handler(OnSRemoveMark));
		SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Remove(SPlayerDie.handler, new SPlayerDie.Handler(OnSPlayerDie));
		TeamScEvent.UpdateOnlineStatusEvent = (Action<long, bool>)Delegate.Remove(TeamScEvent.UpdateOnlineStatusEvent, new Action<long, bool>(UpdateOnlineStatusEvent));
		SFlyBattlePlane.handler = (SFlyBattlePlane.Handler)Delegate.Remove(SFlyBattlePlane.handler, new SFlyBattlePlane.Handler(OnSFlyBattlePlane));
		SSyncOpenParachute.handler = (SSyncOpenParachute.Handler)Delegate.Remove(SSyncOpenParachute.handler, new SSyncOpenParachute.Handler(OnSSyncOpenParachute));
		SSyncFallGround.handler = (SSyncFallGround.Handler)Delegate.Remove(SSyncFallGround.handler, new SSyncFallGround.Handler(OnSSyncFallGround));
		SSyncLevelFlyBattlePlane.handler = (SSyncLevelFlyBattlePlane.Handler)Delegate.Remove(SSyncLevelFlyBattlePlane.handler, new SSyncLevelFlyBattlePlane.Handler(OnSSyncLevelFlyBattlePlane));
		BattleEvent.OnReciveTeamPlayerInfo = (BattleEvent.OnReciveteamPlayerInfo)Delegate.Remove(BattleEvent.OnReciveTeamPlayerInfo, new BattleEvent.OnReciveteamPlayerInfo(OnReciveTeamPlayerInfo));
		SRebirth.handler = (SRebirth.Handler)Delegate.Remove(SRebirth.handler, new SRebirth.Handler(SRebirthHandle));
	}

	private void SRebirthHandle(SRebirth msg)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == msg.playerInfo.roleId)
			{
				mTeamateInfo.isDead = false;
				break;
			}
		}
	}

	public List<TeamateInfo> GetTeamateInfo()
	{
		return _mTeamateInfoList;
	}

	public TeamateInfo GetTeamInfo(long roleId)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == roleId)
			{
				return mTeamateInfo;
			}
		}
		return null;
	}

	public bool IsTeamate(long otherId)
	{
		return GetTeamInfo(otherId) != null;
	}

	private void OnSFlyBattlePlane(SFlyBattlePlane sFlyBattlePlane)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			mTeamateInfo.status = 2;
		}
	}

	private void OnSSyncOpenParachute(SSyncOpenParachute sSyncOpenParachute)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == sSyncOpenParachute.roleId)
			{
				mTeamateInfo.status = 4;
				break;
			}
		}
	}

	private void OnSSyncLevelFlyBattlePlane(SSyncLevelFlyBattlePlane sSyncLevelFlyBattlePlane)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == sSyncLevelFlyBattlePlane.roleId)
			{
				mTeamateInfo.status = 3;
				break;
			}
		}
	}

	private void OnSSyncFallGround(SSyncFallGround sSyncFallGround)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == sSyncFallGround.roleId)
			{
				mTeamateInfo.status = 5;
				break;
			}
		}
	}

	private void OnSTeamInfos(STeamateInfos sTeamateInfos)
	{
		_mTeamateInfoList.Clear();
		foreach (TeamateInfo teamateInfo in sTeamateInfos.teamateInfos)
		{
			_mTeamateInfoList.Add(teamateInfo);
		}
		_mTeamateInfoList.Sort(CompareTeamInfo);
		Utils.TriggerEvent(MapEvent.OnSTeamInfoDelegate);
		TriggerEvent();
	}

	private void OnSTeamInfo(STeamateInfo sTeamateInfo)
	{
		bool flag = false;
		int i = 0;
		for (int count = _mTeamateInfoList.Count; i < count; i++)
		{
			if (_mTeamateInfoList[i].roleId == sTeamateInfo.teamateInfo.roleId)
			{
				Debug.LogWarning("sTeamateInfo.teamateInfo:" + sTeamateInfo.teamateInfo.isDead);
				_mTeamateInfoList[i] = sTeamateInfo.teamateInfo;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			_mTeamateInfoList.Add(sTeamateInfo.teamateInfo);
			_mTeamateInfoList.Sort(CompareTeamInfo);
		}
		TriggerEvent();
	}

	private void OnSPlayerDie(SPlayerDie sPlayerDie)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId != sPlayerDie.roleId)
			{
				continue;
			}
			mTeamateInfo.isDead = true;
			if (Battle.Ins.TeamPlayerDic.ContainsKey(sPlayerDie.roleId))
			{
				BasePlayerController basePlayerController = Battle.Ins.TeamPlayerDic[sPlayerDie.roleId];
				if (basePlayerController != null)
				{
					PlayerDieDic[sPlayerDie.roleId] = basePlayerController.Pos;
				}
				TriggerEvent();
			}
			break;
		}
	}

	private void OnReciveTeamPlayerInfo(PlayerInfo playerinfo)
	{
		if (playerinfo.roleId != Singleton<RoleMgr>.Ins.info.roleId)
		{
			if (playerinfo.curStatus == 11)
			{
				_diePos.Set(playerinfo.pos.x, playerinfo.pos.y, playerinfo.pos.z);
				PlayerDieDic[playerinfo.roleId] = _diePos;
			}
			else if (PlayerDieDic.ContainsKey(playerinfo.roleId))
			{
				PlayerDieDic.Remove(playerinfo.roleId);
			}
		}
	}

	private void UpdateOnlineStatusEvent(long roleId, bool isOnline)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == roleId)
			{
				TriggerEvent();
				break;
			}
		}
	}

	private void OnSMakeMark(SMakeMark sMakeMark)
	{
		SingletonMono<AudioManager>.Ins.Play2D(353);
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == sMakeMark.roleId)
			{
				mTeamateInfo.markList.Clear();
				Vec3 vec = new Vec3();
				vec.x = sMakeMark.pos.x;
				vec.y = sMakeMark.pos.y;
				vec.z = sMakeMark.pos.z;
				mTeamateInfo.markList.Add(vec);
				TriggerEvent();
				break;
			}
		}
	}

	private void OnSRemoveMark(SRemoveMark sRemoveMark)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == sRemoveMark.roleId)
			{
				mTeamateInfo.markList.Clear();
				TriggerEvent();
				break;
			}
		}
	}

	private void OnSGetInVehicle(SGetInVehicle sGetInVehicle)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == sGetInVehicle.roleId)
			{
				mTeamateInfo.vehicleId = sGetInVehicle.id;
				TriggerEvent();
				break;
			}
		}
	}

	private void OnSGetOutVehicle(SGetOutVehicle sGetOutVehicle)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == sGetOutVehicle.roleId)
			{
				mTeamateInfo.vehicleId = 0L;
				TriggerEvent();
				break;
			}
		}
	}

	private void TriggerEvent()
	{
		if (BattleEvent.OnRefreshTeamateInfo != null)
		{
			BattleEvent.OnRefreshTeamateInfo();
		}
	}

	private int CompareTeamInfo(TeamateInfo info1, TeamateInfo info2)
	{
		if (info1.roleId == Singleton<TeamScMgr>.Ins.leaderId)
		{
			return -1;
		}
		return (int)(info1.roleId - info2.roleId);
	}

	public bool IsDead(long roleId)
	{
		foreach (TeamateInfo mTeamateInfo in _mTeamateInfoList)
		{
			if (mTeamateInfo.roleId == roleId)
			{
				return mTeamateInfo.isDead;
			}
		}
		return false;
	}

	public void RemoveTeamNumber(long roleId)
	{
		for (int i = 0; i < _mTeamateInfoList.Count; i++)
		{
			if (_mTeamateInfoList[i].roleId == roleId)
			{
				_mTeamateInfoList.RemoveAt(i);
				break;
			}
		}
		if (PlayerDieDic.ContainsKey(roleId))
		{
			PlayerDieDic.Remove(roleId);
		}
	}
}
