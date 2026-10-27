using Net;
using Share;
using gs.bag.scmsg;

namespace gs.research.scmsg
{
	public class SResearchInfo : Message
	{
		public delegate void Handler(SResearchInfo msg);

		public const int TYPE = 18877369;

		public static Handler handler;

		public long researchId;

		public int lastStartResearchId;

		public int finishTime;

		public int researchStatus;

		public bool isAddAssist;

		public BagItem bagItem = new BagItem();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 18877369;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(researchId);
			oc.push(lastStartResearchId);
			oc.push(finishTime);
			oc.push(researchStatus);
			oc.push(isAddAssist);
			oc.push(bagItem);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			researchId = oc.pop_long();
			lastStartResearchId = oc.pop_int();
			finishTime = oc.pop_int();
			researchStatus = oc.pop_int();
			isAddAssist = oc.pop_bool();
			oc.pop(bagItem);
			return oc;
		}
	}
}
