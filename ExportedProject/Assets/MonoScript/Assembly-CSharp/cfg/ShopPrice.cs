using Share;

namespace cfg
{
	public class ShopPrice
	{
		public int day;

		public int gold;

		public int coupon;

		public ShopPrice(Octets oc)
		{
			day = oc.pop_int();
			gold = oc.pop_int();
			coupon = oc.pop_int();
		}
	}
}
