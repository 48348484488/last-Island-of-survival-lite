using Net;
using Share;

namespace gs.ladder.scmsg
{
	public class SBuyLadderLevel : Message
	{
		public delegate void Handler(SBuyLadderLevel msg);

		public const int TYPE = 19925948;

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
			return 19925948;
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
