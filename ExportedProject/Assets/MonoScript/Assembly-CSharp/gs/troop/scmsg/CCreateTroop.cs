using Net;
using Share;

namespace gs.troop.scmsg
{
	public class CCreateTroop : Message
	{
		public delegate void Handler(CCreateTroop msg);

		public const int TYPE = 15731642;

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
			return 15731642;
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
