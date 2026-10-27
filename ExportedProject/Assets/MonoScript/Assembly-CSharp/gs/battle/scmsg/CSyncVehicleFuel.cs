using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CSyncVehicleFuel : Message
	{
		public delegate void Handler(CSyncVehicleFuel msg);

		public const int TYPE = 11537384;

		public static Handler handler;

		public int id;

		public float fuel;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537384;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(fuel);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			fuel = oc.pop_float();
			return oc;
		}
	}
}
