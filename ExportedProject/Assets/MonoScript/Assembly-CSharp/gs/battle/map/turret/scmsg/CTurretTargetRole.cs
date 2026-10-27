using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class CTurretTargetRole : Message
	{
		public delegate void Handler(CTurretTargetRole msg);

		public const int TYPE = 22023107;

		public static Handler handler;

		public long turretId;

		public long targetId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 22023107;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			oc.push(targetId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			targetId = oc.pop_long();
			return oc;
		}
	}
}
