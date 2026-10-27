using System.Collections;
using UnityEngine;

public class ShootState : FSMState
{
	private bool m_ShootFinish;

	private float m_ShootTime = 0.1f;

	private string m_CurAimatorName = string.Empty;

	public ShootState()
	{
		stateID = StateID.Shoot;
	}

	public override void DoBeforeEntering(object[] args)
	{
		m_ShootTime = 0.1f;
		m_ShootFinish = false;
		Player.WaitForRush = 1f;
		Player.WaitForPaMove = 0.3f;
		Player.CloseAutoRun();
		UpdatePlayAimator();
	}

	private void OnShootFinish()
	{
		m_ShootFinish = true;
	}

	public override void DoBeforeLeaving()
	{
		Player.ShootingTime = 6f;
	}

	public override void Act()
	{
	}

	public override void Reason()
	{
		if (m_ShootFinish || Player.IsPlayFinish(m_TargetAimatorName))
		{
			if (Player.IsKongshou)
			{
				Player.FSMUpBody.SwitchState(StateID.NullStateID);
			}
			else
			{
				Player.FSMUpBody.SwitchState(StateID.Aim);
			}
		}
	}

	public override IEnumerator ActCoroutine()
	{
		if (Player.HaveThrowWeaponInHand)
		{
			yield return new WaitForSeconds(0.2f);
			Utils.TriggerEvent(BattleEvent.OnCanFlyLei);
			yield return new WaitForSeconds(0.6f);
		}
		else
		{
			yield return new WaitForSeconds(m_ShootTime);
		}
		OnShootFinish();
	}

	private void UpdatePlayAimator()
	{
		if (Player.CurGun != null)
		{
			if (Player.FSM.CurrentState.ID == StateID.Crouch && !Player.IsJiMiao)
			{
				m_TargetAimatorName = "Shoot." + Player.CurGun.GunCfg.shootDun;
			}
			else if (Player.FSM.CurrentState.ID == StateID.Pa && !Player.IsJiMiao)
			{
				m_TargetAimatorName = "Shoot." + Player.CurGun.GunCfg.shootPa;
			}
			else
			{
				m_TargetAimatorName = "Shoot." + Player.CurGun.GunCfg.shootZhan;
			}
			m_ShootTime = Player.CurGun.GunCfg.factors[53] * 0.001f;
			string gunShootAim = Player.CurGun.GunCfg.gunShootAim;
			if (!string.IsNullOrEmpty(gunShootAim))
			{
				Player.CurGun.PlayAnim(Player.CurGun.GunCfg.gunShootAim);
			}
			Player.FastChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName, true);
			Player.RoleHandAnimator.Play(m_TargetAimatorName, Player.RoleHandAnimator.GetLayerIndex("UpperBody Layer"), 0f);
		}
		if (Player.HaveThrowWeaponInHand)
		{
			if (Player.FSM.CurrentState.ID == StateID.Crouch)
			{
				m_TargetAimatorName = "Shoot.lei_dun_miaozhun_attack01";
			}
			else if (Player.FSM.CurrentState.ID == StateID.Pa)
			{
				m_TargetAimatorName = "Shoot.lei_pa_miaozhun_attack01";
			}
			else
			{
				m_TargetAimatorName = "Shoot.lei_zhan_miaozhun_attack01";
			}
			Player.FastChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName);
		}
	}
}
