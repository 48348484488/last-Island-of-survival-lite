using System.Collections;
using UnityEngine;

public class MonsterDefenceState : FSMState
{
	private IEnterAttackStrategy m_attackStrategy;

	public MonsterDefenceState()
	{
		stateID = StateID.MonsterDefence;
		SetEnterAttackStrategy(new ActiveAttackStrategy());
	}

	public override void DoBeforeEntering(object[] args)
	{
		if (Monster.MyGunCfg != null)
		{
			Monster.PlayHaveGunAim();
		}
		Monster.ResetRevengeTime();
	}

	public void SetEnterAttackStrategy(IEnterAttackStrategy attackStrategy)
	{
		m_attackStrategy = attackStrategy;
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds(1f);
	}

	public override void Act()
	{
		if (Monster.Target != null)
		{
			Monster.SetTargetRotation(Quaternion.LookRotation(Monster.Target.transform.position - Monster.transform.position, Vector3.up));
		}
	}

	public override void Reason()
	{
		if (Monster.DisToBirthPos > Monster.FollowRange || Monster.Target == null)
		{
			Monster.FSM.SwitchState(StateID.MonsterGoBitrh);
		}
		else if (m_attackStrategy.CanEnterAttackState(Monster))
		{
			Monster.Stop();
			Monster.FSM.SwitchState(StateID.MonsterAttack);
		}
		else if (Monster.MyGunCfg == null)
		{
			if (Monster.DisToTarget > Monster.AttackRange)
			{
				Monster.PlayRun();
			}
			else
			{
				Monster.ChangeAnimatorStates("waring");
			}
		}
	}

	public override void DoBeforeLeaving()
	{
	}
}
