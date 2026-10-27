using System;

public class TrusteeshipEvent
{
	public static Action UpdateFinishDelegate;

	public static Func<bool> UpdateNeedChangeDelegate;

	public static Action<bool> TrusteeshipOnOffDelegate;
}
