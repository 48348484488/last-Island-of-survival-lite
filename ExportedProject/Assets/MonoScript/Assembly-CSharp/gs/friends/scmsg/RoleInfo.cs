using Share;
using gs.role.scmsg;

namespace gs.friends.scmsg
{
	public class RoleInfo : Marshal
	{
		public BasicRoleInfo info = new BasicRoleInfo();

		public byte onlineStatus;

		public string signature = string.Empty;

		public Octets marshal(Octets oc)
		{
			oc.push(info);
			oc.push(onlineStatus);
			oc.push(signature);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(info);
			onlineStatus = oc.pop_byte();
			signature = oc.pop_string();
			return oc;
		}
	}
}
