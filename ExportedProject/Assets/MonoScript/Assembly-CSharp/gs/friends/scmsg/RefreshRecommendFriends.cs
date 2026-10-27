using Net;
using Share;

namespace gs.friends.scmsg
{
	public class RefreshRecommendFriends : Message
	{
		public delegate void Handler(RefreshRecommendFriends msg);

		public const int TYPE = 13634514;

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
			return 13634514;
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
