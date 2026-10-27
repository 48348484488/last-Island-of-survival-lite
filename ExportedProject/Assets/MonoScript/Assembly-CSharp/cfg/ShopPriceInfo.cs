using Share;

namespace cfg
{
	public class ShopPriceInfo
	{
		public string name;

		public float price;

		public int num;

		public int firstZengType;

		public int firstZengArg;

		public int zengType;

		public int zengArg;

		public int vipExp;

		public int dropitemId;

		public int gotDays;

		public ShopPriceInfo(Octets oc)
		{
			name = oc.pop_string();
			price = oc.pop_float();
			num = oc.pop_int();
			firstZengType = oc.pop_int();
			firstZengArg = oc.pop_int();
			zengType = oc.pop_int();
			zengArg = oc.pop_int();
			vipExp = oc.pop_int();
			dropitemId = oc.pop_int();
			gotDays = oc.pop_int();
		}
	}
}
