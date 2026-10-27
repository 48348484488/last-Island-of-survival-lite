using Share;

namespace cfg
{
	public class GunProperty
	{
		public string level;

		public int score;

		public GunProperty(Octets oc)
		{
			level = oc.pop_string();
			score = oc.pop_int();
		}
	}
}
