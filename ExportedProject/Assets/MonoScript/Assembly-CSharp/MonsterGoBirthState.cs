using System.Collections;
using UnityEngine;

public class MonsterGoBirthState : FSMState
{
	public MonsterGoBirthState()
	{
		stateID = StateID.MonsterGoBitrh;
	}

	public override void DoBeforeEntering(object[] args)
	{
		if (Monster.Target == null)
		{
			Monster.PlayWalk();
		}
		else
		{
			Monster.PlayRun();
		}
		Monster.SetTarget(null);
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds(10f);
		Monster.BirthPos = Monster.transform.position;
		Monster.FSM.SwitchState(StateID.MonsterFreeMove);
	}

	public override void Act()
	{
		Monster.SetTargetRotation(Quaternion.LookRotation(Monster.BirthPos - Monster.transform.position, Vector3.up));
	}

	public override void Reason()
	{
		if (Monster.DisToBirthPos <= 3f)
		{
			Monster.FSM.SwitchState(StateID.MonsterFreeMove);
		}
	}

	public override void DoBeforeLeaving()
	{
	}
}
