using UnityEngine;

public class HitTrashcanAction : TrusteeshipAction
{
	public override TrusteeshipActionType SelfType
	{
		get
		{
			return TrusteeshipActionType.HitTrashcan;
		}
	}

	protected override Vector3 LookAtDir
	{
		get
		{
			return TargetTrans.position + 0.5f * TargetTrans.up - SingletonMono<TrusteeshipMgr>.Ins.SelfHeadTrans.position;
		}
	}

	protected override bool NeedTurnCamera
	{
		get
		{
			return true;
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
		SingletonMono<TrusteeshipMgr>.Ins.PutWeaponToHand();
		SingletonMono<TrusteeshipMgr>.Ins.SetNextAction(TrusteeshipActionType.PickItem);
	}

	protected override void StopAction()
	{
	}

	protected override bool NeedChangeAction()
	{
		BattleEvent.TrusteeshipFire();
		return !TargetTrans || !TargetGo.activeSelf;
	}
}
