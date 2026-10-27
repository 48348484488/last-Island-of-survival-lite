using System.Collections.Generic;
using Net;
using Share;

namespace gs.battle.monster.scmsg
{
	public class SControlledMonsters : Message
	{
		public delegate void Handler(SControlledMonsters msg);

		public const int TYPE = 28314552;

		public static Handler handler;

		public HashSet<long> monsterIds = new HashSet<long>();

		public override void handle()
		{
			if (handler != null)
			{
				handler(this);
			}
		}

		public override int getType()
		{
			return 28314552;
		}

		public override Octets marshal(Octets oc)
		{
			oc.push(monsterIds.Count);
			foreach (long monsterId in monsterIds)
			{
				oc.push(monsterId);
			}
			return oc;
		}

		public override Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				monsterIds.Add(oc.pop_long());
			}
			return oc;
		}
	}
}
