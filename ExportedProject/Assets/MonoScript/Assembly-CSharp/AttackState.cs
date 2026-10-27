using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;

public class AttackState : FSMState
{
	private bool m_Jumping;

	private List<int> m_FuziId = new List<int> { 55001, 55002, 55003, 55004, 55005 };

	private List<int> m_GaoziId = new List<int> { 55006, 55007, 55008, 55009, 55010 };

	private StateID m_LastFsmID;

	public AttackState()
	{
		stateID = StateID.Attack;
	}

	public override void DoBeforeEntering(object[] args)
	{
		if (args != null)
		{
			m_Jumping = (bool)args[0];
		}
		UpdatePlayAimator();
	}

	public override void DoBeforeLeaving()
	{
		Player.EnableUpBodyFsm();
		Player.DisEnableFullBodyMask();
		Player.RightHand.GetComponent<HandAttack>().First = false;
		Player.LeftHand.GetComponent<HandAttack>().First = false;
		if (Player.HaveNearWeaponInHand)
		{
			Player.GetCurNearweapon().Close();
		}
		if (m_Jumping)
		{
			Player.FSM.SwitchState(StateID.Stand);
		}
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds(1.5f);
		if (Player.PlayerAnimator.GetCurrentAnimatorStateInfo(Player.UpperBodyLayer).IsName("Null"))
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
		}
	}

	public override void Act()
	{
	}

	public override void Reason()
	{
		if (IsTargetAimatorPlayFinish())
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
			if (m_Jumping)
			{
				Player.FSM.SwitchState(StateID.Stand);
			}
		}
	}

	private void PlayMoveKongshouAttack(int random)
	{
		if (random == 0)
		{
			m_TargetAimatorName = "Attack.kongshou_zhan_attack01";
		}
		else
		{
			m_TargetAimatorName = "Attack.kongshou_zhan_attack02";
		}
		Player.ChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName);
	}

	private void PlayNotMoveKongshouAttack(int random)
	{
		if (random == 0)
		{
			m_TargetAimatorName = "Attack.kongshou_zhan_attack01_1";
		}
		else
		{
			m_TargetAimatorName = "Attack.kongshou_zhan_attack02_1";
		}
		Player.ChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName);
	}

	public void UpdatePlayAimator()
	{
		int num = Random.Range(0, 2);
		if (Player.IsKongshou)
		{
			if (m_Jumping)
			{
				Player.DisEnableUpBodyFsm();
				StandState.StandCapsule(Player);
				m_TargetAimatorName = "Attack.kongshou_zhan_run_jump_attack01";
				Player.ChangeAnimatorStates(Player.BaseLayer, m_TargetAimatorName);
			}
			else if (Player.Input.magnitude <= 0.1f && Player.FSM.CurrentState.ID == StateID.Stand)
			{
				PlayNotMoveKongshouAttack(num);
			}
			else
			{
				PlayMoveKongshouAttack(num);
			}
		}
		else
		{
			if (!Player.HaveNearWeaponInHand)
			{
				return;
			}
			ItemCfg weaponCfg = Player.GetCurrentWeapon().WeaponCfg;
			if (m_FuziId.Contains(weaponCfg.id) && Player.Input.magnitude < 0.1f)
			{
				if (Player.FSM.CurrentState.ID == StateID.Stand)
				{
					m_TargetAimatorName = "Attack.fu_zhan_attack_01";
				}
				else if (Player.FSM.CurrentState.ID == StateID.Crouch)
				{
					m_TargetAimatorName = "Attack.gao_dun_attack_01";
				}
				Player.DisEnableUpBodyFsm();
				Player.ChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName);
			}
			else if (m_GaoziId.Contains(weaponCfg.id) && Player.Input.magnitude < 0.1f)
			{
				if (Player.FSM.CurrentState.ID == StateID.Stand)
				{
					m_TargetAimatorName = "Attack.gao_zhan_attack_01";
				}
				else if (Player.FSM.CurrentState.ID == StateID.Crouch)
				{
					m_TargetAimatorName = "Attack.gao_dun_attack_01";
				}
				Player.DisEnableUpBodyFsm();
				Player.ChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName);
			}
			else if (Player.Input == Vector2.zero && Player.FSM.CurrentState.ID == StateID.Stand)
			{
				if (num == 0)
				{
					m_TargetAimatorName = "Attack.dao_zhan_attack01_1";
				}
				else
				{
					m_TargetAimatorName = "Attack.dao_zhan_attack02_1";
				}
				Player.DisEnableUpBodyFsm();
				Player.ChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName);
			}
			else
			{
				if (num == 0)
				{
					m_TargetAimatorName = "Attack.dao_zhan_attack01";
				}
				else
				{
					m_TargetAimatorName = "Attack.dao_zhan_attack02";
				}
				Player.EnableUpBodyFsm();
				Player.ChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName);
			}
		}
	}
}
