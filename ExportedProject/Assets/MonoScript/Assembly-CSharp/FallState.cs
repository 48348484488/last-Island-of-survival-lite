using System;
using System.Collections;
using UnityEngine;

public class FallState : FSMState
{
	private bool m_SetYSpeedZero;

	private float m_Height;

	private bool m_FromJump;

	private float m_FallTime;

	public FallState()
	{
		stateID = StateID.Fall;
	}

	public override void Init()
	{
		FSMSystem fSMSystem = fsm;
		fSMSystem.Ticker = (Utils.VoidDelegate)Delegate.Combine(fSMSystem.Ticker, new Utils.VoidDelegate(OnTick));
	}

	private void OnTick()
	{
		CheckEnterFall();
	}

	public override void DoBeforeEntering(object[] args)
	{
		if (args.Length > 0)
		{
			m_FromJump = true;
		}
		else
		{
			m_FromJump = false;
		}
		Player.LastJumpUpInput = Player.Input;
		m_FallTime = 0f;
		m_Height = Player.Pos.y;
		Player.PlayerRigidbody.useGravity = true;
		Player.PlayerAnimator.applyRootMotion = false;
		m_SetYSpeedZero = false;
		UpdatePlayAimator();
	}

	public override void DoBeforeLeaving()
	{
		Player.PlayerAnimator.applyRootMotion = true;
		if ((double)SwimState.WaterDeep < 1.3)
		{
			if (Battle.Ins.IsOnHaiDi(Player.RootBone.position))
			{
				return;
			}
			m_Height -= Player.Pos.y;
			if (!(m_Height > 30f) && !(m_Height > 15f) && !(m_Height > 8f))
			{
			}
		}
		Player.CanShoot = true;
	}

	public override void Act()
	{
		m_FallTime += Time.deltaTime;
		if (m_FallTime > 12f)
		{
			Player.SetPosition(Battle.Ins.GetGroundPos(Player.Pos));
		}
		if (Player.InputVector.y > 2f)
		{
			Player.SetAnimatorForwardValue(2f);
		}
		if (Player.HaveNearWeaponInHand && Player.FSMUpBody.CurrentState.ID != StateID.Attack)
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
		}
		Player.ChangeVelocity(1.2f);
		if (m_SetYSpeedZero && Player.PlayerRigidbody.velocity.y > 0f)
		{
			Player.PlayerRigidbody.velocity = new Vector3(Player.Input.x, -20f, Player.Input.y);
		}
		Player.PlayerRigidbody.AddForce(Physics.gravity * 50f);
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds(0.6f);
		m_SetYSpeedZero = true;
	}

	public override void Reason()
	{
		float awayGroundDistance = Battle.Ins.GetAwayGroundDistance(new Vector3(Player.Pos.x, Player.Pos.y, Player.Pos.z));
		if (awayGroundDistance != 10000f && awayGroundDistance > 2.5f)
		{
			Player.CanShoot = false;
		}
		if (awayGroundDistance < 0.6f)
		{
			Player.FSM.SwitchState(StateID.Land);
			Player.PlayerRigidbody.velocity = new Vector3(Player.PlayerRigidbody.velocity.x, -1f, Player.PlayerRigidbody.velocity.z);
		}
	}

	private void UpdatePlayAimator()
	{
		if (Player.CurGun != null)
		{
			if (Player.CurGun.GunCfg.gunType == 5 || Player.CurGun.GunCfg.gunType == 3 || Player.CurGun.GunCfg.gunType == 6)
			{
				m_TargetAimatorName = "Fall.jvjiqiang";
			}
			else if (Player.CurGun.GunCfg.gunType == 8)
			{
				m_TargetAimatorName = "Fall.rpg";
			}
			else if (Player.CurGun.GunCfg.gunType == 4)
			{
				m_TargetAimatorName = "Fall.sandanqiang";
			}
			else if (Player.CurGun.GunCfg.gunType == 1)
			{
				m_TargetAimatorName = "Fall.kongshou";
			}
			else
			{
				m_TargetAimatorName = "Fall.chongfengqiang";
			}
		}
		else
		{
			m_TargetAimatorName = "Fall.kongshou";
		}
		Player.ChangeAnimatorStates(Player.BaseLayer, m_TargetAimatorName);
	}

	private void CheckEnterFall()
	{
		if (Player.GroundDistance > 2f && !Player.InCar && !Player.Climbing && Player.FSM.CurrentState.ID != StateID.Cross && Player.FSM.CurrentState.ID != StateID.Swim && !Player.Skydiving && Player.FSM.CurrentState.ID != StateID.InStartPlane && Player.FSM.CurrentState.ID != StateID.Jump)
		{
			Player.FSM.SwitchState(StateID.Fall);
		}
	}

	private float ReduceDamgeSkill()
	{
		return Player.GetSkillValueIndex0(112);
	}
}
