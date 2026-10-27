using Share;

namespace gs.battle.scmsg
{
	public class DoorStatus : Marshal
	{
		public const int OPEN_CLOSE = 1;

		public const int OPEN_DIRECTION = 2;

		public const int HAVE_LOCK = 4;

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
