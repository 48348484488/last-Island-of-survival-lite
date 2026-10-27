using Net;
using Share;

namespace gs.shop.scmsg
{
	public class CBuyShop : Message
	{
		public delegate void Handler(CBuyShop msg);

		public const int TYPE = 6294458;

		public static Handler handler;

		public int shopId;

		public int shopNum;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294458;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(shopId);
			oc.push(shopNum);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			shopId = oc.pop_int();
			shopNum = oc.pop_int();
			return oc;
		}
	}
}
