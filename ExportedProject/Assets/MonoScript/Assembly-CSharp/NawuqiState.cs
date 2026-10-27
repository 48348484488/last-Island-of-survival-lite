using System.Collections;
using UnityEngine;

public class NawuqiState : FSMState
{
	private int m_WantGetGunId;

	public NawuqiState()
	{
		stateID = StateID.Nawuqi;
	}

	public override void DoBeforeEntering(object[] args)
	{
		m_WantGetGunId = (int)args[0];
		if (Player.GetWeapon(m_WantGetGunId) == null)
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
			return;
		}
		Player.GetWeapon(m_WantGetGunId).Visible = false;
		UpdatePlayAimator();
	}

	private void Finish()
	{
		Player.ChangeHandWeapon(m_WantGetGunId);
		Player.GetWeapon(m_WantGetGunId).Visible = true;
		Battle.Ins.SelfPlayer.SoundControll.PlaySound(131);
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
	}

	public override void Reason()
	{
		if (IsTargetAimatorPlayFinish())
		{
			if (Player.CurGun != null)
			{
				Player.FSMUpBody.SwitchState(StateID.Aim);
			}
			else if (Player.HaveNearWeaponInHand)
			{
				Player.FSMUpBody.SwitchState(StateID.HoldNearWeaponState);
			}
			else
			{
				Player.FSMUpBody.SwitchState(StateID.NullStateID);
			}
			if (Battle.Ins.MyBattlePanel != null)
			{
				Battle.Ins.MyBattlePanel.UpdateGunImages();
			}
		}
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds(0.2f);
		Finish();
		yield return new WaitForSeconds(1f);
		Player.FSMUpBody.SwitchState(StateID.NullStateID);
	}

	private void UpdatePlayAimator()
	{
		Gun gun = Player.GetGun(m_WantGetGunId) as Gun;
		if (gun != null)
		{
			m_TargetAimatorName = "Nawuqi.chongfengqiang_hengwo_zhan_right_nawuqi01";
			if (Player.FSM.CurrentState.ID == StateID.Pa)
			{
				m_TargetAimatorName = "Nawuqi.chongfengqiang_hengwo_pa_right_nawuqi01";
			}
			if (gun.GunCfg.gunType == 1)
			{
				if (Player.FSM.CurrentState.ID == StateID.Stand)
				{
					m_TargetAimatorName = "Nawuqi.shouqiang_zhan_nawuqi01";
				}
				else if (Player.FSM.CurrentState.ID == StateID.Crouch)
				{
					m_TargetAimatorName = "Nawuqi.shouqiang_dun_nawuqi01";
				}
				else if (Player.FSM.CurrentState.ID == StateID.Pa)
				{
					m_TargetAimatorName = "Nawuqi.shouqiang_pa_nawuqi01";
				}
				else
				{
					m_TargetAimatorName = "Nawuqi.shouqiang_zhan_nawuqi01";
				}
			}
		}
		if (Player.GetWeapon(m_WantGetGunId) is NearWeapon)
		{
			if (Player.FSM.CurrentState.ID == StateID.Pa)
			{
				m_TargetAimatorName = "Nawuqi.dao_pa_nawuqi01";
			}
			else
			{
				m_TargetAimatorName = "Nawuqi.dao_zhan_nawuqi01";
			}
		}
		if (Player.FSM.CurrentState.ID == StateID.Pa)
		{
			Player.FastChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName);
		}
		else
		{
			Player.FastChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName);
		}
	}
}
