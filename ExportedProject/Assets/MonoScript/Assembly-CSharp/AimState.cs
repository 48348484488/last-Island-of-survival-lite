using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimState : FSMState
{
	private string m_CurAimatorName = string.Empty;

	private bool m_Jimiao = true;

	private bool Fast;

	private StateID m_OldUnderBodyStateID;

	private List<StateID> m_ForceUseCrossFaceState = new List<StateID>
	{
		StateID.Crouch,
		StateID.Stand
	};

	private float m_speedUpNum;

	public AimState()
	{
		stateID = StateID.Aim;
	}

	public override void Init()
	{
		FSMSystem fSMSystem = fsm;
		fSMSystem.Ticker = (Utils.VoidDelegate)Delegate.Combine(fSMSystem.Ticker, new Utils.VoidDelegate(OnTick));
	}

	private void OnTick()
	{
		CheckEnterAimState();
	}

	public override void DoBeforeEntering(object[] args)
	{
		if (args.Length > 0)
		{
			Fast = (bool)args[0];
		}
		Fast = true;
		m_CurAimatorName = string.Empty;
		m_OldUnderBodyStateID = StateID.NullStateID;
		SpeedUpSkill();
	}

	public override void DoBeforeLeaving()
	{
		Player.DisEnableFullBodyMask();
		ResetSkillSpeedUp();
	}

	public override void Act()
	{
		if (Player.ShootingTime > 0f)
		{
			Player.SetRotation(Battle.Ins.MainCamera.transform);
		}
		if (Player.CurGun == null)
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
		}
		else
		{
			UpdatePlayAimator();
		}
	}

	public override void Reason()
	{
	}

	private void UpdatePlayAimator()
	{
		Player.NeedAimIK = false;
		Player.NeedHoldIK = false;
		bool flag = false;
		bool flag2 = true;
		if (Player.FSM.CurrentState.ID == StateID.Crouch && !Player.IsJiMiao)
		{
			if ((CrouchState.CrouchRush || Player.AutoRun) && Player.FSM.CurrentState.ID != StateID.Fall && Player.FSM.CurrentState.ID != StateID.Jump && !Player.InCar)
			{
				m_TargetAimatorName = "Aim." + Player.CurGun.GunCfg.aimDunRush;
				flag = true;
			}
			else
			{
				m_TargetAimatorName = "Aim." + Player.CurGun.GunCfg.aimDun;
			}
		}
		else if (Player.FSM.CurrentState.ID == StateID.Pa && !Player.IsJiMiao)
		{
			Player.NeedHeadIK = false;
			if (Player.InputVector.magnitude < 0.1f)
			{
				m_TargetAimatorName = "Aim." + Player.CurGun.GunCfg.aimPa;
			}
			else
			{
				Player.NeedHoldIK = false;
				Player.NeedAimIK = false;
				flag2 = false;
				Player.DisEnableUpBodyFsm();
			}
		}
		else if (Player.ShootingTime > 0f || Player.IsJiMiao)
		{
			Fast = false;
			m_TargetAimatorName = "Aim." + Player.CurGun.GunCfg.aimZhan;
		}
		else
		{
			Fast = false;
			flag = false;
			m_TargetAimatorName = "Null";
			flag2 = false;
		}
		if (Player.InCar && !Player.IsDriver && !Player.Tanshen)
		{
			flag = false;
			m_TargetAimatorName = "Aim.zaiju_chengzuo03";
		}
		if (Fast)
		{
			if (flag)
			{
				Player.EnableFullBodyMask();
				Player.DisEnableUpBodyFsm();
				if (m_ForceUseCrossFaceState.Contains(Player.FSM.CurrentState.ID) && m_OldUnderBodyStateID != Player.FSM.CurrentState.ID)
				{
					Player.ChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName);
				}
				else
				{
					Player.FastChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName);
				}
			}
			else
			{
				Player.DisEnableFullBodyMask();
				if (flag2)
				{
					Player.EnableUpBodyFsm();
				}
				if (m_ForceUseCrossFaceState.Contains(Player.FSM.CurrentState.ID) && m_OldUnderBodyStateID != Player.FSM.CurrentState.ID)
				{
					Player.ChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName);
				}
				else
				{
					Player.FastChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName);
				}
			}
		}
		else if (flag)
		{
			Player.EnableFullBodyMask();
			Player.ChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName);
			Player.DisEnableUpBodyFsm();
		}
		else
		{
			if (flag2)
			{
				Player.EnableUpBodyFsm();
			}
			Player.ChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName);
			Player.DisEnableFullBodyMask();
		}
		if (Player.RoleHandAnimator != null && !Player.RoleHandAnimator.GetCurrentAnimatorStateInfo(0).IsName("Aim." + Player.CurGun.GunCfg.aimZhan))
		{
			Player.RoleHandAnimator.Play("Aim." + Player.CurGun.GunCfg.aimZhan, 0, 0f);
		}
		m_OldUnderBodyStateID = Player.FSM.CurrentState.ID;
	}

	private IEnumerator DelayDisEnableUpBodyFsm()
	{
		yield return new WaitForSeconds(0.1f);
		Player.DisEnableUpBodyFsm();
	}

	private IEnumerator DelayDisEnableFullBodyMask()
	{
		yield return new WaitForSeconds(0.1f);
		Player.DisEnableFullBodyMask();
	}

	private void CheckEnterAimState()
	{
		if (Player.CurGun != null && Player.FSMUpBody.CurrentState.ID == StateID.NullStateID && Player.FSM.CurrentState.ID != StateID.Cross && Player.FSM.CurrentState.ID != StateID.CrouchDown && Player.FSM.CurrentState.ID != StateID.PaUp && Player.FSM.CurrentState.ID != StateID.SkyDiving && Player.FSM.CurrentState.ID != StateID.InStartPlane && Player.FSM.CurrentState.ID != StateID.SavePeople && Player.FSM.CurrentState.ID != StateID.DownWaitSave)
		{
			Player.FSMUpBody.SwitchState(StateID.Aim);
		}
	}

	private void SpeedUpSkill()
	{
		m_speedUpNum = Player.RunSpeedUpSkill(106);
	}

	private void ResetSkillSpeedUp()
	{
		Player.ReduceAnimatorSpeed(m_speedUpNum);
		m_speedUpNum = 0f;
	}
}
