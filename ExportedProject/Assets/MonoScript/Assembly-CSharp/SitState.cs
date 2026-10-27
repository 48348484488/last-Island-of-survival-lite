using System;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using SC.UI;
using UnityEngine;
using gs.battle.scmsg;

public class SitState : FSMState
{
	private Transform sitPos;

	public SitState()
	{
		stateID = StateID.Sit;
		ViewMgr.Ins.AddOnShowEvent<BattlePanel>(DelayedAttachEvent);
		SSitDown.handler = (SSitDown.Handler)Delegate.Combine(SSitDown.handler, new SSitDown.Handler(OnSitDown));
	}

	private void OnSitDown(SSitDown msg)
	{
		if (msg.roleId == Singleton<RoleMgr>.Ins.info.roleId)
		{
			Player.FSM.SwitchState(StateID.Sit);
		}
	}

	private void DelayedAttachEvent()
	{
		EventHandlers.OnAimedPart = (Utils.LongDelegate)Delegate.Combine(EventHandlers.OnAimedPart, new Utils.LongDelegate(OnAimedPart));
		EventHandlers.OnClickExtraBtn = (Utils.IntDelegate)Delegate.Combine(EventHandlers.OnClickExtraBtn, new Utils.IntDelegate(ClickBtnCallBack));
		ViewMgr.Ins.RemoveOnShowEvent("BattlePanel", DelayedAttachEvent);
	}

	private void ClickBtnCallBack(int id)
	{
		if (id == 106)
		{
			sitPos = SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.transform.FindChildByNameRecursive("s");
			if (sitPos != null && SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.StatesMsg <= 0)
			{
				CSitDown cSitDown = new CSitDown();
				cSitDown.insId = SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.InsId;
				cSitDown.seatIndex = 0;
				Client2Gs.Ins.Send(cSitDown);
			}
		}
	}

	private void OnAimedPart(long id)
	{
		if (id == -1)
		{
			EventHandlers.OnRemoveExtraBtn(106);
			return;
		}
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(id);
		if (partByInsID != null && partByInsID.MyCfg != null && partByInsID.MyCfg.functionType == 19)
		{
			EventHandlers.OnAddExtraBtn(106);
		}
	}

	public override void DoBeforeEntering(object[] args)
	{
		sitPos = SingletonMono<BuilderBehaviour>.Ins.CurrentAimPart.transform.FindChildByNameRecursive("s");
		if (!(sitPos == null))
		{
			Player.ChangeHandWeapon(-1);
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
			Player.transform.position = sitPos.position;
			Player.ChangeAnimatorStates(Player.BaseLayer, "sit.kongshou_sit_down_yizi");
			Player.SetZaijvRigidbodyValue();
		}
	}

	public override void DoBeforeLeaving()
	{
		Player.ResetRigidbodyValue();
		CGetup msg = new CGetup();
		Client2Gs.Ins.Send(msg);
	}

	public override void Act()
	{
	}

	public override void LateUpdate()
	{
		Player.transform.forward = sitPos.transform.forward;
		Player.NeedHeadIK = false;
	}

	public override void Reason()
	{
		if (Player.FSMUpBody.CurrentState.ID != 0 || Player.Input.magnitude > 0.01f)
		{
			Player.ChangeAnimatorStates(Player.BaseLayer, "sit.kongshou_sit_up_yizi");
		}
		if (Player.IsPlayFinish("sit.kongshou_sit_up_yizi"))
		{
			Player.FSM.SwitchState(StateID.Stand);
		}
	}

	private void PlayAnimation()
	{
	}
}
