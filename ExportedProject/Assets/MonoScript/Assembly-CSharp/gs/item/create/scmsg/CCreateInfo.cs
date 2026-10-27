using Net;
using Share;

namespace gs.item.create.scmsg
{
	public class CCreateInfo : Message
	{
		public delegate void Handler(CCreateInfo msg);

		public const int TYPE = 14683064;

		public static Handler handler;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 14683064;
		}

		public override Octets marshal(Octets oc)
		{
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			return oc;
		}
	}
}
