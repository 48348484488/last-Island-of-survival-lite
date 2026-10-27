using Net;
using Share;

namespace gs.ladder.scmsg
{
	public class CBuyLadderLevel : Message
	{
		public delegate void Handler(CBuyLadderLevel msg);

		public const int TYPE = 19925947;

		public static Handler handler;

		public int level;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 19925947;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(level);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			level = oc.pop_int();
			return oc;
		}
	}
}
