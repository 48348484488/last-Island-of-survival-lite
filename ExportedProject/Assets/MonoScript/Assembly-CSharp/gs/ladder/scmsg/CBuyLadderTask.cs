using Net;
using Share;

namespace gs.ladder.scmsg
{
	public class CBuyLadderTask : Message
	{
		public delegate void Handler(CBuyLadderTask msg);

		public const int TYPE = 19925945;

		public static Handler handler;

		public int type;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 19925945;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(type);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			type = oc.pop_int();
			return oc;
		}
	}
}
