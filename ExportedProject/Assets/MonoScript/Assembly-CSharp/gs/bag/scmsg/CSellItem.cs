using Net;
using Share;

namespace gs.bag.scmsg
{
	public class CSellItem : Message
	{
		public delegate void Handler(CSellItem msg);

		public const int TYPE = 8391634;

		public static Handler handler;

		public int itemId;

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
			return 8391634;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(itemId);
			oc.push(number);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			itemId = oc.pop_int();
			number = oc.pop_int();
			return oc;
		}
	}
}
