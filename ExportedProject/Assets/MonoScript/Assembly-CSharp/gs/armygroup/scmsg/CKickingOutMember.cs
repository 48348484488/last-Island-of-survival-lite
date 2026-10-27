using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class CKickingOutMember : Message
	{
		public delegate void Handler(CKickingOutMember msg);

		public const int TYPE = 32508870;

		public static Handler handler;

		public long kickingRoleId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 32508870;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(kickingRoleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			kickingRoleId = oc.pop_long();
			return oc;
		}
	}
}
