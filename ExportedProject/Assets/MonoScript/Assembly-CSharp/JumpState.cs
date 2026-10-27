using System;
using System.Collections;
using UnityEngine;
using cfg;

public class JumpState : FSMState
{
	private bool m_CanExit;

	private bool m_Attacked;

	private Vector3 playerForward;

	public JumpState()
	{
		stateID = StateID.Jump;
		BattleEvent.OnAttack = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnAttack, new Utils.VoidDelegate(OnAttack));
		ResMgr.Ins.LoadAB("sound/" + SoundCfg.Get(331).path + ".ab", null, false);
		ResMgr.Ins.LoadAB("sound/" + SoundCfg.Get(330).path + ".ab", null, false);
	}

	private void OnAttack()
	{
		m_Attacked = true;
	}

	public override void DoBeforeEntering(object[] args)
	{
		m_Attacked = false;
		m_CanExit = false;
		Player.PlayerAnimator.applyRootMotion = false;
		Player.ChangeVelocity(2f);
		UpdatePlayAimator();
		if (Player.CurGun == null)
		{
			Player.DisEnableUpBodyFsm();
		}
	}

	public override void DoBeforeLeaving()
	{
		Player.PlayerAnimator.applyRootMotion = true;
		Player.EnableUpBodyFsm();
		m_Attacked = false;
	}

	public override void Act()
	{
		Player.ChangeVelocity(2f);
		if (Player.InputVector.y > 2f)
		{
			Player.SetAnimatorForwardValue(2f);
		}
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		m_Attacked = false;
	}

	public override void Reason()
	{
		if (Player.IsPlayFinish(m_TargetAimatorName) && !m_Attacked)
		{
			if (Player.GroundDistance < 2f)
			{
				Player.FSM.SwitchState(StateID.Land);
				return;
			}
			Player.FSM.SwitchState(StateID.Fall, true);
		}
	}

	private void UpdatePlayAimator()
	{
		Player.ChangeYVelocity(5f);
		Player.MainCamera.ChangeState("Jump");
		if (Player.CurGun != null)
		{
			if (Player.CurGun.GunCfg.gunType == 5 || Player.CurGun.GunCfg.gunType == 3 || Player.CurGun.GunCfg.gunType == 6)
			{
				m_TargetAimatorName = "Jump.jvjiqiang";
			}
			else if (Player.CurGun.GunCfg.gunType == 8)
			{
				m_TargetAimatorName = "Jump.rpg";
			}
			else if (Player.CurGun.GunCfg.gunType == 4)
			{
				m_TargetAimatorName = "Jump.sandanqiang";
			}
			else if (Player.CurGun.GunCfg.gunType == 1)
			{
				m_TargetAimatorName = "Jump.kongshou";
			}
			else
			{
				m_TargetAimatorName = "Jump.chongfengqiang";
			}
		}
		else
		{
			m_TargetAimatorName = "Jump.kongshou";
		}
		Player.ChangeAnimatorStates(Player.BaseLayer, m_TargetAimatorName);
	}

	public static void JumpCapsule(PlayerController Player)
	{
		float num = Math.Min(Player.LeftFoot.position.y, Player.LeftFoot.position.y);
		float num2 = Math.Abs(Player.HeadTop.transform.position.y - num);
		Player.PlayerCollider.height = 0.5f;
		Player.PlayerCollider.center = new Vector3(Player.PlayerCollider.center.x, 1.2f, Player.PlayerCollider.center.z);
		Player.PlayerCollider.transform.rotation = Player.transform.rotation;
	}
}
