using System;
using System.Collections.Generic;
using gs.battle.map.circuitry.scmsg;

public class ElectricityEvent
{
	public static Action<long, long> ConnectDelegate;

	public static Action<long, long> DisconnectDelegate;

	public static Action<long, int, Dictionary<byte, PowerFuleInfo>> UpdateFuelInfoDelegate;

	public static Action OnSwitchStateChangeDelegate;

	public static Action<long, byte> OnSwitchDelayTimeRefreshDelegate;

	public static Action OnSwitchDelayTimeChangeSuccessDelegate;
}
