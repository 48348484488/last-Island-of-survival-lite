using Net;
using Share;

namespace gs.shop.scmsg
{
	public class CRefreshRandomShop : Message
	{
		public delegate void Handler(CRefreshRandomShop msg);

		public const int TYPE = 6294466;

		public static Handler handler;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294466;
		}

		public override Octets marshal(Octets oc)
		{
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			return oc;
		}
	}
}
