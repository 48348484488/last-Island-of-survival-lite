using System.Collections;
using UnityEngine;

public class ZhuangtianState : FSMState
{
	private bool m_Paing;

	private int m_itemId;

	private Coroutine m_ZhuangtianCoroutine;

	private float m_speedUpNum;

	public ZhuangtianState()
	{
		stateID = StateID.Zhuangtian;
	}

	public override void DoBeforeEntering(object[] args)
	{
		m_itemId = (int)args[0];
		UpdatePlayAimator1();
		SpeedUpSkill();
	}

	public override void DoBeforeLeaving()
	{
		Player.DisEnableFullBodyMask();
		Battle.StopConroutine(m_ZhuangtianCoroutine);
		ResetSkillSpeedUp();
	}

	public override void Act()
	{
		if (Player.InputVector.y > 2f)
		{
			Player.SetAnimatorForwardValue(1.5f);
		}
		Player.NeedHoldIK = false;
		Player.NeedAimIK = false;
	}

	public override void Reason()
	{
		if (Player.CurGun == null)
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
		}
	}

	public void UpdatePlayAimator1()
	{
		if (Player.CurGun != null)
		{
			Player.CurGun.CloseJiMiao();
			if (Player.FSM.CurrentState.ID == StateID.Pa)
			{
				m_ZhuangtianCoroutine = Battle.StartConroutine(StartZhuantian(Player.CurGun.GunCfg.zhuangtianPa01, Player.CurGun.GunCfg.zhuangtianPa02, Player.CurGun.GunCfg.zhuangtianPa03, true));
			}
			else
			{
				m_ZhuangtianCoroutine = Battle.StartConroutine(StartZhuantian(Player.CurGun.GunCfg.zhuangtianZhan01, Player.CurGun.GunCfg.zhuangtianZhan02, Player.CurGun.GunCfg.zhuangtianZhan03, false));
			}
		}
	}

	private IEnumerator StartZhuantian(string name01, string name02, string name03, bool isPa)
	{
		float time1 = 1.4f;
		float time2 = 0.56f;
		float time3 = 0.67f;
		int wantLoadNum = Singleton<BagMgr>.Ins.GetCurGunDate().BulletMax - Singleton<BagMgr>.Ins.GetCurGunDate().CurBulletNum;
		if (Player.CurGun.Name == "REVOLEVER")
		{
			if (isPa)
			{
				time1 = 1f;
				time2 = 0.56f;
				time3 = 1.36f;
			}
			else
			{
				time1 = 1.4f;
				time2 = 0.56f;
				time3 = 0.67f;
			}
		}
		if (Player.CurGun.Name == "SHOTGUN")
		{
			if (isPa)
			{
				time1 = 1.5f;
				time2 = 0.5f;
				time3 = 1.23f;
			}
			else
			{
				time1 = 1f;
				time2 = 0.67f;
				time3 = 0.63f;
			}
		}
		if (Player.CurGun.Name == "SPAS12" || Player.CurGun.GunCfg.id == 52411)
		{
			if (isPa)
			{
				time1 = 1.13f;
				time2 = 0.53f;
				time3 = 1.43f;
			}
			else
			{
				time1 = 0.96f;
				time2 = 0.63f;
				time3 = 1.27f;
			}
		}
		if (Player.CurGun.Name == "kar98K")
		{
			if (isPa)
			{
				time1 = 2f;
				time2 = 0.53f;
				time3 = 1.43f;
			}
			else
			{
				time1 = 1.5f;
				time2 = 0.63f;
				time3 = 1.3f;
			}
		}
		if (Player.CurGun.GunCfg.id == 52401)
		{
			if (isPa)
			{
				time1 = 1.5f;
				time2 = 0.5f;
				time3 = 1.23f;
			}
			else
			{
				time1 = 1f;
				time2 = 0.67f;
				time3 = 0.63f;
			}
		}
		name01 = "Zhuangtian." + name01;
		name02 = "Zhuangtian." + name02;
		name03 = "Zhuangtian." + name03;
		Gun gun = Player.CurGun;
		if (isPa)
		{
			Player.ChangeAnimatorStates(Player.FullBodyLayer, name01);
			gun.PlayAnim(gun.GunCfg.id + ".pa_huandan01");
		}
		else
		{
			Player.ChangeAnimatorStates(Player.UpperBodyLayer, name01);
			gun.PlayAnim(gun.GunCfg.id + ".zhan_huandan01");
		}
		yield return new WaitForSeconds(time1);
		do
		{
			if (isPa)
			{
				Player.FastChangeAnimatorStates(Player.FullBodyLayer, name02, true);
				gun.PlayAnim(gun.GunCfg.id + ".pa_huandan02");
			}
			else
			{
				Player.FastChangeAnimatorStates(Player.UpperBodyLayer, name02, true);
				gun.PlayAnim(gun.GunCfg.id + ".zhan_huandan02");
			}
			yield return new WaitForSeconds(time2);
			if (Player.CurGun != null)
			{
				Singleton<BagMgr>.Ins.LoadGunBullet(m_itemId);
				wantLoadNum--;
			}
		}
		while (wantLoadNum > 0 && Battle.Ins.BagHaveBullet());
		if (isPa)
		{
			Player.ChangeAnimatorStates(Player.FullBodyLayer, name03);
			gun.PlayAnim(gun.GunCfg.id + ".pa_huandan03");
		}
		else
		{
			Player.ChangeAnimatorStates(Player.UpperBodyLayer, name03);
			gun.PlayAnim(gun.GunCfg.id + ".zhan_huandan03");
		}
		yield return new WaitForSeconds(time3);
		Player.FSMUpBody.SwitchState(StateID.Aim);
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
