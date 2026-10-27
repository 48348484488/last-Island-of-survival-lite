using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CVehicleHitPlayer : Message
	{
		public delegate void Handler(CVehicleHitPlayer msg);

		public const int TYPE = 23071690;

		public static Handler handler;

		public long vehicleId;

		public ShortVec3 forward = new ShortVec3();

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
			return 23071690;
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
			vehicleId = oc.pop_long();
			oc.pop(forward);
			speed = oc.pop_float();
			roleId = oc.pop_long();
			return oc;
		}
	}
}
