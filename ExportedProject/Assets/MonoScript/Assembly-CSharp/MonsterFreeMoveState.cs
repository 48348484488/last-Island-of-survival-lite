using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MonsterFreeMoveState : FSMState
{
	private Vector3 m_TargetPosition;

	private float m_Range = 3f;

	private float m_ForwordValue = 1f;

	private float m_HorizontalValue = 1f;

	private float m_MoveTickTime = 0.1f;

	private Quaternion m_wantRotate;

	private float m_forceRunTime;

	private float m_nextHumanMonsterFreeAim = 9f;

	public float[] m_actionWeight = new float[3] { 30f, 30f, 40f };

	public MonsterFreeMoveState()
	{
		stateID = StateID.MonsterFreeMove;
	}

	public override void DoBeforeEntering(object[] args)
	{
		Monster.PlayHaveGunAim();
	}

	public override void DoBeforeLeaving()
	{
	}

	public override void Act()
	{
		if (m_forceRunTime > 0f)
		{
			m_forceRunTime -= Time.deltaTime;
		}
		if (m_nextHumanMonsterFreeAim > 0f)
		{
			m_nextHumanMonsterFreeAim -= Time.deltaTime;
		}
	}

	public override IEnumerator ActCoroutine()
	{
		while (true)
		{
			if (m_forceRunTime <= 0f)
			{
				RandomAct();
			}
			yield return new WaitForSeconds(3f);
		}
	}

	private void PlayHumanMonsterFree()
	{
		if (Monster.MyGunCfg == null)
		{
		}
	}

	private void RandomAct()
	{
		float num = Random.Range(0f, m_actionWeight[0] + m_actionWeight[1] + m_actionWeight[2]);
		if (num <= m_actionWeight[0])
		{
			Monster.Stop();
			if (Monster.MyGunCfg == null)
			{
				Monster.ChangeAnimatorStates("idle");
			}
			else
			{
				Monster.SetAnimatorForwardValue(0f);
				string aniName = Monster.PlayHumanMonsterFree();
				Utils.StartConroutine(Utils.YieldAniFinish(Monster.MyAnimator, aniName, _003CRandomAct_003Em__0));
			}
		}
		else if (m_actionWeight[0] < num && num <= m_actionWeight[0] + m_actionWeight[1])
		{
			Monster.Stop();
			if (Monster.MyGunCfg == null)
			{
				Monster.ChangeAnimatorStates("free");
				Battle.Ins.SyncPlaySound(Monster.MyCfg.freeSound, Monster.transform.position);
			}
			else
			{
				Monster.PlayWalk();
			}
		}
		if (m_actionWeight[0] + m_actionWeight[1] < num && num <= m_actionWeight[0] + m_actionWeight[1] + m_actionWeight[2])
		{
			Monster.PlayWalk();
			Quaternion targetRotation = Quaternion.Euler(0f, Random.Range(0, 180), 0f);
			Monster.SetTargetRotation(targetRotation);
		}
	}

	public override void Reason()
	{
		if (Monster.DisToBirthPos > Monster.FreeMoveRange && Monster.MyCfg.canMove)
		{
			Monster.FSM.SwitchState(StateID.MonsterGoBitrh);
		}
		if (Monster.Target != null && Monster.MyCfg.type != 3 && Monster.MyCfg.type != 4 && Monster.DisToTarget <= Monster.DefenceRange && Monster.CanSeeTarget())
		{
			Monster.FSM.SwitchState(StateID.MonsterDefence);
		}
	}

	public void SetForceRun(float forceRunTime)
	{
		m_forceRunTime = forceRunTime;
		Monster.PlayRun();
		Quaternion targetRotation = Quaternion.Euler(0f, Random.Range(0, 180), 0f);
		Monster.SetTargetRotation(targetRotation);
	}

	[CompilerGenerated]
	private void _003CRandomAct_003Em__0()
	{
		Monster.ChangeAnimatorStates("Null", 1);
	}
}
