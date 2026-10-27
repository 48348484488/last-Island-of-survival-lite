using Net;
using Share;

namespace gs.friends.scmsg
{
	public class SBlack : Message
	{
		public delegate void Handler(SBlack msg);

		public const int TYPE = 13634498;

		public static Handler handler;

		public BlackRoleInfo info = new BlackRoleInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634498;
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
