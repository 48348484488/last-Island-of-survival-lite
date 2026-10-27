using Share;

namespace gs.battle.scmsg
{
	public class Vec2 : Marshal
	{
		public float x;

		public float z;

		public Octets marshal(Octets oc)
		{
			oc.push(x);
			oc.push(z);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			x = oc.pop_float();
			z = oc.pop_float();
			return oc;
		}
	}
}
