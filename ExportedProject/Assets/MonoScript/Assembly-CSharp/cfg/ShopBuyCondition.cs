using Share;

namespace cfg
{
	public class ShopBuyCondition
	{
		public string conditionType;

		public int arg1;

		public int arg2;

		public ShopBuyCondition(Octets oc)
		{
			conditionType = oc.pop_string();
			arg1 = oc.pop_int();
			arg2 = oc.pop_int();
		}
	}
}
