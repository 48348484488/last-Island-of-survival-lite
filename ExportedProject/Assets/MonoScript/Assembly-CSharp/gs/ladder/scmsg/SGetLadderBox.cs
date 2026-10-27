using Net;
using Share;

namespace gs.ladder.scmsg
{
	public class SGetLadderBox : Message
	{
		public delegate void Handler(SGetLadderBox msg);

		public const int TYPE = 19925953;

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
			return 19925953;
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
