using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class CRejectApply : Message
	{
		public delegate void Handler(CRejectApply msg);

		public const int TYPE = 32508868;

		public static Handler handler;

		public long rejectRoleId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 32508868;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(rejectRoleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			rejectRoleId = oc.pop_long();
			return oc;
		}
	}
}
