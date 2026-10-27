using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class StaticGroup : Marshal
	{
		public Vec3 pos = new Vec3();

		public Vec3 scale = new Vec3();

		public Vec3 angles = new Vec3();

		public List<GameObjectWithLightmap> lod0things = new List<GameObjectWithLightmap>();

		public List<GameObjectWithLightmap> lod1things = new List<GameObjectWithLightmap>();

		public Octets marshal(Octets oc)
		{
			oc.push(pos);
			oc.push(scale);
			oc.push(angles);
			oc.push(lod0things.Count);
			foreach (GameObjectWithLightmap lod0thing in lod0things)
			{
				oc.push(lod0thing);
			}
			oc.push(lod1things.Count);
			foreach (GameObjectWithLightmap lod1thing in lod1things)
			{
				oc.push(lod1thing);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(pos);
			oc.pop(scale);
			oc.pop(angles);
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				GameObjectWithLightmap gameObjectWithLightmap = new GameObjectWithLightmap();
				oc.pop(gameObjectWithLightmap);
				lod0things.Add(gameObjectWithLightmap);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				GameObjectWithLightmap gameObjectWithLightmap2 = new GameObjectWithLightmap();
				oc.pop(gameObjectWithLightmap2);
				lod1things.Add(gameObjectWithLightmap2);
			}
			return oc;
		}
	}
}
