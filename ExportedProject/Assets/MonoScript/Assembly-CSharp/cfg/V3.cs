using Share;

namespace cfg
{
	public class V3
	{
		public float x;

		public float y;

		public float z;

		public V3(Octets oc)
		{
			x = oc.pop_float();
			y = oc.pop_float();
			z = oc.pop_float();
		}
	}
}
