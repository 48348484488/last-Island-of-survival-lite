using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SGenSafeZone : Message
	{
		public delegate void Handler(SGenSafeZone msg);

		public const int TYPE = 11537433;

		public static Handler handler;

		public int round;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537433;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(round);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			round = oc.pop_int();
			return oc;
		}
	}
}
