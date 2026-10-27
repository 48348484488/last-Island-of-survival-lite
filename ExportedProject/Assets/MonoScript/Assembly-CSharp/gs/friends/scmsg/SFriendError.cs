using Net;
using Share;

namespace gs.friends.scmsg
{
	public class SFriendError : Message
	{
		public delegate void Handler(SFriendError msg);

		public const int TYPE = 13634501;

		public static Handler handler;

		public const int IS_BLACK_NOT_CARE = 1;

		public const int NOT_SEARCH_ROLEID = 2;

		public int code;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634501;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(code);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			code = oc.pop_int();
			return oc;
		}
	}
}
