using System.Collections;
using EasyBuildSystem.Runtimes.Internal.Builder;
using SC.UI;
using UnityEngine;
using gs.battle.scmsg;

public class BuildState : FSMState
{
	private int m_WantGetGunInsId;

	private Coroutine m_Coroutine;

	private CEnterBuildState cEnterMsg = new CEnterBuildState();

	private CExitBuildState cExitMsg = new CExitBuildState();

	public static bool m_FromQuickUse;

	public BuildState()
	{
		stateID = StateID.Build;
		BattleEvent.OnWantEnterBuildState = OnWantEnterBuildState;
		BattleEvent.OnWantExitBuildState = OnWantExitBuildState;
	}

	private void OnWantExitBuildState(bool fromQuickUse)
	{
		m_FromQuickUse = fromQuickUse;
		if (Player.FSMUpBody.CurrentState.ID == StateID.NullStateID)
		{
			Utils.TriggerEvent(BattleEvent.OnExitedBuildState);
		}
		else
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
		}
	}

	private void OnWantEnterBuildState(bool fromQuickUse)
	{
		m_FromQuickUse = fromQuickUse;
		if (!Player.CheckCanEnterBuildState())
		{
			AlertBox.Show(Utils.GetString(137));
		}
		else
		{
			WaitShouGun(StateID.Build);
		}
	}

	private void WaitShouGun(StateID stateID)
	{
		if (Player.GetCurrentWeapon() != null)
		{
			Player.ChangeHandWeapon(-1);
		}
		Player.FSMUpBody.SwitchState(stateID);
	}

	private IEnumerator PrePareUseItem(StateID stateID)
	{
		while (Player.FSMUpBody.CurrentState.ID != 0 && Player.FSMUpBody.CurrentState.ID != StateID.Nawuqi)
		{
			yield return null;
		}
		Player.FSMUpBody.SwtichStateReStart(stateID);
	}

	public override void DoBeforeEntering(object[] args)
	{
		if (m_FromQuickUse)
		{
			Utils.TriggerEvent(BattleEvent.OnEnteredBuildStateFromQuickUse);
		}
		else
		{
			Utils.TriggerEvent(BattleEvent.OnEnteredBuildState);
		}
		SingletonMono<BuilderBehaviour>.Ins.ResetAimPreview();
		PlayAnimation();
	}

	private void Finish()
	{
		Battle.Ins.SelfPlayer.SoundControll.PlaySound(131);
	}

	public override void DoBeforeLeaving()
	{
		if (m_FromQuickUse)
		{
			Utils.TriggerEvent(BattleEvent.OnEnteredBuildStateFromQuickUse);
		}
		else
		{
			Utils.TriggerEvent(BattleEvent.OnExitedBuildState);
		}
		Player.HideTuzhi();
		Client2Gs.Ins.Send(cExitMsg);
		Player.ChangeAnimatorStates(Player.UpperBodyLayer, "Null");
	}

	public override void Act()
	{
		Player.m_SpeedMultiple = 2f;
	}

	public override void Reason()
	{
	}

	private void PlayAnimation()
	{
		if (Player.FSM.CurrentState.ID == StateID.Stand || Player.FSM.CurrentState.ID == StateID.Crouch)
		{
			m_TargetAimatorName = "Build.kongshou_zhan_jianzao";
			Player.ChangeAnimatorStates(Player.UpperBodyLayer, m_TargetAimatorName);
			Player.ShowTuzhi();
			Client2Gs.Ins.Send(cEnterMsg);
		}
	}
}
