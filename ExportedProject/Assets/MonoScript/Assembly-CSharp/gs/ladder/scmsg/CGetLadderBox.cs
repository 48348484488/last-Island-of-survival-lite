using Net;
using Share;

namespace gs.ladder.scmsg
{
	public class CGetLadderBox : Message
	{
		public delegate void Handler(CGetLadderBox msg);

		public const int TYPE = 19925952;

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
			return 19925952;
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
