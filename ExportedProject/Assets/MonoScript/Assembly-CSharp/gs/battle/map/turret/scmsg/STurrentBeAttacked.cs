using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class STurrentBeAttacked : Message
	{
		public delegate void Handler(STurrentBeAttacked msg);

		public const int TYPE = 22023120;

		public static Handler handler;

		public long turretId;

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
			return 22023120;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			oc.push(roleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			roleId = oc.pop_long();
			return oc;
		}
	}
}
