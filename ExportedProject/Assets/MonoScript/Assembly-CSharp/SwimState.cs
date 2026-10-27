using System;
using System.Collections;
using UnityEngine;
using gs.battle.scmsg;

public class SwimState : FSMState
{
	private float PulmonaryOnSecond;

	private bool m_UnderWater;

	private bool m_UpWater;

	private Vector3 m_LastFramInput = Vector3.zero;

	public static float WaterDeep;

	private float m_speedUpNum;

	public SwimState()
	{
		stateID = StateID.Swim;
		PulmonaryOnSecond = 2.85f;
	}

	public override void Init()
	{
		FSMSystem fSMSystem = fsm;
		fSMSystem.Ticker = (Utils.VoidDelegate)Delegate.Combine(fSMSystem.Ticker, new Utils.VoidDelegate(OnTick));
	}

	private void OnTick()
	{
		Battle.Ins.CameraInWater();
	}

	public override void DoBeforeEntering(object[] args)
	{
		Utils.TriggerEvent(BattleEvent.OnExitedBuildState);
		Player.FSMUpBody.SwitchState(StateID.NullStateID);
		Player.SetPulmonary(100f);
		Player.m_SpeedMultiple = 1f;
		Player.PlayerRigidbody.useGravity = false;
		Battle.Ins.MainCamera.ChangeState("Swim");
		Player.FreeType = true;
		m_TargetAimatorName = "Swim.upwater";
		Player.transform.SetPositionY(Battle.Ins.WaterSurfaceHeight - 0.1f);
		Player.ChangeAnimatorStates(Player.BaseLayer, m_TargetAimatorName);
		Player.PlayChangeWeapen(-1);
		Player.UnLockRigidBody();
		Player.RotateSpeed = 5f;
		Player.Swiming = true;
		m_UnderWater = false;
		m_UpWater = true;
		SpeedUpSkill();
	}

	public override void DoBeforeLeaving()
	{
		Player.Swiming = false;
		Player.transform.eulerAngles = new Vector3(0f, Player.transform.eulerAngles.y, Player.transform.eulerAngles.z);
		Player.PlayerRigidbody.useGravity = true;
		Player.SetAnimatorHorizontalValue(0f);
		Player.SetAnimatorForwardValue(0f);
		Player.FreeType = false;
		Battle.Ins.MyBattlePanel.HideSwimPanel();
		Battle.Ins.MyBattlePanel.HidePulmonaryPanel();
		Player.SetPulmonary(100f);
		Player.RotateSpeed = 5f;
		ResetSkillSpeedUp();
	}

	public override void Act()
	{
		if (Battle.Ins.MyBattlePanel != null)
		{
			Battle.Ins.MyBattlePanel.ShowSwimPanel();
		}
		if ((double)Player.InputVector.y > 0.1)
		{
			SwimMoveCapsule();
		}
		else
		{
			SwimIdleCapsule();
		}
		Player.CloseAllIK();
		Player.PlayerRigidbody.velocity = new Vector3(Player.PlayerRigidbody.velocity.x, 0f, Player.PlayerRigidbody.velocity.z);
		if (Player.Pos.y > Battle.Ins.WaterSurfaceHeight - 0.1f)
		{
			Player.transform.SetPositionY(Battle.Ins.WaterSurfaceHeight - 0.1f);
		}
		Player.SetAnimatorHorizontalValue(Player.InputVector.x);
		Player.SetAnimatorForwardValue(Player.InputVector.y);
		m_LastFramInput = Player.InputVector;
		UpdateTargetAimName();
	}

	public override void Reason()
	{
		if (WaterDeep < 0.5f && Battle.Ins.GetAwayGroundDistance(Player.Pos) < 1f)
		{
			Player.FSM.SwitchState(StateID.Stand);
		}
	}

