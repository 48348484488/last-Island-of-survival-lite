using Share;

namespace gs.battle.scmsg
{
	public class GameObjectWithLightmap : Marshal
	{
		public Vec3 pos = new Vec3();

		public Vec3 scale = new Vec3();

		public Vec3 angles = new Vec3();

		public short prefabIndex;

		public byte lightmapIndex;

		public Vec4 lightmapScaleOffset = new Vec4();

		public Octets marshal(Octets oc)
		{
			oc.push(pos);
			oc.push(scale);
			oc.push(angles);
			oc.push(prefabIndex);
			oc.push(lightmapIndex);
			oc.push(lightmapScaleOffset);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(pos);
			oc.pop(scale);
			oc.pop(angles);
			prefabIndex = oc.pop_short();
			lightmapIndex = oc.pop_byte();
			oc.pop(lightmapScaleOffset);
			return oc;
		}
	}
}
