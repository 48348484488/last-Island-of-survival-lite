using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class VehicleInfo : Marshal
	{
		public const int CAR = 0;

		public const int SHIP = 1;

		public const int HELICOPTER = 2;

		public int id;

		public int type;

		public Vec3 pos = new Vec3();

		public Vec3 orientation = new Vec3();

		public Vec3 velocity = new Vec3();

		public int hp;

		public float fuel;

		public BaseInput input = new BaseInput();

		public long driverRoleId;

		public Dictionary<int, int> shieldInfo = new Dictionary<int, int>();

		public Dictionary<int, long> seatInfo = new Dictionary<int, long>();

		public long ownerId;

		public Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(type);
			oc.push(pos);
			oc.push(orientation);
			oc.push(velocity);
			oc.push(hp);
			oc.push(fuel);
			oc.push(input);
			oc.push(driverRoleId);
			oc.push(shieldInfo.Count);
			foreach (KeyValuePair<int, int> item in shieldInfo)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			oc.push(seatInfo.Count);
			foreach (KeyValuePair<int, long> item2 in seatInfo)
			{
				oc.push(item2.Key);
				oc.push(item2.Value);
			}
			oc.push(ownerId);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			type = oc.pop_int();
			oc.pop(pos);
			oc.pop(orientation);
			oc.pop(velocity);
			hp = oc.pop_int();
			fuel = oc.pop_float();
			oc.pop(input);
			driverRoleId = oc.pop_long();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				shieldInfo.Add(oc.pop_int(), oc.pop_int());
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				seatInfo.Add(oc.pop_int(), oc.pop_long());
			}
			ownerId = oc.pop_long();
			return oc;
		}
	}
}
