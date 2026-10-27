using Net;
using Share;

namespace gs.bag.scmsg
{
	public class CUseItem2 : Message
	{
		public delegate void Handler(CUseItem2 msg);

		public const int TYPE = 8391636;

		public static Handler handler;

		public int itemId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391636;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(itemId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			itemId = oc.pop_int();
			return oc;
		}
	}
}
