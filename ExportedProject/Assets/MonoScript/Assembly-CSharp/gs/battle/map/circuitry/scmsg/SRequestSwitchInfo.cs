using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class SRequestSwitchInfo : Message
	{
		public delegate void Handler(SRequestSwitchInfo msg);

		public const int TYPE = 27265993;

		public static Handler handler;

		public long switchInsId;

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
			return 27265993;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(switchInsId);
			oc.push(delay);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			switchInsId = oc.pop_long();
			delay = oc.pop_byte();
			return oc;
		}
	}
}
