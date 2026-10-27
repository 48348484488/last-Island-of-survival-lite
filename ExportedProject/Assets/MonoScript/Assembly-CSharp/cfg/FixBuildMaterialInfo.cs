using Share;

namespace cfg
{
	public class FixBuildMaterialInfo
	{
		public int itemId;

		public int needNum;

		public FixBuildMaterialInfo(Octets oc)
		{
			itemId = oc.pop_int();
			needNum = oc.pop_int();
		}
	}
}
