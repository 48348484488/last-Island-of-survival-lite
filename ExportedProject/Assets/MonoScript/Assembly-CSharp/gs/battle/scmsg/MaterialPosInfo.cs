using Share;

namespace gs.battle.scmsg
{
	public class MaterialPosInfo : Marshal
	{
		public Vec3 pos = new Vec3();

		public Vec3 orientation = new Vec3();

		public int typeId;

		public int id;

		public Octets marshal(Octets oc)
		{
			oc.push(pos);
			oc.push(orientation);
			oc.push(typeId);
			oc.push(id);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(pos);
			oc.pop(orientation);
			typeId = oc.pop_int();
			id = oc.pop_int();
			return oc;
		}
	}
}
