using Net;
using Share;
using gs.drop.scmsg;

namespace gs.bag.scmsg
{
	public class SSellItem : Message
	{
		public delegate void Handler(SSellItem msg);

		public const int TYPE = 8391635;

		public static Handler handler;

		public int itemId;

		public int number;

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
			return 8391635;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(itemId);
			oc.push(number);
			oc.push(dropDetail);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			itemId = oc.pop_int();
			number = oc.pop_int();
			oc.pop(dropDetail);
			return oc;
		}
	}
}
