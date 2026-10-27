using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.map.turret.scmsg
{
	public class SCreateRoleTroopChange : Message
	{
		public delegate void Handler(SCreateRoleTroopChange msg);

		public const int TYPE = 22023114;

		public static Handler handler;

		public long turretId;

		public List<long> allRoleIdsInTroop = new List<long>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 22023114;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			oc.push(allRoleIdsInTroop.Count);
			foreach (long item in allRoleIdsInTroop)
			{
				oc.push(item);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				allRoleIdsInTroop.Add(oc.pop_long());
			}
			return oc;
		}
	}
}
