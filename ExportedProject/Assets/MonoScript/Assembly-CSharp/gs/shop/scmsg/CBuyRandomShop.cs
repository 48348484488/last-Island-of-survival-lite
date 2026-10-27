using Net;
using Share;

namespace gs.shop.scmsg
{
	public class CBuyRandomShop : Message
	{
		public delegate void Handler(CBuyRandomShop msg);

		public const int TYPE = 6294464;

		public static Handler handler;

		public int index;

		public int number;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 6294464;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(index);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			index = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}
