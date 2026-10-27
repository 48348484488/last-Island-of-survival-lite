using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CCauseDamgeByVehicle : Message
	{
		public delegate void Handler(CCauseDamgeByVehicle msg);

		public const int TYPE = 23071693;

		public static Handler handler;

		public int vehicleId;

		public int vehicleHp;

		public Dictionary<long, int> playerId2Damage = new Dictionary<long, int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071693;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(vehicleId);
			oc.push(vehicleHp);
			oc.push(playerId2Damage.Count);
			foreach (KeyValuePair<long, int> item in playerId2Damage)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			vehicleId = oc.pop_int();
			vehicleHp = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				playerId2Damage.Add(oc.pop_long(), oc.pop_int());
			}
			return oc;
		}
	}
}
