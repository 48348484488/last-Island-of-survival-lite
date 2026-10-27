using System.Collections.Generic;
using Net;
using Share;
using gs.smelter.scmsg;

namespace gs.battle.map.turret.scmsg
{
	public class SManagementTurret : Message
	{
		public delegate void Handler(SManagementTurret msg);

		public const int TYPE = 22023100;

		public static Handler handler;

		public long turretId;

		public bool isAttackCompanions;

		public bool isAttackAllies;

		public List<long> permissionsList = new List<long>();

		public List<long> allRoleIdsInTroop = new List<long>();

		public Dictionary<int, UseItem> bullets = new Dictionary<int, UseItem>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 22023100;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(turretId);
			oc.push(isAttackCompanions);
			oc.push(isAttackAllies);
			oc.push(permissionsList.Count);
			foreach (long permissions in permissionsList)
			{
				oc.push(permissions);
			}
			oc.push(allRoleIdsInTroop.Count);
			foreach (long item in allRoleIdsInTroop)
			{
				oc.push(item);
			}
			oc.push(bullets.Count);
			foreach (KeyValuePair<int, UseItem> bullet in bullets)
			{
				oc.push(bullet.Key);
				oc.push(bullet.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			turretId = oc.pop_long();
			isAttackCompanions = oc.pop_bool();
			isAttackAllies = oc.pop_bool();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				permissionsList.Add(oc.pop_long());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				allRoleIdsInTroop.Add(oc.pop_long());
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				int key = oc.pop_int();
				UseItem useItem = new UseItem();
				oc.pop(useItem);
				bullets.Add(key, useItem);
			}
			return oc;
		}
	}
}
