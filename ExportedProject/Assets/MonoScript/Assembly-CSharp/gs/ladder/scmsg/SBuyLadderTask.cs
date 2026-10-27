using Net;
using Share;
using gs.drop.scmsg;

namespace gs.ladder.scmsg
{
	public class SBuyLadderTask : Message
	{
		public delegate void Handler(SBuyLadderTask msg);

		public const int TYPE = 19925946;

		public static Handler handler;

		public int type;

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
			return 19925946;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(type);
			oc.push(dropDetail);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			type = oc.pop_int();
			oc.pop(dropDetail);
			return oc;
		}
	}
}
