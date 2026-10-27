using System.Collections;
using UnityEngine;

public class ShouwuqiState : FSMState
{
	private int m_WantGetWeaponId;

	public ShouwuqiState()
	{
		stateID = StateID.Shouwuqi;
	}

	public override void DoBeforeEntering(object[] args)
	{
		m_TargetAimatorName = "Shouwuqi.chongfengqiang_hengwo_zhan_right_shouwuqi01";
		if (args != null)
		{
			m_WantGetWeaponId = (int)args[0];
		}
		else
		{
			m_WantGetWeaponId = -1;
		}
		UpdatePlayAimator();
		if (Player.CurGun != null)
		{
			Player.CurGun.StopShoot();
			Player.CurGun.CloseJiMiao();
		}
	}

	private void Finish()
	{
		Player.ChangeHandWeapon(m_WantGetWeaponId);
		Battle.Ins.SelfPlayer.SoundControll.PlaySound(135);
	}

	public override void DoBeforeLeaving()
	{
		Player.CanShoot = true;
		Player.DisEnableFullBodyMask();
	}

	public override void Act()
	{
		Player.NeedHoldIK = false;
		Player.NeedAimIK = false;
		Player.CanShoot = false;
		if (Player.IsPlayFinish(m_TargetAimatorName))
		{
			if (m_WantGetWeaponId != -1)
			{
				Player.FSMUpBody.SwitchState(StateID.Nawuqi, m_WantGetWeaponId);
			}
			else
			{
				Player.FSMUpBody.SwitchState(StateID.NullStateID);
				Player.DisEnableFullBodyMask();
			}
			if (Battle.Ins.MyBattlePanel != null)
			{
				Battle.Ins.MyBattlePanel.UpdateGunImages();
			}
		}
	}

	public override void Reason()
	{
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds(0.4f);
		Finish();
		yield return new WaitForSeconds(1f);
		Player.FSMUpBody.SwitchState(StateID.NullStateID);
	}

	private void UpdatePlayAimator()
	{
		if (Player.CurGun != null)
		{
			m_TargetAimatorName = "Shouwuqi.chongfengqiang_hengwo_zhan_right_shouwuqi01";
			if (Player.FSM.CurrentState.ID == StateID.Pa)
			{
				m_TargetAimatorName = "Shouwuqi.chongfengqiang_hengwo_pa_right_shouwuqi01";
			}
			if (Player.CurGun.GunCfg.gunType == 1)
			{
				if (Player.FSM.CurrentState.ID == StateID.Stand)
				{
					m_TargetAimatorName = "Shouwuqi.shouqiang_zhan_shouwuqi01";
				}
				else if (Player.FSM.CurrentState.ID == StateID.Crouch)
				{
					m_TargetAimatorName = "Shouwuqi.shouqiang_dun_shouwuqi01";
				}
				else if (Player.FSM.CurrentState.ID == StateID.Pa)
				{
					m_TargetAimatorName = "Shouwuqi.shouqiang_pa_shouwuqi01";
				}
				else
				{
					m_TargetAimatorName = "Shouwuqi.shouqiang_zhan_shouwuqi01";
				}
			}
		}
		if (Player.HaveNearWeaponInHand)
		{
			if (Player.FSM.CurrentState.ID == StateID.Pa)
			{
				m_TargetAimatorName = "Shouwuqi.dao_pa_shouwuqi01";
			}
			else
			{
				m_TargetAimatorName = "Shouwuqi.dao_zhan_shouwuqi01";
			}
		}
		if (Player.FSM.CurrentState.ID == StateID.Pa)
		{
			Player.ChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName);
		}
		else
		{
			Player.ChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName);
		}
	}
}
