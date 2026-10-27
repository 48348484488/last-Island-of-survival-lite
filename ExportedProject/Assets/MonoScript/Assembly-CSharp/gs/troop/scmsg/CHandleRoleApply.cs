using Net;
using Share;

namespace gs.troop.scmsg
{
	public class CHandleRoleApply : Message
	{
		public delegate void Handler(CHandleRoleApply msg);

		public const int TYPE = 15731654;

		public static Handler handler;

		public long otherId;

		public bool agree;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731654;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(otherId);
			oc.push(agree);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			otherId = oc.pop_long();
			agree = oc.pop_bool();
			return oc;
		}
	}
}
