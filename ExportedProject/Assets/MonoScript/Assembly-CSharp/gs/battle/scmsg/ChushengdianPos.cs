using Share;

namespace gs.battle.scmsg
{
	public class ChushengdianPos : Marshal
	{
		public float x;

		public float z;

		public float r;

		public bool bchushengdian;

		public Octets marshal(Octets oc)
		{
			oc.push(x);
			oc.push(z);
			oc.push(r);
			oc.push(bchushengdian);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			x = oc.pop_float();
			z = oc.pop_float();
			r = oc.pop_float();
			bchushengdian = oc.pop_bool();
			return oc;
		}
	}
}
