using System.Collections.Generic;
using Share;

namespace cfg
{
	public class WorkbenchDevelopmentCfg
	{
		public const string Path = "cfg.WorkbenchDevelopmentCfg.oc";

		private static Dictionary<int, WorkbenchDevelopmentCfg> all;

		private static List<WorkbenchDevelopmentCfg> allList;

		public int id;

		public int requisiteId;

		public int requisiteNum;

		public int needTime;

		public int assistId;

		public int assistNum;

		public int dropId1;

		public int dropId2;

		public int baseId;

		public WorkbenchDevelopmentCfg(Octets oc)
		{
			id = oc.pop_int();
			requisiteId = oc.pop_int();
			requisiteNum = oc.pop_int();
			needTime = oc.pop_int();
			assistId = oc.pop_int();
			assistNum = oc.pop_int();
			dropId1 = oc.pop_int();
			dropId2 = oc.pop_int();
			baseId = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, WorkbenchDevelopmentCfg>();
			allList = new List<WorkbenchDevelopmentCfg>();
			while (!octets.is_empty())
			{
				WorkbenchDevelopmentCfg workbenchDevelopmentCfg = new WorkbenchDevelopmentCfg(octets);
				all.Add(workbenchDevelopmentCfg.id, workbenchDevelopmentCfg);
				allList.Add(workbenchDevelopmentCfg);
			}
		}

		public static WorkbenchDevelopmentCfg Get(int key)
		{
			WorkbenchDevelopmentCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, WorkbenchDevelopmentCfg> GetAll()
		{
			return all;
		}

		public static List<WorkbenchDevelopmentCfg> GetAllList()
		{
			return allList;
		}
	}
}
