using Share;

namespace cfg
{
	public class ShopBuyConditionArg
	{
		public int arg1;

		public int arg2;

		public ShopBuyConditionArg(Octets oc)
		{
			arg1 = oc.pop_int();
			arg2 = oc.pop_int();
		}
	}
}
