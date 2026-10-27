using Net;
using Share;

namespace gs.research.scmsg
{
	public class CGetResearchItem : Message
	{
		public delegate void Handler(CGetResearchItem msg);

		public const int TYPE = 18877372;

		public static Handler handler;

		public long researchId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 18877372;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(researchId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			researchId = oc.pop_long();
			return oc;
		}
	}
}
