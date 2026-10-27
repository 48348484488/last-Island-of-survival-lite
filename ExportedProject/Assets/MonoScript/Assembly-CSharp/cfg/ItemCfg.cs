using System.Collections.Generic;
using Share;

namespace cfg
{
	public class ItemCfg
	{
		public const string Path = "cfg.ItemCfg.oc";

		private static Dictionary<int, ItemCfg> all;

		private static List<ItemCfg> allList;

		public int id;

		public string name;

		public int type;

		public int childType;

		public int quality;

		public int itemLevel;

		public bool isCanDiscard;

		public int durability;

		public int durabilityDownValue;

		public bool toQuickUse;

		public bool canUse;

		public string icon;

		public string desc;

		public int sellType;

		public int price;

		public int itemId;

		public bool isPileAble;

		public int dropId;

		public int level;

		public string modelPath;

		public string womanModelPath;

		public string root;

		public int maxPileNum;

		public int addCapacityValue;

		public string dropModel;

		public float useTime;

		public List<float> extras = new List<float>();

		public List<string> extrasstring = new List<string>();

		public int SexLimit;

		public ItemCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			type = oc.pop_int();
			childType = oc.pop_int();
			quality = oc.pop_int();
			itemLevel = oc.pop_int();
			isCanDiscard = oc.pop_boolean();
			durability = oc.pop_int();
			durabilityDownValue = oc.pop_int();
			toQuickUse = oc.pop_boolean();
			canUse = oc.pop_boolean();
			icon = oc.pop_string();
			desc = oc.pop_string();
			sellType = oc.pop_int();
			price = oc.pop_int();
			itemId = oc.pop_int();
			isPileAble = oc.pop_boolean();
			dropId = oc.pop_int();
			level = oc.pop_int();
			modelPath = oc.pop_string();
			womanModelPath = oc.pop_string();
			root = oc.pop_string();
			maxPileNum = oc.pop_int();
			addCapacityValue = oc.pop_int();
			dropModel = oc.pop_string();
			useTime = oc.pop_float();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				float item = oc.pop_float();
				extras.Add(item);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				string item2 = oc.pop_string();
				extrasstring.Add(item2);
			}
			SexLimit = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, ItemCfg>();
			allList = new List<ItemCfg>();
			while (!octets.is_empty())
			{
				ItemCfg itemCfg = new ItemCfg(octets);
				all.Add(itemCfg.id, itemCfg);
				allList.Add(itemCfg);
			}
		}

		public static ItemCfg Get(int key)
		{
			ItemCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, ItemCfg> GetAll()
		{
			return all;
		}

		public static List<ItemCfg> GetAllList()
		{
			return allList;
		}
	}
}
