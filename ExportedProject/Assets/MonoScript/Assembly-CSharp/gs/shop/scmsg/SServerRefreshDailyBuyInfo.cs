using Net;
using Share;

namespace gs.shop.scmsg
{
	public class SServerRefreshDailyBuyInfo : Message
	{
		public delegate void Handler(SServerRefreshDailyBuyInfo msg);

		public const int TYPE = 6294470;

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
			return 6294470;
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
