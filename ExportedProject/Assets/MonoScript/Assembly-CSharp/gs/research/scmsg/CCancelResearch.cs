using Net;
using Share;

namespace gs.research.scmsg
{
	public class CCancelResearch : Message
	{
		public delegate void Handler(CCancelResearch msg);

		public const int TYPE = 18877371;

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
			return 18877371;
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
