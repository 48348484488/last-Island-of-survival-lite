using Net;
using Share;

namespace gs.friends.scmsg
{
	public class CBlack : Message
	{
		public delegate void Handler(CBlack msg);

		public const int TYPE = 13634497;

		public static Handler handler;

		public long roleId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634497;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			return oc;
		}
	}
}
