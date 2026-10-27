using gs.battle.scmsg;

public class CutPlantState : FSMState
{
	private long m_insId;

	public CutPlantState()
	{
		stateID = StateID.CutPlant;
	}

	public override void DoBeforeEntering(object[] args)
	{
		m_insId = (long)args[0];
		if (Battle.Ins.SelfPlayer.FSM.CurrentState.ID == StateID.Stand)
		{
			m_TargetAimatorName = "PickItem.kongshou_zhan_pick01";
		}
		else if (Battle.Ins.SelfPlayer.FSM.CurrentState.ID == StateID.Crouch)
		{
			m_TargetAimatorName = "PickItem.kongshou_dun_caiji";
		}
		else
		{
			m_TargetAimatorName = "PickItem.kongshou_pa_pick01";
		}
		Player.ChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName);
	}

	public override void DoBeforeLeaving()
	{
	}

	public override void Act()
	{
		Player.NeedHeadIK = false;
		Player.NeedAimIK = false;
	}

	public override void LateUpdate()
	{
	}

	public override void Reason()
	{
		if (Player.IsPlayFinish(m_TargetAimatorName))
		{
			CHarvest cHarvest = new CHarvest();
			cHarvest.instanceId = m_insId;
			Client2Gs.Ins.Send(cHarvest);
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
		}
	}

	private void PlayAnimation()
	{
	}
}
