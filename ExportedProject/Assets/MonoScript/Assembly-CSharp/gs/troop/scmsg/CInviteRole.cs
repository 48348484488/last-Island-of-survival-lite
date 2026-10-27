using Net;
using Share;

namespace gs.troop.scmsg
{
	public class CInviteRole : Message
	{
		public delegate void Handler(CInviteRole msg);

		public const int TYPE = 15731645;

		public static Handler handler;

		public long otherId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731645;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherId = oc.pop_long();
			return oc;
		}
	}
}
