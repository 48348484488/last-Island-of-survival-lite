public class NullState : FSMState
{
	public NullState()
	{
		stateID = StateID.NullStateID;
	}

	public override void DoBeforeEntering(object[] args)
	{
		Player.DisEnableUpBodyFsm();
		Player.DisEnableFullBodyMask();
		Player.FastChangeAnimatorStates(Player.UpperBodyLayer, "Null");
		if (Player.CurGun != null)
		{
			Player.CurGun.CloseJiMiao();
		}
	}

	public override void DoBeforeLeaving()
	{
	}

	public override void Act()
	{
	}

	public override void Reason()
	{
	}
}
