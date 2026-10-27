using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SStartShrinkToxicGas : Message
	{
		public delegate void Handler(SStartShrinkToxicGas msg);

		public const int TYPE = 11537390;

		public static Handler handler;

		public int timeLeft;

		public int totalTime;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537390;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(timeLeft);
			oc.push(totalTime);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			timeLeft = oc.pop_int();
			totalTime = oc.pop_int();
			return oc;
		}
	}
}
