using Share;

namespace gs.battle.scmsg
{
	public class Zone : Marshal
	{
		public float centerX;

		public float centerY;

		public float radius;

		public Octets marshal(Octets oc)
		{
			oc.push(centerX);
			oc.push(centerY);
			oc.push(radius);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			centerX = oc.pop_float();
			centerY = oc.pop_float();
			radius = oc.pop_float();
			return oc;
		}
	}
}
