using System.Collections.Generic;
using Share;

namespace gs.battle.scmsg
{
	public class AllMosterList : Marshal
	{
		public Dictionary<int, MonsterPosList> allMosters = new Dictionary<int, MonsterPosList>();

		public Octets marshal(Octets oc)
		{
			oc.push(allMosters.Count);
			foreach (KeyValuePair<int, MonsterPosList> allMoster in allMosters)
			{
				oc.push(allMoster.Key);
				oc.push(allMoster.Value);
			}
			return oc;
		}

		public Octets unmarshal(Octets oc)
		{
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				MonsterPosList monsterPosList = new MonsterPosList();
				oc.pop(monsterPosList);
				allMosters.Add(key, monsterPosList);
			}
			return oc;
		}
	}
}
