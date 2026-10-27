using UnityEngine;
using gs.battle.scmsg;

public class PlantInfo : MapObject
{
	public SPlantInfo MySPlantInfo;

	public int GrowFinishTime;

	protected override void Awake()
	{
		base.Awake();
		GetComponentInChildren<Collider>().gameObject.layer = 17;
	}

	public void Caiji()
	{
		if (Battle.Ins.SelfPlayer.FSM.CurrentState.ID == StateID.Stand || Battle.Ins.SelfPlayer.FSM.CurrentState.ID == StateID.Crouch)
		{
			Battle.Ins.SelfPlayer.FSMUpBody.SwitchState(StateID.CutPlant, InsId);
		}
	}
}
