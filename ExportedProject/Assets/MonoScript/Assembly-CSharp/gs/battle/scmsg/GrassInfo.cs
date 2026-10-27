using Share;

namespace gs.battle.scmsg
{
	public class GrassInfo : Marshal
	{
		public Vec3 pos = new Vec3();

		public Vec3 scale = new Vec3();

		public Vec3 angles = new Vec3();

		public short prefabIndex;

		public short brightness;

		public Octets marshal(Octets oc)
		{
			oc.push(pos);
			oc.push(scale);
			oc.push(angles);
			oc.push(prefabIndex);
			oc.push(brightness);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(pos);
			oc.pop(scale);
			oc.pop(angles);
			prefabIndex = oc.pop_short();
			brightness = oc.pop_short();
			return oc;
		}
	}
}
