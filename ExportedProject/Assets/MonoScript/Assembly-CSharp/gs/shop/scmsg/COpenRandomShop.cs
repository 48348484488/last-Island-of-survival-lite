using Net;
using Share;

namespace gs.shop.scmsg
{
	public class COpenRandomShop : Message
	{
		public delegate void Handler(COpenRandomShop msg);

		public const int TYPE = 6294462;

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
			return 6294462;
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
