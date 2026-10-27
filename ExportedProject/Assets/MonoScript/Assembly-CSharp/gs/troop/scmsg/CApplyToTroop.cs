using Net;
using Share;

namespace gs.troop.scmsg
{
	public class CApplyToTroop : Message
	{
		public delegate void Handler(CApplyToTroop msg);

		public const int TYPE = 15731651;

		public static Handler handler;

		public long otherId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731651;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherId = oc.pop_long();
			return oc;
		}
	}
}
