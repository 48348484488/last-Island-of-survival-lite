using Net;
using Share;

namespace gs.friends.scmsg
{
	public class SSearchByRoleId : Message
	{
		public delegate void Handler(SSearchByRoleId msg);

		public const int TYPE = 13634505;

		public static Handler handler;

		public RoleInfo roleInfo = new RoleInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634505;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleInfo);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(roleInfo);
			return oc;
		}
	}
}
