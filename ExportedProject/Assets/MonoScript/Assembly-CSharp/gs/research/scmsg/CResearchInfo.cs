using Net;
using Share;

namespace gs.research.scmsg
{
	public class CResearchInfo : Message
	{
		public delegate void Handler(CResearchInfo msg);

		public const int TYPE = 18877368;

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
			return 18877368;
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
