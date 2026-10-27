using Net;
using Share;

namespace gs.battle.map.circuitry.scmsg
{
	public class SOnOrOffElementSwitch : Message
	{
		public delegate void Handler(SOnOrOffElementSwitch msg);

		public const int TYPE = 27265989;

		public static Handler handler;

		public long instanceId;

		public bool isOn;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 27265989;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(instanceId);
			oc.push(isOn);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			instanceId = oc.pop_long();
			isOn = oc.pop_bool();
			return oc;
		}
	}
}
