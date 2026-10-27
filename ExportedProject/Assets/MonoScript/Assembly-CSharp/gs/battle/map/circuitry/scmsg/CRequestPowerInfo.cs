using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class CRequestPowerInfo : Message
	{
		public delegate void Handler(CRequestPowerInfo msg);

		public const int TYPE = 27265978;

		public static Handler handler;

		public long powerId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 27265978;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(powerId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			powerId = oc.pop_long();
			return oc;
		}
	}
}
