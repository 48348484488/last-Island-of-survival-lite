using Share;

namespace gs.battle.scmsg
{
	public class ShortVec3 : Marshal
	{
		public short x;

		public short y;

		public short z;

		public Octets marshal(Octets oc)
		{
			oc.push(x);
			oc.push(y);
			oc.push(z);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			x = oc.pop_short();
			y = oc.pop_short();
			z = oc.pop_short();
			return oc;
		}
	}
}
