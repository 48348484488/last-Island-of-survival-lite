using Share;

namespace cfg
{
	public class MineToolInfo
	{
		public int toolItemId;

		public MineToolInfo(Octets oc)
		{
			toolItemId = oc.pop_int();
		}
	}
}
