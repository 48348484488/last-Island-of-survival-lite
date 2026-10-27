using Net;
using Share;

namespace gs.armygroup.scmsg
{
	public class CPassApply : Message
	{
		public delegate void Handler(CPassApply msg);

		public const int TYPE = 32508867;

		public static Handler handler;

		public long applyRoleId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 32508867;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(applyRoleId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			applyRoleId = oc.pop_long();
			return oc;
		}
	}
}
