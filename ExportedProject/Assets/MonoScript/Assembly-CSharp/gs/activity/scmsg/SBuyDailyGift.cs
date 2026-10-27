using Net;
using Share;
using gs.drop.scmsg;

namespace gs.activity.scmsg
{
	public class SBuyDailyGift : Message
	{
		public delegate void Handler(SBuyDailyGift msg);

		public const int TYPE = 29363136;

		public static Handler handler;

		public int index;

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
			return 29363136;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(index);
			oc.push(dropDetail);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			index = oc.pop_int();
			oc.pop(dropDetail);
			return oc;
		}
	}
}
