using Share;

namespace gs.friends.scmsg
{
	public class CareRoleInfo : Marshal
	{
		public RoleInfo roleInfo = new RoleInfo();

		public int careTime;

		public string rename = string.Empty;

		public int intimacyValue;

		public int intimacyLevel;

		public Octets marshal(Octets oc)
		{
			oc.push(roleInfo);
			oc.push(careTime);
			oc.push(rename);
			oc.push(intimacyValue);
			oc.push(intimacyLevel);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(roleInfo);
			careTime = oc.pop_int();
			rename = oc.pop_string();
			intimacyValue = oc.pop_int();
			intimacyLevel = oc.pop_int();
			return oc;
		}
	}
}
