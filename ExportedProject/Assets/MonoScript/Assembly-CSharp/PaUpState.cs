public class PaUpState : FSMState
{
	private bool m_CanExit;

	private bool m_ToStand;

	public PaUpState()
	{
		stateID = StateID.PaUp;
	}

	public override void DoBeforeEntering(object[] args)
	{
		m_CanExit = false;
		m_ToStand = false;
		if (args.Length > 0)
		{
			m_ToStand = (bool)args[0];
		}
		if (m_ToStand)
		{
			Player.MainCamera.ChangeState("Stand", true, 1.5f);
		}
		else
		{
			Player.MainCamera.ChangeState("Crouch");
		}
		Player.DisEnableUpBodyFsm();
		Player.FSMUpBody.SwitchState(StateID.NullStateID);
		UpdatePlayAimator();
	}

	public override void DoBeforeLeaving()
	{
		Player.EnableUpBodyFsm();
	}

	public override void Act()
	{
		Player.CloseAllIK();
	}

	public override void Reason()
	{
		if (IsTargetAimatorPlayFinish())
		{
			if (m_ToStand)
			{
				Player.FSM.SwitchState(StateID.Stand);
			}
			else
			{
				Player.FSM.SwitchState(StateID.Crouch);
			}
			Player.EnableUpBodyFsm();
			if (Player.CurGun == null)
			{
				Player.FSMUpBody.SwitchState(StateID.NullStateID);
			}
			else
			{
				Player.FSMUpBody.SwitchState(StateID.Aim);
			}
		}
		Player.NeedHeadIK = false;
	}

	private void UpdatePlayAimator()
	{
		if (Player.CurGun == null)
		{
			m_TargetAimatorName = "PaUp.PaUp_KongShou";
		}
		else
		{
			m_TargetAimatorName = "PaUp.PaUp_chongfengqiang_hengwo";
		}
		if (m_ToStand)
		{
			if (Player.CurGun != null)
			{
				m_TargetAimatorName = "PaUp.chiqiang_pa_qiezhan01";
			}
			else
			{
				m_TargetAimatorName = "PaUp.kongshou_pa_qiezhan01";
			}
		}
		Player.ChangeAnimatorStates(Player.BaseLayer, m_TargetAimatorName);
	}
}
