using Net;
using Share;

namespace gs.friends.scmsg
{
	public class CFindManitoPlayer : Message
	{
		public delegate void Handler(CFindManitoPlayer msg);

		public const int TYPE = 13634506;

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
			return 13634506;
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
