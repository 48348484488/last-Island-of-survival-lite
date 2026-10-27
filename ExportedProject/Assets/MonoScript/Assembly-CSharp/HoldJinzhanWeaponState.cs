using System;

public class HoldJinzhanWeaponState : FSMState
{
	private bool Fast;

	private bool m_NeedfullbodyFsm;

	private float m_speedUpNum;

	public HoldJinzhanWeaponState()
	{
		stateID = StateID.HoldNearWeaponState;
	}

	public override void Init()
	{
		FSMSystem fSMSystem = fsm;
		fSMSystem.Ticker = (Utils.VoidDelegate)Delegate.Combine(fSMSystem.Ticker, new Utils.VoidDelegate(OnTick));
	}

	private void OnTick()
	{
		CheckEnterHoldJinZhanWeaponState();
	}

	public override void DoBeforeEntering(object[] args)
	{
		if (args.Length > 0)
		{
			Fast = (bool)args[0];
		}
		m_NeedfullbodyFsm = false;
		Player.ChangeAnimatorStates(Player.UpperBodyLayer, "Null");
		SpeedUpSkill();
	}

	public override void DoBeforeLeaving()
	{
		Player.DisEnableFullBodyMask();
		ResetSkillSpeedUp();
	}

	public override void Act()
	{
		if (Player.HaveNearWeaponInHand)
		{
			UpdateJinzhanAnimatorName();
		}
		Player.CloseAllIK();
	}

	public override void Reason()
	{
	}

	private void UpdateJinzhanAnimatorName()
	{
		if (Player.FSM.CurrentState.ID == StateID.Pa)
		{
			m_TargetAimatorName = "HoldJinzhanWeapon.Pa";
			m_NeedfullbodyFsm = true;
		}
		else if (Player.FSM.CurrentState.ID == StateID.Stand)
		{
			if ((StandState.StandRush || Player.AutoRun) && Player.Input.y > 0f)
			{
				m_TargetAimatorName = "HoldJinzhanWeapon.dao_zhan_rush_front01";
				m_NeedfullbodyFsm = true;
			}
			else
			{
				m_NeedfullbodyFsm = false;
			}
		}
		else if (Player.FSM.CurrentState.ID == StateID.Crouch)
		{
			if (CrouchState.CrouchRush || Player.AutoRun)
			{
				m_TargetAimatorName = "HoldJinzhanWeapon.dao_dun_rush_front01";
				m_NeedfullbodyFsm = true;
			}
			else
			{
				m_NeedfullbodyFsm = false;
			}
		}
		else
		{
			m_NeedfullbodyFsm = false;
		}
		if (m_NeedfullbodyFsm)
		{
			Player.DisEnableUpBodyFsm();
			if (Fast)
			{
				Player.FastChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName);
			}
			else
			{
				Player.ChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName);
			}
		}
		else
		{
			Player.DisEnableFullBodyMask();
		}
	}

	private void CheckEnterHoldJinZhanWeaponState()
	{
		if (Player.HaveNearWeaponInHand && Player.FSMUpBody.CurrentState.ID == StateID.NullStateID && Player.FSM.CurrentState.ID != StateID.Cross && Player.FSM.CurrentState.ID != StateID.CrouchDown && Player.FSM.CurrentState.ID != StateID.PaUp && Player.FSM.CurrentState.ID != StateID.Jump && Player.FSM.CurrentState.ID != StateID.Land && Player.FSM.CurrentState.ID != StateID.Fall && Player.FSM.CurrentState.ID != StateID.SavePeople && Player.FSM.CurrentState.ID != StateID.DownWaitSave)
		{
			Player.FSMUpBody.SwitchState(StateID.HoldNearWeaponState);
		}
	}

	private void SpeedUpSkill()
	{
		m_speedUpNum = Player.RunSpeedUpSkill(106);
	}

	private void ResetSkillSpeedUp()
	{
		Player.ReduceAnimatorSpeed(m_speedUpNum);
		m_speedUpNum = 0f;
	}
}
