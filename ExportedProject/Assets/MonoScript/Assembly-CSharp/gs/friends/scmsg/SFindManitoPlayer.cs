using System.Collections.Generic;
using Net;
using Share;

namespace gs.friends.scmsg
{
	public class SFindManitoPlayer : Message
	{
		public delegate void Handler(SFindManitoPlayer msg);

		public const int TYPE = 13634507;

		public static Handler handler;

		public List<RoleInfo> players = new List<RoleInfo>();

		public int lostCdTime;

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 13634507;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(players.Count);
			foreach (RoleInfo player in players)
			{
				oc.push(player);
			}
			oc.push(lostCdTime);
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				RoleInfo roleInfo = new RoleInfo();
				oc.pop(roleInfo);
				players.Add(roleInfo);
			}
			lostCdTime = oc.pop_int();
			return oc;
		}
	}
}
