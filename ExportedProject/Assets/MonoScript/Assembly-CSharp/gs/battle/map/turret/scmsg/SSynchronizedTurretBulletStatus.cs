using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class SSynchronizedTurretBulletStatus : Message
	{
		public delegate void Handler(SSynchronizedTurretBulletStatus msg);

		public const int TYPE = 22023119;

		public static Handler handler;

		public long turretId;

		public bool isHave;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 22023119;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			oc.push(isHave);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			isHave = oc.pop_bool();
			return oc;
		}
	}
}
