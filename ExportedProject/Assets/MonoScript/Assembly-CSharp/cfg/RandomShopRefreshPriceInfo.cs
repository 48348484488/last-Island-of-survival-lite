using Share;

namespace cfg
{
	public class RandomShopRefreshPriceInfo
	{
		public int moneyType;

		public int price;

		public RandomShopRefreshPriceInfo(Octets oc)
		{
			moneyType = oc.pop_int();
			price = oc.pop_int();
		}
	}
}
