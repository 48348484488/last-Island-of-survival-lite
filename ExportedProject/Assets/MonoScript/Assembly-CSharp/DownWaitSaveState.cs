using System;
using System.Runtime.CompilerServices;
using gs.battle.scmsg;

public class DownWaitSaveState : FSMState
{
	private int m_AudioId;

	private bool m_CanPlayrWaitSaveSound = true;

	private float m_speedUpNum;

	public DownWaitSaveState()
	{
		stateID = StateID.DownWaitSave;
		SHpStatusChange.handler = (SHpStatusChange.Handler)Delegate.Combine(SHpStatusChange.handler, new SHpStatusChange.Handler(OnHpStatusChange));
		SSyncPlayerSave.handler = (SSyncPlayerSave.Handler)Delegate.Combine(SSyncPlayerSave.handler, new SSyncPlayerSave.Handler(OnSyncPlayerSave));
		SPlayerDie.handler = (SPlayerDie.Handler)Delegate.Combine(SPlayerDie.handler, new SPlayerDie.Handler(OnSPlayerDie));
		SBloodAirportPlayerDie.handler = (SBloodAirportPlayerDie.Handler)Delegate.Combine(SBloodAirportPlayerDie.handler, new SBloodAirportPlayerDie.Handler(OnBloodAirportPlayerDie));
	}

	private void OnBloodAirportPlayerDie(SBloodAirportPlayerDie msg)
	{
		CanBreak = true;
		SingletonMono<AudioManager>.Ins.StopMusic(m_AudioId);
	}

	private void OnSPlayerDie(SPlayerDie msg)
	{
		if (msg.roleId == Singleton<RoleMgr>.Ins.info.roleId)
		{
			CanBreak = true;
			SingletonMono<AudioManager>.Ins.StopMusic(m_AudioId);
		}
	}

	private void OnSyncPlayerSave(SSyncPlayerSave msg)
	{
		if (msg.roleId == Singleton<RoleMgr>.Ins.info.roleId)
		{
			CanBreak = true;
			Player.FSM.SwitchState(StateID.Crouch);
		}
	}

	private void OnHpStatusChange(SHpStatusChange msg)
	{
		if (!msg.isSecond)
		{
			return;
		}
		if (msg.roleId == Singleton<RoleMgr>.Ins.info.roleId)
		{
			if (Player.NearCar != null && Player.InCar)
			{
				Battle.Ins.MyBattlePanel.OnClickDownCar(null);
			}
			ThrowWeaponState.IsUpLei = true;
			Player.FSM.SwitchState(StateID.DownWaitSave);
			Battle.Ins.MyBattlePanel.ShowSaveSelfPanel();
			if (m_CanPlayrWaitSaveSound)
			{
				m_CanPlayrWaitSaveSound = false;
				if (Player.IsMale)
				{
					m_AudioId = SingletonMono<AudioManager>.Ins.Play2D(415);
				}
				else
				{
					m_AudioId = SingletonMono<AudioManager>.Ins.Play2D(412);
				}
				SingletonMono<TimerManager>.Ins.AddTimer(Player.RoleId + "CanPlayWaitSaveSound", 5f, _003COnHpStatusChange_003Em__0);
			}
		}
		else
		{
			if (Battle.Ins.TeamPlayerDic.ContainsKey(msg.roleId))
			{
				SingletonMono<AudioManager>.Ins.Play2D(348);
			}
			BasePlayerController player = Battle.Ins.GetPlayer(msg.roleId);
			player.PlayerAnimation(player.BaseLayer, "DownWaitSave.waitsave");
		}
	}

	public override void DoBeforeEntering(object[] args)
	{
		if (Battle.Ins.SelfPlayer.CurGun != null)
		{
			Battle.Ins.SelfPlayer.CurGun.StopShoot();
		}
		Player.ChangeAnimatorStates(Player.BaseLayer, "DownWaitSave.waitsave");
		Player.DisEnableUpBodyFsm();
		Player.MainCamera.ChangeState("Crouch");
		CanBreak = false;
		CrouchState.CrouchCapsule(Player);
		Player.FreeType = true;
		Player.m_SpeedMultiple = 1f;
		Player.ChangeHandWeapon(-1);
		SpeedUpSkill();
		Utils.TriggerEvent(BattleEvent.OnSelfDown, true);
	}

	public override void DoBeforeLeaving()
	{
		Player.FreeType = false;
		SingletonMono<AudioManager>.Ins.StopMusic(m_AudioId);
		ResetSkillSpeedUp();
		Player.IsDownWaitSave = false;
		Utils.TriggerEvent(BattleEvent.OnSelfDown, false);
	}

	public override void Act()
	{
		Player.FSMUpBody.SwitchState(StateID.NullStateID);
		Player.CanOpenJimiao = false;
		Player.SetAnimatorHorizontalValue(Player.InputVector.x);
		Player.SetAnimatorForwardValue(Player.InputVector.y);
		Player.CloseAllIK();
	}

	public override void Reason()
	{
	}

	private void SpeedUpSkill()
	{
		m_speedUpNum = Player.RunSpeedUpSkill(103);
	}

	private void ResetSkillSpeedUp()
	{
		Player.ReduceAnimatorSpeed(m_speedUpNum);
	}

	[CompilerGenerated]
	private void _003COnHpStatusChange_003Em__0()
	{
		m_CanPlayrWaitSaveSound = true;
	}
}
