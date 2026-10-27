using Net;
using Share;

namespace gs.battle.scmsg
{
	public class STeamDie : Message
	{
		public delegate void Handler(STeamDie msg);

		public const int TYPE = 11537342;

		public static Handler handler;

		public int groupId;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537342;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(groupId);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			groupId = oc.pop_int();
			return oc;
		}
	}
}
