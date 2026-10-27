using System.Collections.Generic;
using Share;

namespace cfg
{
	public class WorkbenchRepairCfg
	{
		public const string Path = "cfg.WorkbenchRepairCfg.oc";

		private static Dictionary<int, WorkbenchRepairCfg> all;

		private static List<WorkbenchRepairCfg> allList;

		public int itemId;

		public int costType;

		public int costValue;

		public List<DrawingNeedMaterial> material = new List<DrawingNeedMaterial>();

		public WorkbenchRepairCfg(Octets oc)
		{
			itemId = oc.pop_int();
			costType = oc.pop_int();
			costValue = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				DrawingNeedMaterial item = new DrawingNeedMaterial(oc);
				material.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, WorkbenchRepairCfg>();
			allList = new List<WorkbenchRepairCfg>();
			while (!octets.is_empty())
			{
				WorkbenchRepairCfg workbenchRepairCfg = new WorkbenchRepairCfg(octets);
				all.Add(workbenchRepairCfg.itemId, workbenchRepairCfg);
				allList.Add(workbenchRepairCfg);
			}
		}

		public static WorkbenchRepairCfg Get(int key)
		{
			WorkbenchRepairCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, WorkbenchRepairCfg> GetAll()
		{
			return all;
		}

		public static List<WorkbenchRepairCfg> GetAllList()
		{
			return allList;
		}
	}
}
