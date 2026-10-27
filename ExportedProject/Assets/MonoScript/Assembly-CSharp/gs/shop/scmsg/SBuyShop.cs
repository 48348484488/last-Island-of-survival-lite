using Net;
using Share;
using gs.drop.scmsg;

namespace gs.shop.scmsg
{
	public class SBuyShop : Message
	{
		public delegate void Handler(SBuyShop msg);

		public const int TYPE = 6294459;

		public static Handler handler;

		public int shopId;

		public int shopNum;

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
			return 6294459;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(shopId);
			oc.push(shopNum);
			oc.push(dropDetail);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			shopId = oc.pop_int();
			shopNum = oc.pop_int();
			oc.pop(dropDetail);
			return oc;
		}
	}
}