	public override IEnumerator ActCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			if (m_UnderWater)
			{
				PulmonaryOnSecond = 2.85f * (1f - AddInWaterTime());
				Player.SetPulmonary((float)Player.Pulmonary - PulmonaryOnSecond);
			}
			else if (m_UpWater)
			{
				PulmonaryOnSecond = 20f * (1f + AddO2());
				Player.SetPulmonary((int)((float)Player.Pulmonary + PulmonaryOnSecond));
			}
			if (Player.IsDownWaitSave)
			{
				Player.SetPulmonary(0f);
			}
			if (Player.Pulmonary >= 100)
			{
				if (Battle.Ins.MyBattlePanel != null)
				{
					Battle.Ins.MyBattlePanel.HidePulmonaryPanel();
				}
			}
			else if (Battle.Ins.MyBattlePanel != null)
			{
				if (m_UnderWater)
				{
					Battle.Ins.MyBattlePanel.UpdatePulmonaryPanel((float)Player.Pulmonary * 0.01f, (int)((float)Player.Pulmonary / PulmonaryOnSecond));
				}
				else
				{
					Battle.Ins.MyBattlePanel.UpdatePulmonaryPanel((float)Player.Pulmonary * 0.01f, (int)(5f - (float)Player.Pulmonary / PulmonaryOnSecond));
				}
			}
			if (Player.Pulmonary <= 0)
			{
				CHitSelft cHitSelft = new CHitSelft();
				cHitSelft.hitType = 10;
				cHitSelft.damage = 10;
				Client2Gs.Ins.Send(cHitSelft);
			}
		}
	}

	public void UpdateTargetAimName()
	{
		if ((double)Player.HeadTop.position.y > (double)Battle.Ins.WaterSurfaceHeight - 0.1 && !m_UpWater)
		{
			m_TargetAimatorName = "Swim.upwater";
			m_UpWater = true;
			m_UnderWater = false;
		}
		else if (Player.HeadTop.position.y < Battle.Ins.WaterSurfaceHeight - 0.3f && m_UpWater)
		{
			m_TargetAimatorName = "Swim.underwater";
			m_UpWater = false;
			m_UnderWater = true;
		}
		Player.ChangeAnimatorStates(Player.BaseLayer, m_TargetAimatorName);
	}

	public void SwimIdleCapsule()
	{
		Player.PlayerCollider.height = 1.8f;
		Player.PlayerCollider.center = new Vector3(Player.PlayerCollider.center.x, -0.5f, Player.PlayerCollider.center.z);
		Player.PlayerCollider.direction = 1;
		Player.PlayerCollider.transform.rotation = Player.transform.rotation;
	}

	public void SwimMoveCapsule()
	{
		Player.PlayerCollider.height = 1.1f;
		Player.PlayerCollider.center = new Vector3(Player.PlayerCollider.center.x, -0.3f, Player.PlayerCollider.center.z);
		Player.PlayerCollider.direction = 2;
		Player.PlayerCollider.transform.rotation = Player.transform.rotation;
	}

	public void CheckSwim()
	{
		if (Player.Skydiving || Player.IsDownWaitSave || Player.IsDie)
		{
			return;
		}
		WaterDeep = Battle.Ins.GetWaterDeep(Player.Pos);
		float num = 1.3f;
		if (Player.FSM.CurrentState.ID == StateID.Pa)
		{
			num = 0.2f;
		}
		if (Player.FSM.CurrentState.ID == StateID.Crouch)
		{
			num = 0.5f;
		}
		if (!(WaterDeep >= num) || Player.FSMUpBody.CurrentState.ID == StateID.Shouwuqi || Player.FSMUpBody.CurrentState.ID == StateID.Nawuqi)
		{
			return;
		}
		if (Player.InCar)
		{
			if (Player.NearCar != null && Player.InCar && Player.NearCar.GetSpeed() < 39f)
			{
				Battle.Ins.MyBattlePanel.OnClickDownCar(null);
			}
		}
		else
		{
			Player.FSM.SwitchState(StateID.Swim);
		}
	}

	private void SpeedUpSkill()
	{
		m_speedUpNum = Player.RunSpeedUpSkill(101);
	}

	private void ResetSkillSpeedUp()
	{
		Player.ReduceAnimatorSpeed(m_speedUpNum);
	}

	private float AddO2()
	{
		return Player.GetSkillValueIndex0(115);
	}

	private float AddInWaterTime()
	{
		return Player.GetSkillValueIndex0(116);
	}
}
