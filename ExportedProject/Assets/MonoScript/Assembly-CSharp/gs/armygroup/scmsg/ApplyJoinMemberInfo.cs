using Share;

namespace gs.armygroup.scmsg
{
	public class ApplyJoinMemberInfo : Marshal
	{
		public long roleId;

		public string roleName = string.Empty;

		public int roleLevel;

		public bool roleSex;

		public Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(roleName);
			oc.push(roleLevel);
			oc.push(roleSex);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			roleName = oc.pop_string();
			roleLevel = oc.pop_int();
			roleSex = oc.pop_bool();
			return oc;
		}
	}
}
