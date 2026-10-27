using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CVehicleExplosion : Message
	{
		public delegate void Handler(CVehicleExplosion msg);

		public const int TYPE = 23071692;

		public static Handler handler;

		public int vehicleId;

		public Dictionary<long, int> roleDamageInfo = new Dictionary<long, int>();

		public Dictionary<int, int> vehicleDamageInfo = new Dictionary<int, int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 23071692;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(vehicleId);
			oc.push(roleDamageInfo.Count);
			foreach (KeyValuePair<long, int> item in roleDamageInfo)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			oc.push(vehicleDamageInfo.Count);
			foreach (KeyValuePair<int, int> item2 in vehicleDamageInfo)
			{
				oc.push(item2.Key);
				oc.push(item2.Value);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			vehicleId = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				roleDamageInfo.Add(oc.pop_long(), oc.pop_int());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				vehicleDamageInfo.Add(oc.pop_int(), oc.pop_int());
			}
			return oc;
		}
	}
}
