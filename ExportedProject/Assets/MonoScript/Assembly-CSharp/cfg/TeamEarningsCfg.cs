using System;
using System.Collections.Generic;
using Share;

namespace cfg
{
	public class TeamEarningsCfg
	{
		public const string Path = "cfg.TeamEarningsCfg.oc";

		private static Dictionary<int, TeamEarningsCfg> all;

		private static List<TeamEarningsCfg> allList;

		public int id;

		public List<int> earningIds = new List<int>();

		public List<int> earningNums = new List<int>();

		public TeamEarningsCfg(Octets oc)
		{
			id = oc.pop_int();
			string text = oc.pop_string();
			string[] array = text.Split('|');
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				int item;
				try
				{
					item = int.Parse(array[i]);
				}
				catch (Exception)
				{
					continue;
				}
				earningIds.Add(item);
			}
			string text2 = oc.pop_string();
			string[] array2 = text2.Split('|');
			int j = 0;
			for (int num2 = array2.Length; j < num2; j++)
			{
				int item2;
				try
				{
					item2 = int.Parse(array2[j]);
				}
				catch (Exception)
				{
					continue;
				}
				earningNums.Add(item2);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, TeamEarningsCfg>();
			allList = new List<TeamEarningsCfg>();
			while (!octets.is_empty())
			{
				TeamEarningsCfg teamEarningsCfg = new TeamEarningsCfg(octets);
				all.Add(teamEarningsCfg.id, teamEarningsCfg);
				allList.Add(teamEarningsCfg);
			}
		}

		public static TeamEarningsCfg Get(int key)
		{
			TeamEarningsCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, TeamEarningsCfg> GetAll()
		{
			return all;
		}

		public static List<TeamEarningsCfg> GetAllList()
		{
			return allList;
		}
	}
}
