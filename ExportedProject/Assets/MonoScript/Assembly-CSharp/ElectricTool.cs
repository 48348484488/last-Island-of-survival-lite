public class ElectricTool
{
	private const int SwitchOnOffFlagBit = 1;

	private const int ConnectToBatteryFlagBit = 2;

	public static bool IsConnectToBattery(int status)
	{
		return (status & 2) > 0;
	}

	public static bool IsSwitchOn(int status)
	{
		return (status & 1) <= 0;
	}
}
