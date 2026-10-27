using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class SSynchronizedTurretPatrolAngle : Message
	{
		public delegate void Handler(SSynchronizedTurretPatrolAngle msg);

		public const int TYPE = 22023117;

		public static Handler handler;

		public long turretId;

		public float angle;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 22023117;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			oc.push(angle);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			angle = oc.pop_float();
			return oc;
		}
	}
}
