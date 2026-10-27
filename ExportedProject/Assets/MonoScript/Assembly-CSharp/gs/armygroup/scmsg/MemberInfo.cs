using Share;

namespace gs.armygroup.scmsg
{
	public class MemberInfo : Marshal
	{
		public long roleId;

		public string roleName = string.Empty;

		public int roleLevel;

		public long prosperous;

		public bool roleSex;

		public int offlineTime;

		public int roleIcon;

		public Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(roleName);
			oc.push(roleLevel);
			oc.push(prosperous);
			oc.push(roleSex);
			oc.push(offlineTime);
			oc.push(roleIcon);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			roleName = oc.pop_string();
			roleLevel = oc.pop_int();
			prosperous = oc.pop_long();
			roleSex = oc.pop_bool();
			offlineTime = oc.pop_int();
			roleIcon = oc.pop_int();
			return oc;
		}
	}
}
