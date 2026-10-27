using Net;
using Share;
using gs.drop.scmsg;

namespace gs.activity.scmsg
{
	public class SGetWeekSignReward : Message
	{
		public delegate void Handler(SGetWeekSignReward msg);

		public const int TYPE = 29363130;

		public static Handler handler;

		public int day;

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
			return 29363130;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(day);
			oc.push(dropDetail);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			day = oc.pop_int();
			oc.pop(dropDetail);
			return oc;
		}
	}
}
