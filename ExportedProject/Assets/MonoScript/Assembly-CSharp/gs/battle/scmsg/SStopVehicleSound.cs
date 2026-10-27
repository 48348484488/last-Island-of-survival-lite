using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SStopVehicleSound : Message
	{
		public delegate void Handler(SStopVehicleSound msg);

		public const int TYPE = 11537409;

		public static Handler handler;

		public int vehicleId;

		public int soundId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537409;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(vehicleId);
			oc.push(soundId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			vehicleId = oc.pop_int();
			soundId = oc.pop_int();
			return oc;
		}
	}
}
