using Net;
using Share;

namespace gs.role.scmsg
{
	public class SBasicInfo : Message
	{
		public delegate void Handler(SBasicInfo msg);

		public const int TYPE = 4197307;

		public static Handler handler;

		public BasicRoleInfo info = new BasicRoleInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 4197307;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(info);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(info);
			return oc;
		}
	}
}
