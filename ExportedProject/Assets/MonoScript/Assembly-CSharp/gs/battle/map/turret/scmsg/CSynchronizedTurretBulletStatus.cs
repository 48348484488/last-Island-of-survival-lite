using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class CSynchronizedTurretBulletStatus : Message
	{
		public delegate void Handler(CSynchronizedTurretBulletStatus msg);

		public const int TYPE = 22023118;

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
			return 22023118;
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
