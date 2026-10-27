using System;
using System.Collections;
using System.Runtime.CompilerServices;
using SC.UI;
using UnityEngine;
using cfg;

public class ThrowWeaponState : FSMState
{
	[CompilerGenerated]
	private sealed class _003CMyThrowLei_003Ec__AnonStorey2
	{
		internal int id;

		internal ThrowWeaponState _0024this;

		internal void _003C_003Em__0(GameObject o)
		{
			if (_0024this.Player.FSMUpBody.CurrentState.ID != StateID.ThrowWeaponState)
			{
				UnityEngine.Object.Destroy(o);
				return;
			}
			_0024this.m_throwLei = o.AddComponent<ThrowLei>();
			_0024this.m_throwLei.Id = id;
			Battle.Ins.SelfPlayer.CurThrowWeapon = o;
		}
	}

	public static bool IsUpLei = true;

	private bool m_LaxianFinish;

	private int m_LeiId;

	private Coroutine m_Coroutine;

	private ThrowLei m_throwLei;

	private GameObject paowuxian;

	public ThrowWeaponState()
	{
		stateID = StateID.ThrowWeaponState;
		BattleEvent.OnDownThrowLei = (Utils.IntDelegate)Delegate.Combine(BattleEvent.OnDownThrowLei, new Utils.IntDelegate(OnDownThrowLei));
	}

	private void OnDownThrowLei(int id)
	{
		if (Player.FSMUpBody.CurrentState.ID != StateID.ThrowWeaponState && Player.FSMUpBody.CurrentState.ID != StateID.Shoot && Singleton<BagMgr>.Ins.GetItemNum(id) > 0 && IsUpLei && Player.HaveThrowWeaponInHand && !(Player.GetCurrentWeapon().WeaponObject == null))
		{
			m_LeiId = id;
			Battle.StopConroutine(m_Coroutine);
			IsUpLei = false;
			BattleEvent.OnUpLei = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnUpLei, new Utils.VoidDelegate(OnUpLei));
			Player.FSMUpBody.SwitchState(StateID.ThrowWeaponState);
		}
	}

	private IEnumerator PrePareThrowLei(int m_LeiId)
	{
		while (Player.FSMUpBody.CurrentState.ID != 0)
		{
			yield return null;
		}
		Player.FSMUpBody.SwitchState(StateID.ThrowWeaponState);
	}

	private void LoadOnePaowuxian()
	{
		ResMgr.Ins.CreateFromAB("model/Paowuxian.ab", null, _003CLoadOnePaowuxian_003Em__0);
	}

	public override void DoBeforeEntering(object[] args)
	{
		ThrowCfg throwCfg = ThrowCfg.Get(m_LeiId);
		Battle.Ins.SyncPlaySound(throwCfg.laSound);
		m_LaxianFinish = false;
		UpdateLaxianAnimatorName();
		LoadOnePaowuxian();
	}

	private void OnUpLei()
	{
		m_throwLei = null;
		GameObject gameObject = UnityEngine.Object.Instantiate(Player.GetCurrentWeapon().WeaponObject);
		gameObject.transform.SetParent(Battle.Ins.SelfPlayer.RightHandGuaDian.transform);
		gameObject.transform.position = Player.RightHandGuaDian.transform.position;
		m_throwLei = gameObject.AddComponent<ThrowLei>();
		m_throwLei.Id = m_LeiId;
		IsUpLei = true;
		UseTimePanel.HideUseTimePanel();
		UnityEngine.Object.Destroy(paowuxian);
	}

	private void OnLaxianFinish()
	{
		m_LaxianFinish = true;
	}

	public override void DoBeforeLeaving()
	{
		UseTimePanel.HideUseTimePanel();
		UnityEngine.Object.Destroy(paowuxian);
		Battle.StopConroutine(m_Coroutine);
		BattleEvent.OnUpLei = null;
		if (!IsUpLei)
		{
			UnityEngine.Object.Destroy(m_throwLei.gameObject);
			IsUpLei = true;
		}
	}

	public override void Act()
	{
		Player.CloseAllIK();
		UpdatePlayAimator();
		if (m_LaxianFinish && IsUpLei)
		{
			Utils.TriggerEvent(BattleEvent.OnCanThrowLei);
		}
	}

	public override void Reason()
	{
		if (!Player.HaveThrowWeaponInHand)
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
		}
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds(0.8f);
		UseTimePanel.ShowUseTimePanel(4f, string.Empty);
		OnLaxianFinish();
		yield return Utils.WaitForSeconds(4f);
		OnUpLei();
		yield return new WaitForSeconds(3f);
		Player.FSMUpBody.SwitchState(StateID.NullStateID);
		Player.PlayChangeWeapen(-1);
	}

	private void UpdatePlayAimator()
	{
		if (m_LaxianFinish)
		{
			if (Player.FSM.CurrentState.ID == StateID.Stand)
			{
				UpdateTargetAimatorName("stand");
			}
			else if (Player.FSM.CurrentState.ID == StateID.Crouch)
			{
				UpdateTargetAimatorName("crouch");
			}
			else if (Player.FSM.CurrentState.ID == StateID.Pa)
			{
				UpdateTargetAimatorName("pa");
			}
			else
			{
				UpdateTargetAimatorName("stand");
			}
			Player.ChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName);
		}
	}

	private void UpdateTargetAimatorName(string zhuangtai)
	{
		m_TargetAimatorName = "ThrowWeapon." + zhuangtai;
	}

	private void UpdateLaxianAnimatorName()
	{
		if (Player.FSM.CurrentState.ID == StateID.Pa)
		{
			Player.ChangeAnimatorStates(Player.UpperBodyLayer, "ThrowWeapon.Grenade_pa_shoulei01");
		}
		else
		{
			Player.ChangeAnimatorStates(Player.UpperBodyLayer, "ThrowWeapon.Grenade_zhan_shoulei01");
		}
	}

	private void MyThrowLei(int id)
	{
		_003CMyThrowLei_003Ec__AnonStorey2 _003CMyThrowLei_003Ec__AnonStorey = new _003CMyThrowLei_003Ec__AnonStorey2();
		_003CMyThrowLei_003Ec__AnonStorey.id = id;
		_003CMyThrowLei_003Ec__AnonStorey._0024this = this;
		ThrowCfg throwCfg = ThrowCfg.Get(_003CMyThrowLei_003Ec__AnonStorey.id);
		Battle.Ins.SyncPlaySound(throwCfg.laSound);
		if (throwCfg.type == 1)
		{
			ResMgr.Ins.CreateFromAB(throwCfg.path, null, _003CMyThrowLei_003Ec__AnonStorey._003C_003Em__0);
		}
	}

	[CompilerGenerated]
	private void _003CLoadOnePaowuxian_003Em__0(GameObject go)
	{
		go.transform.position = Battle.Ins.SelfPlayer.RightHandGuaDian.transform.position;
		if (paowuxian != null)
		{
			UnityEngine.Object.Destroy(paowuxian);
		}
		paowuxian = go;
		if (Player.FSMUpBody.CurrentState.ID != StateID.ThrowWeaponState)
		{
			UnityEngine.Object.Destroy(go);
		}
	}
}
