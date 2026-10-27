using SC.UI;
using UnityEngine;
using cfg;

public class PickItemAction : TrusteeshipAction
{
	public override TrusteeshipActionType SelfType
	{
		get
		{
			return TrusteeshipActionType.PickItem;
		}
	}

	protected override Vector3 LookAtDir
	{
		get
		{
			return SingletonMono<TrusteeshipMgr>.Ins.SelfMainCameraTrans.forward;
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
			return 0.05f;
		}
	}

	protected override void AttachEvent()
	{
	}

	protected override void DetachEvent()
	{
	}

	protected override void StartAction(Vector3 selfPos)
	{
		SingletonMono<TrusteeshipMgr>.Ins.IntervalTime = 0.5f;
		SingletonMono<TrusteeshipMgr>.Ins.SetNextAction(TrusteeshipActionType.FindTarget);
	}

	protected override void StopAction()
	{
	}

	protected override bool NeedChangeAction()
	{
		int totalCellNum = Singleton<BattleDropMgr>.Ins.GetTotalCellNum();
		int num = 0;
		for (int i = 0; i < totalCellNum; i++)
		{
			int itemId;
			int num2;
			long boxInstanceId;
			bool idAndNumber = Singleton<BattleDropMgr>.Ins.GetIdAndNumber(i, out itemId, out num2, out boxInstanceId);
			ItemCfg itemCfg = ItemCfg.Get(itemId);
			BoxCfg boxCfgInfo = BoxCfg.Get(-itemId);
			if (itemCfg != null && itemId != ConstsBs.DEAD_BOX_ITEM_ID && !SingletonMono<TrusteeshipMgr>.Ins.GetSettingPick(itemCfg.type))
			{
				num++;
				continue;
			}
			if (!PickPanel.PickItem(itemCfg, boxCfgInfo, i, itemId, num2, boxInstanceId, idAndNumber))
			{
				SingletonMono<TrusteeshipMgr>.Ins.BagHaveCapacity = false;
			}
			break;
		}
		return Singleton<BattleDropMgr>.Ins.IsAllPicked() || num == totalCellNum;
	}
}
