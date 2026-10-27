using Net;
using Share;

namespace gs.troop.scmsg
{
	public class CAutoRefuse : Message
	{
		public delegate void Handler(CAutoRefuse msg);

		public const int TYPE = 15731662;

		public static Handler handler;

		public long roleId;

		public short aletId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731662;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(aletId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			aletId = oc.pop_short();
			return oc;
		}
	}
}
