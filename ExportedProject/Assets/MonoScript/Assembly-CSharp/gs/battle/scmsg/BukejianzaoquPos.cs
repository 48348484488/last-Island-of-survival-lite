using Share;

namespace gs.battle.scmsg
{
	public class BukejianzaoquPos : Marshal
	{
		public float x;

		public float z;

		public float r;

		public Octets marshal(Octets oc)
		{
			oc.push(x);
			oc.push(z);
			oc.push(r);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			x = oc.pop_float();
			z = oc.pop_float();
			r = oc.pop_float();
			return oc;
		}
	}
}
