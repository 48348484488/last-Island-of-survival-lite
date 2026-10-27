using Share;

namespace cfg
{
	public class GNSSFuelInfo
	{
		public int needTime;

		public GNSSFuelInfo(Octets oc)
		{
			needTime = oc.pop_int();
		}
	}
}
