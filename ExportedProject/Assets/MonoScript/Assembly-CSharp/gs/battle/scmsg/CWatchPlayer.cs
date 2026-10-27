using Net;
using Share;

namespace gs.battle.scmsg
{
	public class CWatchPlayer : Message
	{
		public delegate void Handler(CWatchPlayer msg);

		public const int TYPE = 11537435;

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
			return 11537435;
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
