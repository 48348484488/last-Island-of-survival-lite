using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SVehicleInfo : Message
	{
		public delegate void Handler(SVehicleInfo msg);

		public const int TYPE = 11537366;

		public static Handler handler;

		public VehicleInfo vehicleInfo = new VehicleInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537366;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(vehicleInfo);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(vehicleInfo);
			return oc;
		}
	}
}
