using Share;

namespace gs.shop.scmsg
{
	public class RandomShopItemInfo : Marshal
	{
		public int shopId;

		public int number;

		public Octets marshal(Octets oc)
		{
			oc.push(shopId);
			oc.push(number);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			shopId = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}
