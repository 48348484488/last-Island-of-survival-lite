using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class CCancelCreate : Message
	{
		public delegate void Handler(CCancelCreate msg);

		public const int TYPE = 14683069;

		public static Handler handler;

		public int cancelIndex;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 14683069;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(cancelIndex);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			cancelIndex = oc.pop_int();
			return oc;
		}
	}
}
