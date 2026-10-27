using System.Collections.Generic;
using Share;

namespace cfg
{
	public class EquipCfg
	{
		public const string Path = "cfg.EquipCfg.oc";

		private static Dictionary<int, EquipCfg> all;

		private static List<EquipCfg> allList;

		public int id;

		public string name;

		public int equipType;

		public int cellIndex;

		public List<float> props = new List<float>();

		public List<float> propPercents = new List<float>();

		public EquipCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			equipType = oc.pop_int();
			cellIndex = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				float item = oc.pop_float();
				props.Add(item);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				float item2 = oc.pop_float();
				propPercents.Add(item2);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, EquipCfg>();
			allList = new List<EquipCfg>();
			while (!octets.is_empty())
			{
				EquipCfg equipCfg = new EquipCfg(octets);
				all.Add(equipCfg.id, equipCfg);
				allList.Add(equipCfg);
			}
		}

		public static EquipCfg Get(int key)
		{
			EquipCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, EquipCfg> GetAll()
		{
			return all;
		}

		public static List<EquipCfg> GetAllList()
		{
			return allList;
		}
	}
}
