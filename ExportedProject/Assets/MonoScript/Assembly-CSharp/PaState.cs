using UnityEngine;

public class PaState : FSMState
{
	public static bool PaWalk;

	private float m_speedUpNum;

	public PaState()
	{
		stateID = StateID.Pa;
	}

	public override void DoBeforeEntering(object[] args)
	{
		Player.MainCamera.ChangeState("Pa");
		Player.ChangeAnimatorStates(Player.BaseLayer, "Pa.Pa");
		Player.RotateSpeed = 10f;
		PaCapsule();
		Utils.TriggerEvent(BattleEvent.OnEnterPa);
		SpeedUpSkill();
		Player.CanOpenJimiao = false;
	}

	public override void DoBeforeLeaving()
	{
		Player.transform.eulerAngles = new Vector3(0f, Player.transform.eulerAngles.y, Player.transform.eulerAngles.z);
		Player.CanOpenJimiao = true;
		ResetSkillSpeedUp();
	}

	public override void Act()
	{
		Move();
		Player.NeedHeadIK = false;
		Player.NeedAimIK = false;
	}

	public override void Reason()
	{
	}

	public void PaCapsule()
	{
		Player.PlayerCollider.height = 1.9f;
		Player.PlayerCollider.center = new Vector3(Player.PlayerCollider.center.x, 0.42f, 0f);
		Player.PlayerCollider.direction = 2;
		Player.PlayerCollider.transform.rotation = Player.PlayerCollider.transform.rotation;
	}

	private void Move()
	{
		Player.FreeType = false;
		PaWalk = false;
		if (Player.Input.magnitude != 0f)
		{
			Player.m_SpeedMultiple = 1f / Player.Input.magnitude;
			PaWalk = true;
		}
		Player.m_lastFrameFreeType = Player.FreeType;
		ChangeCameraState();
		if (Player.WaitForPaMove > 0f)
		{
			Player.SetAnimatorHorizontalValue(0f);
			Player.SetAnimatorForwardValue(0f);
		}
		else
		{
			Player.SetAnimatorHorizontalValue(Player.InputVector.x);
			Player.SetAnimatorForwardValue(Player.InputVector.y);
		}
	}

	private void ChangeCameraState()
	{
		if (Player.IsJiMiao && PaWalk)
		{
			Player.MainCamera.SetMainTarget(Player.PlayerTransform, "Pa");
		}
	}

	private void SpeedUpSkill()
	{
		m_speedUpNum = Player.RunSpeedUpSkill(102);
	}

	private void ResetSkillSpeedUp()
	{
		Player.ReduceAnimatorSpeed(m_speedUpNum);
		m_speedUpNum = 0f;
	}
}
