using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SBloodAirportScore : Message
	{
		public delegate void Handler(SBloodAirportScore msg);

		public const int TYPE = 11537462;

		public static Handler handler;

		public int redScore;

		public int blueScore;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537462;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(redScore);
			oc.push(blueScore);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			redScore = oc.pop_int();
			blueScore = oc.pop_int();
			return oc;
		}
	}
}
