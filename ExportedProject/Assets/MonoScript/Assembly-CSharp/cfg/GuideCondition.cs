using Share;

namespace cfg
{
	public class GuideCondition
	{
		public int conditionIntArg;

		public string conditionStrArg;

		public int nextIdNotMeetCondition;

		public GuideCondition(Octets oc)
		{
			conditionIntArg = oc.pop_int();
			conditionStrArg = oc.pop_string();
			nextIdNotMeetCondition = oc.pop_int();
		}
	}
}
