using Net;
using Share;

namespace gs.friends.scmsg
{
	public class SCare : Message
	{
		public delegate void Handler(SCare msg);

		public const int TYPE = 13634494;

		public static Handler handler;

		public CareRoleInfo info = new CareRoleInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634494;
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
