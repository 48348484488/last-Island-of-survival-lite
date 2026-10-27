using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class TeamateInfo : Marshal
	{
		public long roleId;

		public int status;

		public string name = string.Empty;

		public bool isDead;

		public List<Vec3> markList = new List<Vec3>();

		public long vehicleId;

		public int quickMessageId;

		public Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(status);
			oc.push(name);
			oc.push(isDead);
			oc.push(markList.Count);
			foreach (Vec3 mark in markList)
			{
				oc.push(mark);
			}
			oc.push(vehicleId);
			oc.push(quickMessageId);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			status = oc.pop_int();
			name = oc.pop_string();
			isDead = oc.pop_bool();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				Vec3 vec = new Vec3();
				oc.pop(vec);
				markList.Add(vec);
			}
			vehicleId = oc.pop_long();
			quickMessageId = oc.pop_int();
			return oc;
		}
	}
}
