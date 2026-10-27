using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SVehicleSkinChange : Message
	{
		public delegate void Handler(SVehicleSkinChange msg);

		public const int TYPE = 11537469;

		public static Handler handler;

		public int newVehicleTypeId;

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
			return 11537469;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(newVehicleTypeId);
			oc.push(vehicleInfo);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			newVehicleTypeId = oc.pop_int();
			oc.pop(vehicleInfo);
			return oc;
		}
	}
}
