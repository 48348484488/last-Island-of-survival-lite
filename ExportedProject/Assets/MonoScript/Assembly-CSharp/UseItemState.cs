using System;
using System.Collections;
using System.Runtime.CompilerServices;
using SC.UI;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

public class UseItemState : FSMState
{
	private Coroutine m_Coroutine;

	private float m_UseItemTime;

	private int m_UseItemId;

	private bool m_Used;

	private GameObject m_ItemObj;

	private StateID m_BeforeEnterFsmStateId;

	private int WantUseItemId;

	private int WantUseItemInsId;

	private ItemCfg m_itemCfg;

	private int m_itemSoundId;

	private float m_speedUpNum;

	public UseItemState()
	{
		stateID = StateID.UseItem;
		BattlePackEvent.UseItemByInstanceItemId = (Utils.Int2Delegate)Delegate.Combine(BattlePackEvent.UseItemByInstanceItemId, new Utils.Int2Delegate(OnPackUseItem));
	}

	private void OnPackUseItem(int itemId, int insId)
	{
		if (!Player.CheckCanUseItem())
		{
			return;
		}
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		if (itemCfg.type != 32 && itemCfg.type != 124)
		{
			return;
		}
		if (Player.Tanshen)
		{
			AlertBox.Show(Utils.GetString(238));
		}
		else if (CheckCanUse(itemCfg) && WantUseItemId != itemId && Singleton<BagMgr>.Ins.GetItemNum(itemId) > 0 && (Player.FSM.CurrentState.ID == StateID.Stand || Player.FSM.CurrentState.ID == StateID.Crouch || Player.FSM.CurrentState.ID == StateID.Pa || Player.FSM.CurrentState.ID == StateID.InCar))
		{
			Player.ChangeHandWeapon(-1);
			Utils.StopConroutine(m_Coroutine);
			if (Player.FSMUpBody.CurrentState.ID != StateID.UseItem)
			{
				Player.FSMUpBody.SwtichStateReStart(StateID.UseItem, itemId, insId);
			}
		}
	}

	private bool CheckCanUse(ItemCfg itemCfg)
	{
		if ((itemCfg.id == 300 || itemCfg.id == 320) && (float)Player.HP / (float)ConstsBs.HpPlayer >= itemCfg.extras[2] / 10000f)
		{
			AlertBox.Show(Utils.GetString(223));
			return false;
		}
		if ((itemCfg.id == 301 || itemCfg.id == 321) && (float)Player.HP / (float)ConstsBs.HpPlayer >= itemCfg.extras[2] / 10000f)
		{
			AlertBox.Show(Utils.GetString(224));
			return false;
		}
		if ((itemCfg.id == 302 || itemCfg.id == 322) && (float)Player.HP >= (float)ConstsBs.HpPlayer)
		{
			AlertBox.Show(Utils.GetString(225));
			return false;
		}
		return true;
	}

	private IEnumerator PrePareUseItem(int itemId, int insId)
	{
		while (Player.FSMUpBody.CurrentState.ID != 0 && Player.FSMUpBody.CurrentState.ID != StateID.Nawuqi)
		{
			yield return null;
		}
		Player.FSMUpBody.SwtichStateReStart(StateID.UseItem, itemId, insId);
	}

