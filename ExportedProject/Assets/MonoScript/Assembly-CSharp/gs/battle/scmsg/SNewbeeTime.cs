using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SNewbeeTime : Message
	{
		public delegate void Handler(SNewbeeTime msg);

		public const int TYPE = 11537552;

		public static Handler handler;

		public int timeLeft;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537552;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(timeLeft);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			timeLeft = oc.pop_int();
			return oc;
		}
	}
}
