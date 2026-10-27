using Net;
using Share;

namespace gs.friends.scmsg
{
	public class SFansAdd : Message
	{
		public delegate void Handler(SFansAdd msg);

		public const int TYPE = 13634489;

		public static Handler handler;

		public FansRoleInfo info = new FansRoleInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634489;
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
