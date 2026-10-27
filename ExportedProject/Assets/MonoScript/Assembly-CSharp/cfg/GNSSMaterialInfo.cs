using Share;

namespace cfg
{
	public class GNSSMaterialInfo
	{
		public int needTime;

		public int dropId;

		public GNSSMaterialInfo(Octets oc)
		{
			needTime = oc.pop_int();
			dropId = oc.pop_int();
		}
	}
}
