using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SVehicleHitPlayer : Message
	{
		public delegate void Handler(SVehicleHitPlayer msg);

		public const int TYPE = 23071691;

		public static Handler handler;

		public int vehicleId;

		public Vec3 forward = new Vec3();

		public float speed;

		public long roleId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071691;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(vehicleId);
			oc.push(forward);
			oc.push(speed);
			oc.push(roleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			vehicleId = oc.pop_int();
			oc.pop(forward);
			speed = oc.pop_float();
			roleId = oc.pop_long();
			return oc;
		}
	}
}
