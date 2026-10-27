using Share;

namespace gs.chat.scmsg
{
	public class MsgType : Marshal
	{
		public const int WORLD = 1;

		public const int SYSTEM = 2;

		public const int TROOP = 3;

		public const int FAMILY = 4;

		public const int HORN_SMALL = 5;

		public const int HORN_BIG = 6;

		public const int NOTICE = 7;

		public const int TROOP_RECRUIT = 8;

		public const int ZHANDUI_RECRUIT = 9;

		public Octets marshal(Octets oc)
		{
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			return oc;
		}
	}
}
