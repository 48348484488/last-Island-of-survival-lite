using Net;
using Share;

namespace gs.bag.scmsg
{
	public class SBagSizeChange : Message
	{
		public delegate void Handler(SBagSizeChange msg);

		public const int TYPE = 8391657;

		public static Handler handler;

		public int size;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 8391657;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(size);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			size = oc.pop_int();
			return oc;
		}
	}
}
