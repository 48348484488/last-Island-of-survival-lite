using UnityEngine;

public class StandState : FSMState
{
	private string m_FrontOrBack = "f";

	private int m_startBackAngle = 130;

	private int m_endBackAngle = 230;

	public static bool StandRush;

	public static bool StandRun;

	private float m_speedUpNum;

	public StandState()
	{
		stateID = StateID.Stand;
	}

	public override void DoBeforeEntering(object[] args)
	{
		Player.InStartPlane = false;
		StandCapsule(Player);
		if (args.Length > 0)
		{
			Player.SetPosition((Vector3)args[0]);
		}
		if (Player.IsDownWaitSave)
		{
			Player.FSM.SwitchState(StateID.DownWaitSave);
			return;
		}
		Utils.TriggerEvent(BattleEvent.OnEnterStand);
		Player.MainCamera.SetMainTarget(Player.PlayerTransform, "Stand");
		Player.PlayerCollider.enabled = true;
		Player.PlayerRigidbody.useGravity = true;
		Player.RotateSpeed = 8f;
	}

	public override void DoBeforeLeaving()
	{
		ResetSkillSpeedUp();
	}

	public override void Act()
	{
		Player.SetAnimatorHorizontalValue(Player.InputVector.x);
		if (Player.GetAnimatorForwardValue() > 2f && Player.InputVector.y <= 2f)
		{
			Player.SetAnimatorForwardValue(Player.InputVector.y);
		}
		else
		{
			Player.SetAnimatorForwardValue(Player.InputVector.y);
		}
		SetSmallAngleToBackMove();
		if (Player.FSMUpBody.CurrentState.ID == StateID.Aim || Player.FSMUpBody.CurrentState.ID == StateID.Nawuqi || Player.IsJiMiao)
		{
			if (Player.ShootingTime > 0f || Player.IsJiMiao)
			{
				if (Player.IsMale)
				{
					Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.Stand");
				}
				else
				{
					Player.ChangeAnimatorStates(Player.BaseLayer, "StandWoman");
				}
				Player.NeedAimIK = true;
			}
			else
			{
				Player.NeedAimIK = false;
				if (Player.CurGun != null)
				{
					if (Player.CurGun.GunCfg.gunAimType == 5 || Player.CurGun.GunCfg.gunAimType == 3 || Player.CurGun.GunCfg.gunAimType == 6)
					{
						Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.jvjiqiang." + m_FrontOrBack);
					}
					else if (Player.CurGun.GunCfg.gunAimType == 8)
					{
						Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.rpg." + m_FrontOrBack);
					}
					else if (Player.CurGun.GunCfg.gunAimType == 4)
					{
						Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.sandanqiang." + m_FrontOrBack);
					}
					else if (Player.CurGun.GunCfg.gunAimType == 1)
					{
						if (Player.IsMale)
						{
							Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.shouqiang." + m_FrontOrBack);
						}
						else
						{
							Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.shouqiangWomen." + m_FrontOrBack);
						}
					}
					else
					{
						Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.chongfengqiang." + m_FrontOrBack);
					}
				}
			}
		}
		else if (Player.GetCurNearweapon() != null)
		{
			if (Player.IsMale)
			{
				Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.jinzhan." + m_FrontOrBack);
			}
			else
			{
				Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.jinzhanWomen." + m_FrontOrBack);
			}
		}
		else if (Player.FSMUpBody.CurrentState.ID == StateID.NullStateID || Player.FSMUpBody.CurrentState.ID == StateID.Shouwuqi || Player.Input.magnitude < 0.1f)
		{
			if (Player.IsMale)
			{
				Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.kongshou." + m_FrontOrBack);
			}
			else
			{
				Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.kongshouWomen." + m_FrontOrBack);
			}
		}
		else
		{
			if (Player.IsMale)
			{
				Player.ChangeAnimatorStates(Player.BaseLayer, "Stand.Stand");
			}
			else
			{
				Player.ChangeAnimatorStates(Player.BaseLayer, "StandWoman");
			}
			Player.SetRotation(Battle.Ins.MainCamera.SelfTransform);
		}
		Move();
		if (Player.IsKongshou)
		{
			if (m_speedUpNum == 0f)
			{
				SpeedUpSkill();
			}
		}
		else
		{
			ResetSkillSpeedUp();
		}
	}

	private void SetSmallAngleToBackMove()
	{
		float orientation = MathUtils.GetOrientation(Player.Input);
		if (orientation >= (float)m_startBackAngle && orientation <= (float)m_endBackAngle)
		{
			m_FrontOrBack = "b";
		}
		else
		{
			m_FrontOrBack = "f";
		}
		if (orientation > 90f && orientation < (float)m_startBackAngle)
		{
			Player.Input.y = 0f;
			Player.Input.x = 1f;
		}
		else if (orientation > (float)m_endBackAngle && orientation < 270f)
		{
			Player.Input.x = -1f;
			Player.Input.y = 0f;
		}
	}

	public override void Reason()
	{
	}

	public static void StandCapsule(PlayerController Player)
	{
		Player.PlayerCollider.height = 1.8f;
		Player.PlayerCollider.radius = 0.42f;
		Player.PlayerCollider.center = new Vector3(Player.PlayerCollider.center.x, 0.9f, Player.PlayerCollider.center.z);
		Player.PlayerCollider.direction = 1;
		Player.PlayerCollider.transform.rotation = Player.PlayerCollider.transform.rotation;
	}

	private void Move()
	{
		StandRush = false;
		StandRun = false;
		if ((double)Player.Input.magnitude > 0.917 && Player.m_CanRush)
		{
			StandRush = true;
			Player.m_SpeedMultiple = 3f;
			Player.YaoGanAngle = MathUtils.GetOrientation(Player.Input);
			if (Player.YaoGanAngle >= 0f && Player.YaoGanAngle <= 50f)
			{
				StandRush = true;
				Player.m_SpeedMultiple = 3f;
			}
			else if (Player.YaoGanAngle > 310f && Player.YaoGanAngle <= 360f)
			{
				StandRush = true;
				Player.m_SpeedMultiple = 3f;
			}
			else
			{
				Player.m_SpeedMultiple = 2f;
			}
		}
		else if (Player.Input.magnitude >= 0f)
		{
			StandRun = true;
			Player.m_SpeedMultiple = 2f;
		}
		Player.m_lastFrameFreeType = Player.FreeType;
		ChangeCameraState();
	}

	private void ChangeCameraState()
	{
		if (!Player.IsJiMiao)
		{
			if (StandRush || Player.AutoRun)
			{
				Player.MainCamera.ChangeState("StandRush");
			}
			else if (StandRun)
			{
				Player.MainCamera.ChangeState("Stand");
			}
		}
	}

	private void SpeedUpSkill()
	{
		m_speedUpNum = Player.RunSpeedUpSkill(107);
	}

	private void ResetSkillSpeedUp()
	{
		Player.ReduceAnimatorSpeed(m_speedUpNum);
		m_speedUpNum = 0f;
	}
}
