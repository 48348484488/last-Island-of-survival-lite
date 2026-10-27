using Share;

namespace cfg
{
	public class V2
	{
		public float x;

		public float y;

		public V2(Octets oc)
		{
			x = oc.pop_float();
			y = oc.pop_float();
		}
	}
}
