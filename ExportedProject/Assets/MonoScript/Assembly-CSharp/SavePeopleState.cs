using System;
using System.Collections;
using UnityEngine;
using gs.battle.scmsg;

public class SavePeopleState : FSMState
{
	private long m_Rold;

	private StateID m_BeforeEnterFsmStateId;

	private StateID m_BeforeEnterFsmUpStateId;

	public SavePeopleState()
	{
		stateID = StateID.SavePeople;
	}

	private void OnSyncPlayerSave(SSyncPlayerSave msg)
	{
		if (msg.roleId == m_Rold)
		{
			Player.FSM.SwitchState(m_BeforeEnterFsmStateId);
		}
	}

	public override void DoBeforeEntering(object[] args)
	{
		if (args.Length > 0)
		{
			m_BeforeEnterFsmStateId = (StateID)args[0];
		}
		if (Battle.Ins.SelfPlayer.CurGun != null)
		{
			Battle.Ins.SelfPlayer.CurGun.StopShoot();
		}
		Player.CloseAutoRun();
		Player.MainCamera.ChangeState("Crouch");
		Player.FSMUpBody.SwitchState(StateID.NullStateID);
		SSyncPlayerSave.handler = (SSyncPlayerSave.Handler)Delegate.Combine(SSyncPlayerSave.handler, new SSyncPlayerSave.Handler(OnSyncPlayerSave));
		m_BeforeEnterFsmUpStateId = Player.FSMUpBody.CurrentState.ID;
		m_Rold = Player.CanSavedPeople.RoleId;
		CStartSavePlayer cStartSavePlayer = new CStartSavePlayer();
		cStartSavePlayer.roleId = m_Rold;
		Client2Gs.Ins.Send(cStartSavePlayer);
		Player.ChangeAnimatorStates(Player.BaseLayer, "SavePeople.kongshou_pa_shoushang_idle02");
	}

	public override void DoBeforeLeaving()
	{
		CInterruptSavePlayer cInterruptSavePlayer = new CInterruptSavePlayer();
		cInterruptSavePlayer.roleId = m_Rold;
		Client2Gs.Ins.Send(cInterruptSavePlayer);
		SSyncPlayerSave.handler = (SSyncPlayerSave.Handler)Delegate.Remove(SSyncPlayerSave.handler, new SSyncPlayerSave.Handler(OnSyncPlayerSave));
	}

	public override IEnumerator ActCoroutine()
	{
		yield return new WaitForSeconds(11f);
		Player.FSM.SwitchState(StateID.Crouch);
	}

	public override void Act()
	{
		Player.CanOpenJimiao = false;
		if (Player.FSMUpBody.CurrentState.ID != m_BeforeEnterFsmUpStateId || Math.Abs(Player.Input.magnitude) > 0.0001f || Player.CanSavedPeople == null)
		{
			Player.FSM.SwitchState(m_BeforeEnterFsmStateId);
		}
	}

	public override void Reason()
	{
	}
}
