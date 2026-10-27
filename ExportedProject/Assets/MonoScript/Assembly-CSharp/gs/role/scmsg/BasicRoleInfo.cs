using Share;

namespace gs.role.scmsg
{
	public class BasicRoleInfo : Marshal
	{
		public string userId = string.Empty;

		public long roleId;

		public int version;

		public int frameId;

		public short headId;

		public string name = string.Empty;

		public bool sex;

		public short level;

		public Octets marshal(Octets oc)
		{
			oc.push(userId);
			oc.push(roleId);
			oc.push(version);
			oc.push(frameId);
			oc.push(headId);
			oc.push(name);
			oc.push(sex);
			oc.push(level);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			userId = oc.pop_string();
			roleId = oc.pop_long();
			version = oc.pop_int();
			frameId = oc.pop_int();
			headId = oc.pop_short();
			name = oc.pop_string();
			sex = oc.pop_bool();
			level = oc.pop_short();
			return oc;
		}
	}
}
