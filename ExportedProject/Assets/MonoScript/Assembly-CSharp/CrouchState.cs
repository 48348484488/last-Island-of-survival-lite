using UnityEngine;

public class CrouchState : FSMState
{
	public static bool CrouchRush;

	public static bool CrouchRun;

	public static bool CrouchWalk;

	private float m_speedUpNum;

	public CrouchState()
	{
		stateID = StateID.Crouch;
	}

	public override void DoBeforeEntering(object[] args)
	{
		Player.MainCamera.ChangeState("Crouch");
		Player.FixedChangeAnimatorStates(Player.BaseLayer, "Crouch");
		Player.RotateSpeed = 10f;
		CrouchCapsule(Player);
		Utils.TriggerEvent(BattleEvent.OnEnterCrouch);
		SpeedUpSkill();
	}

	public override void DoBeforeLeaving()
	{
		Player.SetAnimatorHorizontalValue(0f);
		Player.SetAnimatorForwardValue(0f);
		ResetSkillSpeedUp();
	}

	public override void Act()
	{
		Move();
		Player.SetAnimatorHorizontalValue(Player.InputVector.x);
		if (Player.GetAnimatorForwardValue() > 2f && Player.InputVector.y <= 2f)
		{
			Player.SetAnimatorForwardValue(Player.InputVector.y);
		}
		else
		{
			Player.SetAnimatorForwardValue(Player.InputVector.y);
		}
	}

	public override void Reason()
	{
	}

	public static void CrouchCapsule(PlayerController Player)
	{
		Player.PlayerCollider.height = 1.5f;
		Player.PlayerCollider.center = new Vector3(Player.PlayerCollider.center.x, 0.73f, Player.PlayerCollider.center.z);
		Player.PlayerCollider.direction = 1;
		Player.PlayerCollider.transform.rotation = Player.PlayerCollider.transform.rotation;
	}

	private void Move()
	{
		Player.FreeType = false;
		CrouchRush = false;
		CrouchRun = false;
		CrouchWalk = false;
		if ((double)Player.Input.magnitude > 0.917 && Player.m_CanRush)
		{
			if (Player.FSMUpBody.CurrentState.ID == StateID.Aim || Player.FSMUpBody.CurrentState.ID == StateID.HoldNearWeaponState || Player.FSMUpBody.CurrentState.ID == StateID.NullStateID)
			{
				Player.YaoGanAngle = MathUtils.GetOrientation(Player.Input);
				if (Player.YaoGanAngle >= 0f && Player.YaoGanAngle <= 50f)
				{
					Player.m_SpeedMultiple = 3f;
					CrouchRush = true;
				}
				else if (Player.YaoGanAngle > 310f && Player.YaoGanAngle <= 360f)
				{
					Player.m_SpeedMultiple = 3f;
					CrouchRush = true;
				}
				else
				{
					Player.m_SpeedMultiple = 2f;
				}
			}
		}
		else if ((double)Player.Input.magnitude > 0.33)
		{
			CrouchRun = true;
			if (Player.m_CanRush)
			{
				Player.m_SpeedMultiple = 3f;
			}
			else
			{
				Player.m_SpeedMultiple = 2f * Player.Input.magnitude;
			}
		}
		else
		{
			CrouchWalk = true;
			if (Player.m_CanRush)
			{
				Player.m_SpeedMultiple = 3f;
			}
			else
			{
				Player.m_SpeedMultiple = 1f * Player.Input.magnitude;
			}
		}
		Player.m_lastFrameFreeType = Player.FreeType;
		ChangeCameraState();
	}

	private void ChangeCameraState()
	{
		if (!Player.IsJiMiao)
		{
			if (CrouchRush || Player.AutoRun)
			{
				Player.MainCamera.SetMainTarget(Player.PlayerTransform, "CrouchRush");
			}
			else if (CrouchRun)
			{
				Player.MainCamera.SetMainTarget(Player.PlayerTransform, "CrouchRun");
			}
			else if (CrouchWalk)
			{
				Player.MainCamera.SetMainTarget(Player.PlayerTransform, "CrouchWalk");
			}
		}
	}

	private void SpeedUpSkill()
	{
		m_speedUpNum = Player.RunSpeedUpSkill(104);
	}

	private void ResetSkillSpeedUp()
	{
		Player.ReduceAnimatorSpeed(m_speedUpNum);
		m_speedUpNum = 0f;
	}
}
