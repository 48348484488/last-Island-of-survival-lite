using Share;

namespace cfg
{
	public class MaterialInfo
	{
		public int itemId;

		public int needNum;

		public int hp;

		public int returnNum;

		public MaterialInfo(Octets oc)
		{
			itemId = oc.pop_int();
			needNum = oc.pop_int();
			hp = oc.pop_int();
			returnNum = oc.pop_int();
		}
	}
}
