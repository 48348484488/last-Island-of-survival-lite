using Net;
using Share;

namespace gs.activity.scmsg
{
	public class CGetWeekSignReward : Message
	{
		public delegate void Handler(CGetWeekSignReward msg);

		public const int TYPE = 29363129;

		public static Handler handler;

		public int day;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 29363129;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(day);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			day = oc.pop_int();
			return oc;
		}
	}
}
