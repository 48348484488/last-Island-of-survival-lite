using Share;

namespace cfg
{
	public class FixDrop
	{
		public int dropType;

		public int id;

		public int value;

		public FixDrop(Octets oc)
		{
			dropType = oc.pop_int();
			id = oc.pop_int();
			value = oc.pop_int();
		}
	}
}
