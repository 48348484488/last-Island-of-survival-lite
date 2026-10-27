using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.scmsg
{
	public class STeamateInfos : Message
	{
		public delegate void Handler(STeamateInfos msg);

		public const int TYPE = 11537405;

		public static Handler handler;

		public HashSet<TeamateInfo> teamateInfos = new HashSet<TeamateInfo>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 11537405;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(teamateInfos.Count);
			foreach (TeamateInfo teamateInfo in teamateInfos)
			{
				oc.push(teamateInfo);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				TeamateInfo teamateInfo = new TeamateInfo();
				oc.pop(teamateInfo);
				teamateInfos.Add(teamateInfo);
			}
			return oc;
		}
	}
}
