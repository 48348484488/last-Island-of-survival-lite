using Share;

namespace cfg
{
	public class WorkbenchNeedMaterial
	{
		public int itemId;

		public int num;

		public WorkbenchNeedMaterial(Octets oc)
		{
			itemId = oc.pop_int();
			num = oc.pop_int();
		}
	}
}
