using System.Collections.Generic;
using Net;
using Share;

namespace gs.troop.scmsg
{
	public class STroopInfo : Message
	{
		public delegate void Handler(STroopInfo msg);

		public const int TYPE = 15731644;

		public static Handler handler;

		public int troopId;

		public long leaderId;

		public bool allowAddStranger;

		public List<TroopPlayer> players = new List<TroopPlayer>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 15731644;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(troopId);
			oc.push(leaderId);
			oc.push(allowAddStranger);
			oc.push(players.Count);
			foreach (TroopPlayer player in players)
			{
				oc.push(player);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			troopId = oc.pop_int();
			leaderId = oc.pop_long();
			allowAddStranger = oc.pop_bool();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				TroopPlayer troopPlayer = new TroopPlayer();
				oc.pop(troopPlayer);
				players.Add(troopPlayer);
			}
			return oc;
		}
	}
}
