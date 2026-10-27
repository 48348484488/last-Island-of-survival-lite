using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class CReportManagementTurret : Message
	{
		public delegate void Handler(CReportManagementTurret msg);

		public const int TYPE = 22023112;

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
			return 22023112;
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
