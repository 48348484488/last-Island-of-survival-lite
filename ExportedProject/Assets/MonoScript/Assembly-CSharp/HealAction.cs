using System;
using UnityEngine;
using cfg;
using gs.bag.scmsg;

public class HealAction : TrusteeshipAction
{
	public override TrusteeshipActionType SelfType
	{
		get
		{
			return TrusteeshipActionType.Heal;
		}
	}

	protected override Vector3 LookAtDir
	{
		get
		{
			return Vector3.zero;
		}
	}

	protected override bool NeedTurnCamera
	{
		get
		{
			return false;
		}
	}

	protected override float WaitTurnFinishTime
	{
		get
		{
			return 10f;
		}
	}

	protected override void AttachEvent()
	{
		BattleEvent.OnUseItemFinish = (Utils.IntDelegate)Delegate.Combine(BattleEvent.OnUseItemFinish, new Utils.IntDelegate(OnHealFinish));
	}

	protected override void DetachEvent()
	{
		BattleEvent.OnUseItemFinish = (Utils.IntDelegate)Delegate.Remove(BattleEvent.OnUseItemFinish, new Utils.IntDelegate(OnHealFinish));
	}

	private void OnHealFinish(int itemId)
	{
		if (itemId < 0)
		{
			SingletonMono<TrusteeshipMgr>.Ins.StopTrusteeship();
		}
		else
		{
			SingletonMono<TrusteeshipMgr>.Ins.ChangeActionWithDelay();
		}
	}

	protected override void StartAction(Vector3 selfPos)
	{
		SingletonMono<TrusteeshipMgr>.Ins.IntervalTime = 100f;
		SingletonMono<TrusteeshipMgr>.Ins.SetNextAction(TrusteeshipActionType.FindTarget);
		Heal(SingletonMono<TrusteeshipMgr>.Ins.MedicineId);
	}

	private void Heal(int healInsId)
	{
		if (healInsId > 0)
		{
			BagItem allItemByInstanceId = Singleton<BagMgr>.Ins.GetAllItemByInstanceId(healInsId);
			if (allItemByInstanceId != null)
			{
				ItemCfg itemCfg = ItemCfg.Get(allItemByInstanceId.itemId);
				if (itemCfg != null)
				{
					Singleton<BagMgr>.Ins.OnClickUseItem(healInsId);
					return;
				}
			}
		}
		SingletonMono<TrusteeshipMgr>.Ins.ChangeTrusteeshipAction();
	}

	protected override void StopAction()
	{
	}

	protected override bool NeedChangeAction()
	{
		return true;
	}
}
