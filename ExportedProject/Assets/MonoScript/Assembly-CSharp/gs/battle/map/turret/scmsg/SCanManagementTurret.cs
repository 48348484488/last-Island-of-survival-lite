using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class SCanManagementTurret : Message
	{
		public delegate void Handler(SCanManagementTurret msg);

		public const int TYPE = 22023098;

		public static Handler handler;

		public long turretId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 22023098;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			return oc;
		}
	}
}
