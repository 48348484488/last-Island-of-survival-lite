using Share;

namespace gs.battle.scmsg
{
	public class TreeObjectInfo : Marshal
	{
		public Vec3 pos = new Vec3();

		public Vec3 scale = new Vec3();

		public Vec3 angles = new Vec3();

		public short prefabIndex;

		public byte treeType;

		public int id;

		public Octets marshal(Octets oc)
		{
			oc.push(pos);
			oc.push(scale);
			oc.push(angles);
			oc.push(prefabIndex);
			oc.push(treeType);
			oc.push(id);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(pos);
			oc.pop(scale);
			oc.pop(angles);
			prefabIndex = oc.pop_short();
			treeType = oc.pop_byte();
			id = oc.pop_int();
			return oc;
		}
	}
}