	public override void DoBeforeEntering(object[] args)
	{
		Utils.TriggerEvent(BattlePackEvent.OnBreakUseItem);
		WantUseItemId = (int)args[0];
		WantUseItemInsId = (int)args[1];
		m_Used = false;
		m_itemCfg = ItemCfg.Get(WantUseItemId);
		if (Player.PlayerRigidbody.velocity.magnitude > 0.1f && m_itemCfg.type != 124)
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
			return;
		}
		if (m_itemCfg.type == 124)
		{
			SendUseMsg();
		}
		CStartUseMedicine msg = new CStartUseMedicine();
		Client2Gs.Ins.Send(msg);
		try
		{
			m_itemSoundId = SingletonMono<AudioManager>.Ins.Play2D((int)m_itemCfg.extras[6]);
		}
		catch (Exception)
		{
		}
		m_BeforeEnterFsmStateId = Player.FSM.CurrentState.ID;
		UpdatePlayAimator();
	}

	public override void DoBeforeLeaving()
	{
		Utils.TriggerEvent(BattleEvent.OnUseItemFinish, (!m_Used) ? (-1) : WantUseItemInsId);
		Reset();
		CStopUseMedicine msg = new CStopUseMedicine();
		Client2Gs.Ins.Send(msg);
		if (Battle.Ins.MyBattlePanel != null)
		{
			Battle.Ins.MyBattlePanel.UpdateGunImages();
		}
	}

	private void Reset()
	{
		Player.DisEnableFullBodyMask();
		Battle.StopConroutine(m_Coroutine);
		m_Coroutine = null;
		WantUseItemId = -1;
		WantUseItemInsId = -1;
		UseTimePanel.HideUseTimePanel();
		SingletonMono<AudioManager>.Ins.StopMusic(m_itemSoundId);
		if (m_ItemObj != null)
		{
			UnityEngine.Object.DestroyImmediate(m_ItemObj);
		}
	}

	public override void Act()
	{
	}

	public override IEnumerator ActCoroutine()
	{
		yield return Utils.WaitForSeconds(0.5f);
	}

	public override void Reason()
	{
		if (Player.FSM.CurrentState.ID != m_BeforeEnterFsmStateId || Player.PlayerRigidbody.velocity.magnitude > 0.8f || Player.Input.magnitude > 0.01f)
		{
			Leave();
		}
		if (m_itemCfg.type != 124 && Singleton<BagMgr>.Ins.GetItemNum(WantUseItemId) <= 0)
		{
			Leave();
		}
		if (IsTargetAimatorPlayFinish())
		{
			SendUseMsg();
			Leave();
		}
	}

	private void SendUseMsg()
	{
		if (!m_Used)
		{
			Singleton<BagMgr>.Ins.UseItem(WantUseItemInsId, 1);
			m_Used = true;
		}
	}

	private void Leave()
	{
		Player.FSMUpBody.SwitchState(StateID.NullStateID);
	}

	private void UpdatePlayAimator()
	{
		ItemCfg itemCfg = ItemCfg.Get(WantUseItemId);
		m_TargetAimatorName = ((Player.FSM.CurrentState.ID != StateID.Pa) ? itemCfg.extrasstring[0] : itemCfg.extrasstring[1]);
		if (Player.FSM.CurrentState.ID == StateID.Pa)
		{
			Player.FastChangeAnimatorStates(Player.FullBodyLayer, m_TargetAimatorName, true);
		}
		else
		{
			Player.FastChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName, true);
		}
		m_Coroutine = Battle.StartConroutine(ShowUseItemTimePanel(itemCfg));
		LoadItemModel();
	}

	private void LoadItemModel()
	{
		ItemCfg itemCfg = ItemCfg.Get(WantUseItemId);
		if (!string.IsNullOrEmpty(itemCfg.modelPath))
		{
			ResMgr.Ins.CreateFromAB(itemCfg.modelPath, null, _003CLoadItemModel_003Em__0);
		}
	}

	private IEnumerator ShowUseItemTimePanel(ItemCfg itemCfg)
	{
		if (!Player.PlayerAnimator.GetCurrentAnimatorStateInfo(Player.UpperBodyLayer).IsName(m_TargetAimatorName))
		{
			yield return null;
		}
		if (Player.FSM.CurrentState.ID == StateID.Pa)
		{
			m_UseItemTime = Player.PlayerAnimator.GetCurrentAnimatorStateInfo(Player.FullBodyLayer).length;
		}
		else
		{
			m_UseItemTime = Player.PlayerAnimator.GetCurrentAnimatorStateInfo(Player.UpperBodyLayer).length;
		}
		if (itemCfg.type != 124)
		{
			UseTimePanel.ShowUseTimePanel(m_UseItemTime, Utils.GetString(232, itemCfg.name));
		}
	}

	private void SpeedUpSkill()
	{
		m_speedUpNum = Player.RunSpeedUpSkill(117);
	}

	private void ResetSkillSpeedUp()
	{
		Player.ReduceAnimatorSpeed(m_speedUpNum);
	}

	[CompilerGenerated]
	private void _003CLoadItemModel_003Em__0(GameObject go)
	{
		if (WantUseItemId == -1)
		{
			UnityEngine.Object.DestroyImmediate(go);
			return;
		}
		m_ItemObj = go;
		go.transform.SetParent(Player.RightHandGuaDian.transform, false);
	}
}
