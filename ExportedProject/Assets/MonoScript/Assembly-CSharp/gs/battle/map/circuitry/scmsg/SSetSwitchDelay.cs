using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class SSetSwitchDelay : Message
	{
		public delegate void Handler(SSetSwitchDelay msg);

		public const int TYPE = 27265991;

		public static Handler handler;

		public long instanceId;

		public byte delay;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 27265991;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(delay);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			delay = oc.pop_byte();
			return oc;
		}
	}
}
