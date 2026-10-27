using System;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

public class PlantState : FSMState
{
	public PlantState()
	{
		stateID = StateID.Plant;
		BattleEvent.OnClickBuildBtn = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnClickBuildBtn, new Utils.VoidDelegate(OnClickBuildBtn));
	}

	private void OnClickBuildBtn()
	{
		if (Player.InPlantState && SingletonMono<PlantMgr>.Ins.CurPreviewPlant != null && SingletonMono<PlantMgr>.Ins.AllowPlacement)
		{
			CPlant cPlant = new CPlant();
			cPlant.plantId = (int)ItemCfg.Get(SingletonMono<PlantMgr>.Ins.CurPreviewPlant.CfgId).extras[0];
			cPlant.pos.x = SingletonMono<PlantMgr>.Ins.CurPreviewPlant.transform.position.x;
			cPlant.pos.y = SingletonMono<PlantMgr>.Ins.CurPreviewPlant.transform.position.y;
			cPlant.pos.z = SingletonMono<PlantMgr>.Ins.CurPreviewPlant.transform.position.z;
			cPlant.orientation.x = SingletonMono<PlantMgr>.Ins.CurPreviewPlant.transform.eulerAngles.x;
			cPlant.orientation.y = SingletonMono<PlantMgr>.Ins.CurPreviewPlant.transform.eulerAngles.y;
			cPlant.orientation.z = SingletonMono<PlantMgr>.Ins.CurPreviewPlant.transform.eulerAngles.z;
			Client2Gs.Ins.Send(cPlant);
			UnityEngine.Object.Destroy(SingletonMono<PlantMgr>.Ins.CurPreviewPlant.gameObject);
		}
	}

	public override void DoBeforeEntering(object[] args)
	{
		Player.ChangeHandWeapon(-1);
		Player.ChangeAnimatorStates(Player.UpperBodyLayer, "Plant.kongshou_zhan_zhongzhi");
	}

	public override void DoBeforeLeaving()
	{
		if (SingletonMono<PlantMgr>.Ins.CurPreviewPlant != null)
		{
			UnityEngine.Object.Destroy(SingletonMono<PlantMgr>.Ins.CurPreviewPlant.gameObject);
		}
	}

	public override void Act()
	{
	}

	public override void LateUpdate()
	{
	}

	public override void Reason()
	{
		if (SingletonMono<PlantMgr>.Ins.CurPreviewPlant == null)
		{
			Player.FSMUpBody.SwitchState(StateID.NullStateID);
		}
	}

	private void PlayAnimation()
	{
	}
}
