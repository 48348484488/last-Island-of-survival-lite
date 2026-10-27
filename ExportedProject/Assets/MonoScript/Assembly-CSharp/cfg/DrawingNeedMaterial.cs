using Share;

namespace cfg
{
	public class DrawingNeedMaterial
	{
		public int itemId;

		public int num;

		public DrawingNeedMaterial(Octets oc)
		{
			itemId = oc.pop_int();
			num = oc.pop_int();
		}
	}
}
