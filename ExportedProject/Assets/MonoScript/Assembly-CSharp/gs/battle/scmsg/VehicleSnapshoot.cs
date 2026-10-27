using Share;

namespace gs.battle.scmsg
{
	public class VehicleSnapshoot : Marshal
	{
		public int id;

		public int type;

		public Vec3 pos = new Vec3();

		public Vec3 orientation = new Vec3();

		public Octets marshal(Octets oc)
		{
			oc.push(id);
			oc.push(type);
			oc.push(pos);
			oc.push(orientation);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			id = oc.pop_int();
			type = oc.pop_int();
			oc.pop(pos);
			oc.pop(orientation);
			return oc;
		}
	}
}
