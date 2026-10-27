using Share;

namespace gs.role.scmsg
{
	public class RoleVersion : Marshal
	{
		public long roleId;

		public string name = string.Empty;

		public int version;

		public Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(name);
			oc.push(version);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			name = oc.pop_string();
			version = oc.pop_int();
			return oc;
		}
	}
}
