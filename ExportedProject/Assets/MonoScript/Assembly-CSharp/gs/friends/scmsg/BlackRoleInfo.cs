using Share;

namespace gs.friends.scmsg
{
	public class BlackRoleInfo : Marshal
	{
		public RoleInfo roleInfo = new RoleInfo();

		public int blackTime;

		public Octets marshal(Octets oc)
		{
			oc.push(roleInfo);
			oc.push(blackTime);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(roleInfo);
			blackTime = oc.pop_int();
			return oc;
		}
	}
}
