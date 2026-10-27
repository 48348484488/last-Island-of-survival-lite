using Share;

namespace gs.battle.scmsg
{
	public class BuildPartInfo : Marshal
	{
		public long roleId;

		public int typeId;

		public long instanceId;

		public int hp;

		public Vec3 pos = new Vec3();

		public float eulerY;

		public int status;

		public string name = string.Empty;

		public long toolBoxId;

		public Octets extraInfo = new Octets();

		public Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(typeId);
			oc.push(instanceId);
			oc.push(hp);
			oc.push(pos);
			oc.push(eulerY);
			oc.push(status);
			oc.push(name);
			oc.push(toolBoxId);
			oc.push(extraInfo);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			typeId = oc.pop_int();
			instanceId = oc.pop_long();
			hp = oc.pop_int();
			oc.pop(pos);
			eulerY = oc.pop_float();
			status = oc.pop_int();
			name = oc.pop_string();
			toolBoxId = oc.pop_long();
			extraInfo = oc.pop_octets();
			return oc;
		}
	}
}
