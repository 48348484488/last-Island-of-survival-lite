using Net;
using Share;

namespace gs.friends.scmsg
{
	public class CFriendInfo : Message
	{
		public delegate void Handler(CFriendInfo msg);

		public const int TYPE = 13634508;

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
			return 13634508;
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
