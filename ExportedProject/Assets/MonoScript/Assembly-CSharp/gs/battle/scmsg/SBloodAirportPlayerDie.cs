using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SBloodAirportPlayerDie : Message
	{
		public delegate void Handler(SBloodAirportPlayerDie msg);

		public const int TYPE = 11537456;

		public static Handler handler;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537456;
		}

		public override Octets marshal(Octets oc)
		{
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			return oc;
		}
	}
}
