using System.Collections;
using SC.UI;
using UnityEngine;

public class HuanzidanState : FSMState
{
	private int m_BulletTypeNum;

	private float m_BulletAddTime;

	private Coroutine m_Coroutine;

	private int m_itemId;

	private float m_speedUpNum;

	public HuanzidanState()
	{
		stateID = StateID.Huanzidan;
	}

	public override void DoBeforeEntering(object[] args)
	{
		m_itemId = (int)args[0];
		UpdatePlayAimator1();
		m_Coroutine = Battle.StartConroutine(ShowAddBulletTimePanel());
		SpeedUpSkill();
	}

	public override void DoBeforeLeaving()
	{
		Player.DisEnableFullBodyMask();
		UseTimePanel.HideUseTimePanel();
		Battle.StopConroutine(m_Coroutine);
		ResetSkillSpeedUp();
		Transform transform = Player.Wuqi_guadian002.transform.Find("rpgzidan");
		if (transform != null)
		{
			Object.Destroy(transform.gameObject);
		}
	}

	public override void Act()
	{
		Player.NeedHoldIK = false;
		Player.NeedAimIK = false;
		if (Player.InputVector.y > 2f)
		{
			Player.SetAnimatorForwardValue(1.5f);
		}
	}

	public override void Reason()
	{
		if (Player.CurGun != null && !Battle.Ins.BagHaveBullet())
		{
			Player.FSMUpBody.SwitchState(StateID.Aim);
		}
		if (Player.IsPlayFinish(m_TargetAimatorName))
		{
			if (Player.CurGun != null && Player.CurGun.GunCfg.gunType != 8)
			{
				Player.AgainLoadBulletTime = 2f;
				Singleton<BagMgr>.Ins.AutoLoadGunBullet(Singleton<BagMgr>.Ins.HandInstanceId, m_itemId);
			}
			Player.FSMUpBody.SwitchState(StateID.Aim);
		}
		if (Player.CurGun == null)
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
		}
	}

	public override IEnumerator ActCoroutine()
	{
		yield return Utils.WaitForSeconds(0.5f);
		if (Player.CurGun.GunCfg.gunType == 8)
		{
			GameObject b = Battle.Ins.GetOneRpgBullet;
			Transform zidan2 = Player.Wuqi_guadian002.transform.Find("rpgzidan");
			if (zidan2 != null)
			{
				Object.Destroy(zidan2.gameObject);
			}
			b.transform.SetParent(Player.Wuqi_guadian002.transform, false);
			if (Player.FSM.CurrentState.ID == StateID.Pa)
			{
				yield return Utils.WaitForSeconds(1.3f);
			}
			else
			{
				yield return Utils.WaitForSeconds(0.75f);
			}
			zidan2 = Player.CurGun.GunInfo.Muzzle.Find("rpgzidan");
			if (zidan2 != null)
			{
				Object.Destroy(zidan2.gameObject);
			}
			Player.AgainLoadBulletTime = 2f;
			Singleton<BagMgr>.Ins.AutoLoadGunBullet(Singleton<BagMgr>.Ins.HandInstanceId, m_itemId);
			b.transform.SetParent(Player.CurGun.GunInfo.Muzzle, false);
		}
	}

	public void UpdatePlayAimator1()
	{
		if (Player.CurGun != null)
		{
			Player.CurGun.CloseJiMiao();
			string text = ((Player.FSM.CurrentState.ID == StateID.Pa) ? ((Player.CurGun.CurLoadBulletNum != 0) ? Player.CurGun.GunCfg.changeBulletPa : Player.CurGun.GunCfg.zeroChangeBulletPa) : ((Player.CurGun.CurLoadBulletNum != 0) ? Player.CurGun.GunCfg.changeBulletZhan : Player.CurGun.GunCfg.zeroChangeBulletZhan));
			if (string.IsNullOrEmpty(text))
			{
				Debug.LogError("huandan error : " + Player.CurGun.GunCfg.name);
			}
			m_TargetAimatorName = "Huanzidan." + text;
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

	public IEnumerator ShowAddBulletTimePanel()
	{
		if (Player.FSM.CurrentState.ID == StateID.Pa)
		{
			if (!Player.PlayerAnimator.GetCurrentAnimatorStateInfo(Player.FullBodyLayer).IsName(m_TargetAimatorName))
			{
				yield return null;
			}
			m_BulletAddTime = Player.PlayerAnimator.GetCurrentAnimatorStateInfo(Player.FullBodyLayer).length;
		}
		else
		{
			if (!Player.PlayerAnimator.GetCurrentAnimatorStateInfo(Player.UpperBodyLayer).IsName(m_TargetAimatorName))
			{
				yield return null;
			}
			m_BulletAddTime = Player.PlayerAnimator.GetCurrentAnimatorStateInfo(Player.UpperBodyLayer).length;
		}
		UseTimePanel.ShowUseTimePanel(m_BulletAddTime, Utils.GetString(227));
	}

	private void SpeedUpSkill()
	{
		m_speedUpNum = Player.RunSpeedUpSkill(105);
	}

	private void ResetSkillSpeedUp()
	{
		Player.ReduceAnimatorSpeed(m_speedUpNum);
	}
}
