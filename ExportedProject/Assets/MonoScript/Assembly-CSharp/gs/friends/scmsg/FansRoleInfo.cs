using Share;

namespace gs.friends.scmsg
{
	public class FansRoleInfo : Marshal
	{
		public RoleInfo roleInfo = new RoleInfo();

		public int fansTime;

		public Octets marshal(Octets oc)
		{
			oc.push(roleInfo);
			oc.push(fansTime);
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			oc.pop(roleInfo);
			fansTime = oc.pop_int();
			return oc;
		}
	}
}
