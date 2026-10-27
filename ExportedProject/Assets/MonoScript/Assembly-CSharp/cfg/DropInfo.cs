using Share;

namespace cfg
{
	public class DropInfo
	{
		public int dropType;

		public int value;

		public int num;

		public int probability;

		public DropInfo(Octets oc)
		{
			dropType = oc.pop_int();
			value = oc.pop_int();
			num = oc.pop_int();
			probability = oc.pop_int();
		}
	}
}
