using Net;
using Share;
using gs.drop.scmsg;

namespace gs.shop.scmsg
{
	public class SGetFirstChargeGift : Message
	{
		public delegate void Handler(SGetFirstChargeGift msg);

		public const int TYPE = 6294474;

		public static Handler handler;

		public DropDetail dropDetail = new DropDetail();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294474;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(dropDetail);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(dropDetail);
			return oc;
		}
	}
}
