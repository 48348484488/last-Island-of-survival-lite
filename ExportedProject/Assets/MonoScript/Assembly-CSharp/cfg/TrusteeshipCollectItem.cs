using Share;

namespace cfg
{
	public class TrusteeshipCollectItem
	{
		public int itemId;

		public bool defaultValue;

		public int selfCfgId;

		public string name;

		public TrusteeshipCollectItem(Octets oc)
		{
			itemId = oc.pop_int();
			defaultValue = oc.pop_boolean();
			selfCfgId = oc.pop_int();
			name = oc.pop_string();
		}
	}
}
