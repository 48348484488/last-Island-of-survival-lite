using System.Collections.Generic;
using Share;

namespace cfg
{
	public class ModelCfg
	{
		public const string Path = "cfg.ModelCfg.oc";

		private static Dictionary<int, ModelCfg> all;

		private static List<ModelCfg> allList;

		public int id;

		public string name;

		public string icon;

		public string modelPath;

		public string color;

		public int type;

		public string root;

		public ModelCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			icon = oc.pop_string();
			modelPath = oc.pop_string();
			color = oc.pop_string();
			type = oc.pop_int();
			root = oc.pop_string();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, ModelCfg>();
			allList = new List<ModelCfg>();
			while (!octets.is_empty())
			{
				ModelCfg modelCfg = new ModelCfg(octets);
				all.Add(modelCfg.id, modelCfg);
				allList.Add(modelCfg);
			}
		}

		public static ModelCfg Get(int key)
		{
			ModelCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, ModelCfg> GetAll()
		{
			return all;
		}

		public static List<ModelCfg> GetAllList()
		{
			return allList;
		}
	}
}
