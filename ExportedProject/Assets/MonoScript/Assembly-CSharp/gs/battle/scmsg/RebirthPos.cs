using Share;

namespace gs.battle.scmsg
{
	public class RebirthPos : Marshal
	{
		public int typeId;

		public Vec3 pos = new Vec3();

		public int cdTime;

		public string name = string.Empty;

		public Octets marshal(Octets oc)
		{
			oc.push(typeId);
			oc.push(pos);
			oc.push(cdTime);
			oc.push(name);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			typeId = oc.pop_int();
			oc.pop(pos);
			cdTime = oc.pop_int();
			name = oc.pop_string();
			return oc;
		}
	}
}
