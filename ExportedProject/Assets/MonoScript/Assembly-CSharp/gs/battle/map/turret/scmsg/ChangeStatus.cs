using Share;

namespace gs.battle.map.turret.scmsg
{
	public class ChangeStatus : Marshal
	{
		public const int open = 0;

		public const int close = 1;

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
