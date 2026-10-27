using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class CChangeTurretStatus : Message
	{
		public delegate void Handler(CChangeTurretStatus msg);

		public const int TYPE = 22023115;

		public static Handler handler;

		public long turretId;

		public byte change;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 22023115;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			oc.push(change);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			change = oc.pop_byte();
			return oc;
		}
	}
}
