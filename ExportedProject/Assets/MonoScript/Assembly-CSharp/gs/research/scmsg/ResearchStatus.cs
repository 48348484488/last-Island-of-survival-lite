using Share;

namespace gs.research.scmsg
{
	public class ResearchStatus : Marshal
	{
		public const int FREE = 1;

		public const int RESEARCH_START = 2;

		public const int RESEARCH_FAILED = 3;

		public const int IS_SUS_NOT_GET = 4;

		public int researchStatus;

		public Octets marshal(Octets oc)
		{
			oc.push(researchStatus);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			researchStatus = oc.pop_int();
			return oc;
		}
	}
}
