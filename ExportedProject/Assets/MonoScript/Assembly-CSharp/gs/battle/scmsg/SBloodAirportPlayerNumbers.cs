using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SBloodAirportPlayerNumbers : Message
	{
		public delegate void Handler(SBloodAirportPlayerNumbers msg);

		public const int TYPE = 11537457;

		public static Handler handler;

		public int redNumber;

		public int blueNumber;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537457;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(redNumber);
			oc.push(blueNumber);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			redNumber = oc.pop_int();
			blueNumber = oc.pop_int();
			return oc;
		}
	}
}
