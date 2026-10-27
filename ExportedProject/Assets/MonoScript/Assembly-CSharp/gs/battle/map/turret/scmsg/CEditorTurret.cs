using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class CEditorTurret : Message
	{
		public delegate void Handler(CEditorTurret msg);

		public const int TYPE = 22023111;

		public static Handler handler;

		public long turretId;

		public bool isAttackCompanions;

		public bool isAttackAllies;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 22023111;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			oc.push(isAttackCompanions);
			oc.push(isAttackAllies);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			isAttackCompanions = oc.pop_bool();
			isAttackAllies = oc.pop_bool();
			return oc;
		}
	}
}
