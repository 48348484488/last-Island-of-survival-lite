using Net;
using Share;

namespace gs.shop.scmsg
{
	public class SShopShangJia : Message
	{
		public delegate void Handler(SShopShangJia msg);

		public const int TYPE = 6294468;

		public static Handler handler;

		public int shopId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294468;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(shopId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			shopId = oc.pop_int();
			return oc;
		}
	}
}
