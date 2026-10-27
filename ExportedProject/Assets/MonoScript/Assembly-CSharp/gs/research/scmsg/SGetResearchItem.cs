using Net;
using Share;
using gs.drop.scmsg;

namespace gs.research.scmsg
{
	public class SGetResearchItem : Message
	{
		public delegate void Handler(SGetResearchItem msg);

		public const int TYPE = 18877373;

		public static Handler handler;

		public long researchId;

		public int drawingId;

		public DropDetail dropDetail = new DropDetail();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 18877373;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(researchId);
			oc.push(drawingId);
			oc.push(dropDetail);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			researchId = oc.pop_long();
			drawingId = oc.pop_int();
			oc.pop(dropDetail);
			return oc;
		}
	}
}
