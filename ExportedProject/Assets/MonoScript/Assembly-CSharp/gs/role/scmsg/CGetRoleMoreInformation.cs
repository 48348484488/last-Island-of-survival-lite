using Net;
using Share;

namespace gs.role.scmsg
{
	public class CGetRoleMoreInformation : Message
	{
		public delegate void Handler(CGetRoleMoreInformation msg);

		public const int TYPE = 4197324;

		public static Handler handler;

		public long targetRoleId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197324;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(targetRoleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			targetRoleId = oc.pop_long();
			return oc;
		}
	}
}
