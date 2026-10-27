using Share;

namespace cfg
{
	public class OneItemGetWay
	{
		public string btnName;

		public string btnIcon;

		public string desc;

		public string viewName;

		public OneItemGetWay(Octets oc)
		{
			btnName = oc.pop_string();
			btnIcon = oc.pop_string();
			desc = oc.pop_string();
			viewName = oc.pop_string();
		}
	}
}
