using System.Collections.Generic;
using Share;

namespace cfg
{
	public class WorkbenchLevelUpCfg
	{
		public const string Path = "cfg.WorkbenchLevelUpCfg.oc";

		private static Dictionary<int, WorkbenchLevelUpCfg> all;

		private static List<WorkbenchLevelUpCfg> allList;

		public int workbenchLevel;

		public int needTime;

		public List<DrawingNeedMaterial> material = new List<DrawingNeedMaterial>();

		public WorkbenchLevelUpCfg(Octets oc)
		{
			workbenchLevel = oc.pop_int();
			needTime = oc.pop_int();
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
			all = new Dictionary<int, WorkbenchLevelUpCfg>();
			allList = new List<WorkbenchLevelUpCfg>();
			while (!octets.is_empty())
			{
				WorkbenchLevelUpCfg workbenchLevelUpCfg = new WorkbenchLevelUpCfg(octets);
				all.Add(workbenchLevelUpCfg.workbenchLevel, workbenchLevelUpCfg);
				allList.Add(workbenchLevelUpCfg);
			}
		}

		public static WorkbenchLevelUpCfg Get(int key)
		{
			WorkbenchLevelUpCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, WorkbenchLevelUpCfg> GetAll()
		{
			return all;
		}

		public static List<WorkbenchLevelUpCfg> GetAllList()
		{
			return allList;
		}
	}
}
