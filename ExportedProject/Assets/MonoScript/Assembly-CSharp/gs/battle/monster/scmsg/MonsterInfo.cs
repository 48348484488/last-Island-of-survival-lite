using System.Collections.Generic;
using Share;
using gs.battle.scmsg;

namespace gs.battle.monster.scmsg
{
	public class MonsterInfo : Marshal
	{
		public int id;

		public long instanceId;

		public bool hasOwner;

		public Vec3 pos = new Vec3();

		public Vec3 birthpos = new Vec3();

		public ShortVec3 orientation = new ShortVec3();

		public ShortVec3 velocity = new ShortVec3();

		public int hp;

		public Dictionary<byte, int> animatiorInfo = new Dictionary<byte, int>();

		public Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(instanceId);
			oc.push(hasOwner);
			oc.push(pos);
			oc.push(birthpos);
			oc.push(orientation);
			oc.push(velocity);
			oc.push(hp);
			oc.push(animatiorInfo.Count);
			foreach (KeyValuePair<byte, int> item in animatiorInfo)
			{
				oc.push(item.Key);
				oc.push(item.Value);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			instanceId = oc.pop_long();
			hasOwner = oc.pop_bool();
			oc.pop(pos);
			oc.pop(birthpos);
			oc.pop(orientation);
			oc.pop(velocity);
			hp = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				animatiorInfo.Add(oc.pop_byte(), oc.pop_int());
			}
			return oc;
		}
	}
}
