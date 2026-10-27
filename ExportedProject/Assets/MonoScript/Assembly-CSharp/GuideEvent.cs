using System;
using gs.task.scmsg;

public class GuideEvent
{
	public static Action StepForwardAction;

	public static Utils.VoidDelegate StepCompleteAction;

	public static Action ShowGuideTipsAction;

	public static Action<TaskInfo> TaskProgressChangeAction;

	public static Action<bool> GuideEffectActiveNeedChangeAction;

	public static Action<float> UpdateProtectTimeAction;

	public static Action GuideTotalFinishAction;

	public static Action StepInNotMeetConditionTask;
}
