using Share;

namespace cfg
{
	public class GunLevelUpItemInfo
	{
		public int itemId;

		public int num;

		public GunLevelUpItemInfo(Octets oc)
		{
			itemId = oc.pop_int();
			num = oc.pop_int();
		}
	}
}
