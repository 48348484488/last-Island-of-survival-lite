using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CRemoveVehiclesFromCanSee : Message
	{
		public delegate void Handler(CRemoveVehiclesFromCanSee msg);

		public const int TYPE = 11537447;

		public static Handler handler;

		public HashSet<int> vehicleIds = new HashSet<int>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537447;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(vehicleIds.Count);
			foreach (int vehicleId in vehicleIds)
			{
				oc.push(vehicleId);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				vehicleIds.Add(oc.pop_int());
			}
			return oc;
		}
	}
}
