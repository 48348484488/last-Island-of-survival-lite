using Share;

namespace gs.battle.scmsg
{
	public class TreePosInfo : Marshal
	{
		public Vec3 pos = new Vec3();

		public byte typeId;

		public int id;

		public Octets marshal(Octets oc)
		{
			oc.push(pos);
			oc.push(typeId);
			oc.push(id);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(pos);
			typeId = oc.pop_byte();
			id = oc.pop_int();
			return oc;
		}
	}
}
