using Net;
using Share;

namespace gs.shop.scmsg
{
	public class SBuyGold : Message
	{
		public delegate void Handler(SBuyGold msg);

		public const int TYPE = 6294473;

		public static Handler handler;

		public int cuponNum;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294473;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(cuponNum);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			cuponNum = oc.pop_int();
			return oc;
		}
	}
}
