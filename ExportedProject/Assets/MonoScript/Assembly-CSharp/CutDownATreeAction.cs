using UnityEngine;

public class CutDownATreeAction : TrusteeshipAction
{
	public override TrusteeshipActionType SelfType
	{
		get
		{
			return TrusteeshipActionType.CutDownATree;
		}
	}

	protected override Vector3 LookAtDir
	{
		get
		{
			return TargetTrans.position + Vector3.up - SingletonMono<TrusteeshipMgr>.Ins.SelfMainCameraTrans.position;
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
		SingletonMono<TrusteeshipMgr>.Ins.IntervalTime = 0.5f;
		SingletonMono<TrusteeshipMgr>.Ins.SetNextAction(TrusteeshipActionType.FindTarget);
	}

	protected override void StopAction()
	{
	}

	protected override bool NeedChangeAction()
	{
		if (SingletonMono<TrusteeshipMgr>.Ins.AimInsId < 0)
		{
			return true;
		}
		BattleEvent.TrusteeshipFire();
		return !TargetTrans || !TargetGo.activeSelf;
	}
}
