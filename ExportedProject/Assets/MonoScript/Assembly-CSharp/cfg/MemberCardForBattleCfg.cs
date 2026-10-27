using System.Collections.Generic;
using Share;

namespace cfg
{
	public class MemberCardForBattleCfg
	{
		public const string Path = "cfg.MemberCardForBattleCfg.oc";

		private static Dictionary<int, MemberCardForBattleCfg> all;

		private static List<MemberCardForBattleCfg> allList;

		public int id;

		public int type;

		public int beforePickItemId;

		public int afterPickItemId;

		public bool isFirstPick;

		public int afterDropItemId;

		public MemberCardForBattleCfg(Octets oc)
		{
			id = oc.pop_int();
			type = oc.pop_int();
			beforePickItemId = oc.pop_int();
			afterPickItemId = oc.pop_int();
			isFirstPick = oc.pop_boolean();
			afterDropItemId = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, MemberCardForBattleCfg>();
			allList = new List<MemberCardForBattleCfg>();
			while (!octets.is_empty())
			{
				MemberCardForBattleCfg memberCardForBattleCfg = new MemberCardForBattleCfg(octets);
				all.Add(memberCardForBattleCfg.id, memberCardForBattleCfg);
				allList.Add(memberCardForBattleCfg);
			}
		}

		public static MemberCardForBattleCfg Get(int key)
		{
			MemberCardForBattleCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, MemberCardForBattleCfg> GetAll()
		{
			return all;
		}

		public static List<MemberCardForBattleCfg> GetAllList()
		{
			return allList;
		}
	}
}
