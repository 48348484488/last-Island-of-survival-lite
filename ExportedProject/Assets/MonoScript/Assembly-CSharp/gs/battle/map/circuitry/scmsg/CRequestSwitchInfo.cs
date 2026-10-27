using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class CRequestSwitchInfo : Message
	{
		public delegate void Handler(CRequestSwitchInfo msg);

		public const int TYPE = 27265992;

		public static Handler handler;

		public long switchInsId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 27265992;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(switchInsId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			switchInsId = oc.pop_long();
			return oc;
		}
	}
}
