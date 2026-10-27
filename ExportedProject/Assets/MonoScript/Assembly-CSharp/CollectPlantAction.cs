using EasyBuildSystem.Runtimes.Events;
using UnityEngine;

public class CollectPlantAction : TrusteeshipAction
{
	private bool _collecting;

	public override TrusteeshipActionType SelfType
	{
		get
		{
			return TrusteeshipActionType.CollectPlant;
		}
	}

	protected override Vector3 LookAtDir
	{
		get
		{
			return TargetTrans.position - (SingletonMono<TrusteeshipMgr>.Ins.SelfHeadTrans.position - SingletonMono<TrusteeshipMgr>.Ins.SelfHeadTrans.InverseTransformVector(Vector3.forward * 0.15f));
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
		_collecting = false;
		SingletonMono<TrusteeshipMgr>.Ins.IntervalTime = 0.5f;
		SingletonMono<TrusteeshipMgr>.Ins.SetNextAction(TrusteeshipActionType.FindTarget);
	}

	protected override void StopAction()
	{
	}

	protected override bool NeedChangeAction()
	{
		if (!_collecting)
		{
			_collecting = true;
			EventHandlers.OnClickExtraBtn(110);
		}
		if (SingletonMono<TrusteeshipMgr>.Ins.AimInsId < 0)
		{
			return true;
		}
		return !TargetTrans || !TargetGo.activeSelf;
	}
}
