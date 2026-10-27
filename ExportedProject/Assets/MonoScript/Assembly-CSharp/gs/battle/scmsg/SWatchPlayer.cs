using Net;
using Share;

namespace gs.battle.scmsg
{
	public class SWatchPlayer : Message
	{
		public delegate void Handler(SWatchPlayer msg);

		public const int TYPE = 11537436;

		public static Handler handler;

		public long roleId;

		public int hp;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537436;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(roleId);
			oc.push(hp);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			roleId = oc.pop_long();
			hp = oc.pop_int();
			return oc;
		}
	}
}
