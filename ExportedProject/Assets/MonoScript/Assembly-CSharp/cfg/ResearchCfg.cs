using System.Collections.Generic;
using Share;

namespace cfg
{
	public class ResearchCfg
	{
		public const string Path = "cfg.ResearchCfg.oc";

		private static Dictionary<int, ResearchCfg> all;

		private static List<ResearchCfg> allList;

		public int id;

		public int requisiteId;

		public int requisiteNum;

		public int needTime;

		public int assistId;

		public int assistNum;

		public int drawingId1;

		public int probability1;

		public int drawingId2;

		public int probability2;

		public int failedDorpId;

		public ResearchCfg(Octets oc)
		{
			id = oc.pop_int();
			requisiteId = oc.pop_int();
			requisiteNum = oc.pop_int();
			needTime = oc.pop_int();
			assistId = oc.pop_int();
			assistNum = oc.pop_int();
			drawingId1 = oc.pop_int();
			probability1 = oc.pop_int();
			drawingId2 = oc.pop_int();
			probability2 = oc.pop_int();
			failedDorpId = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, ResearchCfg>();
			allList = new List<ResearchCfg>();
			while (!octets.is_empty())
			{
				ResearchCfg researchCfg = new ResearchCfg(octets);
				all.Add(researchCfg.id, researchCfg);
				allList.Add(researchCfg);
			}
		}

		public static ResearchCfg Get(int key)
		{
			ResearchCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, ResearchCfg> GetAll()
		{
			return all;
		}

		public static List<ResearchCfg> GetAllList()
		{
			return allList;
		}
	}
}
