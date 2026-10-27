using Share;

namespace cfg
{
	public class CutTreeToolInfo
	{
		public int cutTreeToolItemId;

		public CutTreeToolInfo(Octets oc)
		{
			cutTreeToolItemId = oc.pop_int();
		}
	}
}
