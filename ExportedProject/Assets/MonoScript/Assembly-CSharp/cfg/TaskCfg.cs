using System.Collections.Generic;
using Share;

namespace cfg
{
	public class TaskCfg
	{
		public const string Path = "cfg.TaskCfg.oc";

		private static Dictionary<int, TaskCfg> all;

		private static List<TaskCfg> allList;

		public int id;

		public string name;

		public string icon;

		public int type;

		public int subType;

		public int flag;

		public int level;

		public string taskName;

		public int parentTaskId;

		public int difficult;

		public string desc;

		public string executeDesc;

		public int firstDropId;

		public int normalDropId;

		public string className;

		public List<int> args = new List<int>();

		public int dest;

		public int destParam;

		public int achieveType;

		public string achieveConditionDes;

		public int multiToOneDescId;

		public TaskCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			icon = oc.pop_string();
			type = oc.pop_int();
			subType = oc.pop_int();
			flag = oc.pop_int();
			level = oc.pop_int();
			taskName = oc.pop_string();
			parentTaskId = oc.pop_int();
			difficult = oc.pop_int();
			desc = oc.pop_string();
			executeDesc = oc.pop_string();
			firstDropId = oc.pop_int();
			normalDropId = oc.pop_int();
			className = oc.pop_string();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int item = oc.pop_int();
				args.Add(item);
			}
			dest = oc.pop_int();
			destParam = oc.pop_int();
			achieveType = oc.pop_int();
			achieveConditionDes = oc.pop_string();
			multiToOneDescId = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, TaskCfg>();
			allList = new List<TaskCfg>();
			while (!octets.is_empty())
			{
				TaskCfg taskCfg = new TaskCfg(octets);
				all.Add(taskCfg.id, taskCfg);
				allList.Add(taskCfg);
			}
		}

		public static TaskCfg Get(int key)
		{
			TaskCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, TaskCfg> GetAll()
		{
			return all;
		}

		public static List<TaskCfg> GetAllList()
		{
			return allList;
		}
	}
}
