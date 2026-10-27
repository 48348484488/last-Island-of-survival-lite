using UnityEngine;

public class LandState : FSMState
{
	public LandState()
	{
		stateID = StateID.Land;
	}

	public override void DoBeforeEntering(object[] args)
	{
		Player.MainCamera.ChangeState("Jump");
		UpdatePlayAimator();
		Player.PlayerAnimator.applyRootMotion = false;
		Player.ChangeVelocity(2f);
	}

	public override void DoBeforeLeaving()
	{
		StandState.StandCapsule(Player);
		Player.PlayerAnimator.applyRootMotion = true;
	}

	public override void Act()
	{
		Player.ChangeVelocity(2f);
		Player.PlayerRigidbody.AddForce(Physics.gravity * 5f);
	}

	public override void Reason()
	{
		float awayGroundDistance = Battle.Ins.GetAwayGroundDistance(new Vector3(Player.Pos.x, Player.Pos.y, Player.Pos.z));
		if (IsTargetAimatorPlayFinish() || awayGroundDistance < 0.1f)
		{
			Player.PlayerCollider.material = Battle.Ins.MaxPhysicMaterial;
			Player.FSM.SwitchState(StateID.Stand);
		}
	}

	private void UpdatePlayAimator()
	{
		if (Player.CurGun != null)
		{
			if (Player.CurGun.GunCfg.gunType == 5 || Player.CurGun.GunCfg.gunType == 3 || Player.CurGun.GunCfg.gunType == 6)
			{
				m_TargetAimatorName = "Land.jvjiqiang";
			}
			else if (Player.CurGun.GunCfg.gunType == 8)
			{
				m_TargetAimatorName = "Land.rpg";
			}
			else if (Player.CurGun.GunCfg.gunType == 4)
			{
				m_TargetAimatorName = "Land.sandanqiang";
			}
			else if (Player.CurGun.GunCfg.gunType == 1)
			{
				m_TargetAimatorName = "Land.kongshou";
			}
			else
			{
				m_TargetAimatorName = "Land.chongfengqiang";
			}
		}
		else
		{
			m_TargetAimatorName = "Land.kongshou";
		}
		if (Player.HaveNearWeaponInHand && Player.FSMUpBody.CurrentState.ID != StateID.Attack)
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
		}
		Player.ChangeAnimatorStates(Player.BaseLayer, m_TargetAimatorName);
	}
}
