using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class CChangeTurretCartridgeIndex : Message
	{
		public delegate void Handler(CChangeTurretCartridgeIndex msg);

		public const int TYPE = 22023113;

		public static Handler handler;

		public long turretId;

		public int changeIndex;

		public int targetIndex;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 22023113;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			oc.push(changeIndex);
			oc.push(targetIndex);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			changeIndex = oc.pop_int();
			targetIndex = oc.pop_int();
			return oc;
		}
	}
}
