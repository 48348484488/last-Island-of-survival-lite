using Share;

namespace cfg
{
	public class RandomShopRefreshInfo
	{
		public int shopId;

		public int number;

		public RandomShopRefreshInfo(Octets oc)
		{
			shopId = oc.pop_int();
			number = oc.pop_int();
		}
	}
}
