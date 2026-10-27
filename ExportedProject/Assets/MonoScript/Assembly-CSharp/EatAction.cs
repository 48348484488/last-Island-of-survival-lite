using System;
using UnityEngine;
using cfg;
using gs.bag.scmsg;

public class EatAction : TrusteeshipAction
{
	public override TrusteeshipActionType SelfType
	{
		get
		{
			return TrusteeshipActionType.Eat;
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
		BattleEvent.OnUseItemFinish = (Utils.IntDelegate)Delegate.Combine(BattleEvent.OnUseItemFinish, new Utils.IntDelegate(OnEatFinish));
	}

	protected override void DetachEvent()
	{
		BattleEvent.OnUseItemFinish = (Utils.IntDelegate)Delegate.Remove(BattleEvent.OnUseItemFinish, new Utils.IntDelegate(OnEatFinish));
	}

	private void OnEatFinish(int itemId)
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
		Eat(SingletonMono<TrusteeshipMgr>.Ins.FoodId);
	}

	private void Eat(int eatInsId)
	{
		if (eatInsId > 0)
		{
			BagItem allItemByInstanceId = Singleton<BagMgr>.Ins.GetAllItemByInstanceId(eatInsId);
			if (allItemByInstanceId != null)
			{
				ItemCfg itemCfg = ItemCfg.Get(allItemByInstanceId.itemId);
				if (itemCfg != null)
				{
					Singleton<BagMgr>.Ins.OnClickUseItem(eatInsId);
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
