using Share;

namespace cfg
{
	public class DropDetailInfo
	{
		public int dropDetailId;

		public int probability;

		public DropDetailInfo(Octets oc)
		{
			dropDetailId = oc.pop_int();
			probability = oc.pop_int();
		}
	}
}
