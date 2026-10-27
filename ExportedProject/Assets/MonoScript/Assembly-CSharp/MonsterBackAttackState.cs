using System.Collections;
using UnityEngine;

public class MonsterBackAttackState : FSMState
{
	public MonsterBackAttackState()
	{
		stateID = StateID.MonsterBackAttack;
	}

	public override void DoBeforeEntering(object[] args)
	{
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds(1f);
	}

	public override void Act()
	{
		Monster.SetTargetRotation(Quaternion.LookRotation(Monster.Target.transform.position - Monster.transform.position, Vector3.up));
	}

	public override void Reason()
	{
		if (!IsTargetAimatorPlayFinish())
		{
		}
	}

	public override void DoBeforeLeaving()
	{
	}
}
