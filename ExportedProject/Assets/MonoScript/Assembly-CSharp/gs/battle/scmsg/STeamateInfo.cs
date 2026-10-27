using Net;
using Share;

namespace gs.battle.scmsg
{
	public class STeamateInfo : Message
	{
		public delegate void Handler(STeamateInfo msg);

		public const int TYPE = 11537406;

		public static Handler handler;

		public TeamateInfo teamateInfo = new TeamateInfo();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537406;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(teamateInfo);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			oc.pop(teamateInfo);
			return oc;
		}
	}
}
