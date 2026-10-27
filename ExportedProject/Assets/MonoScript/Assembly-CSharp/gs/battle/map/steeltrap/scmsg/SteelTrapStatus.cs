using Share;

namespace gs.battle.map.steeltrap.scmsg
{
	public class SteelTrapStatus : Marshal
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
