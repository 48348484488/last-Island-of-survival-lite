using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SSyncCarWheelRotation : Message
	{
		public delegate void Handler(SSyncCarWheelRotation msg);

		public const int TYPE = 11537411;

		public static Handler handler;

		public int vehicleId;

		public float frontYRatation;

		public float tailYRatation;

		public float rpm;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537411;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(vehicleId);
			oc.push(frontYRatation);
			oc.push(tailYRatation);
			oc.push(rpm);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			vehicleId = oc.pop_int();
			frontYRatation = oc.pop_float();
			tailYRatation = oc.pop_float();
			rpm = oc.pop_float();
			return oc;
		}
	}
}
