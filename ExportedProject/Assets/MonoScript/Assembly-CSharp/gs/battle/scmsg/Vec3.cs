using Share;

namespace gs.battle.scmsg
{
	public class Vec3 : Marshal
	{
		public float x;

		public float y;

		public float z;

		public Octets marshal(Octets oc)
		{
			oc.push(x);
			oc.push(y);
			oc.push(z);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			x = oc.pop_float();
			y = oc.pop_float();
			z = oc.pop_float();
			return oc;
		}
	}
}
