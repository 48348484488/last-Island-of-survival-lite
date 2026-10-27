using Net;
using Share;

namespace gs.shop.scmsg
{
	public class SShopXiaJia : Message
	{
		public delegate void Handler(SShopXiaJia msg);

		public const int TYPE = 6294469;

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
			return 6294469;
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
