using Share;

namespace gs.role.scmsg
{
	public class OnlineType : Marshal
	{
		public const int Offline = 0;

		public const int Online = 1;

		public const int Troop = 2;

		public const int Match = 16;

		public const int Battle = 4;

		public const int Leave = 8;

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
