public class CrouchDownState : FSMState
{
	private bool m_CanExit;

	public CrouchDownState()
	{
		stateID = StateID.CrouchDown;
	}

	public override void DoBeforeEntering(object[] args)
	{
		m_CanExit = false;
		Player.MainCamera.ChangeState("Pa");
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
		if (Player.IsPlayFinish(m_TargetAimatorName))
		{
			Player.FSM.SwitchState(StateID.Pa);
			Player.EnableUpBodyFsm();
			if (Player.CurGun == null)
			{
				Player.FSMUpBody.SwitchState(StateID.NullStateID);
			}
		}
	}

	private void UpdatePlayAimator()
	{
		if (Player.CurGun == null)
		{
			m_TargetAimatorName = "CrouchDown.kongshou";
		}
		else
		{
			m_TargetAimatorName = "CrouchDown.chongfengqiang_hengwo";
		}
		Player.ChangeAnimatorStates(Player.BaseLayer, m_TargetAimatorName);
	}
}
