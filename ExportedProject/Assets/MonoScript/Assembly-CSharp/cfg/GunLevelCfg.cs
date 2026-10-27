using System;
using System.Collections.Generic;
using Share;

namespace cfg
{
	public class GunLevelCfg
	{
		public const string Path = "cfg.GunLevelCfg.oc";

		private static Dictionary<int, GunLevelCfg> all;

		private static List<GunLevelCfg> allList;

		public int id;

		public int levelColorType;

		public string model;

		public bool showEffect;

		public string killIcon;

		public string gunIcon;

		public List<GunLevelUpItemInfo> levelupItemInfos = new List<GunLevelUpItemInfo>();

		public bool chip2Open;

		public List<int> gunproperty = new List<int>();

		public int nextLevelId;

		public List<int> idsProp = new List<int>();

		public List<float> valuesProp = new List<float>();

		public GunLevelCfg(Octets oc)
		{
			id = oc.pop_int();
			levelColorType = oc.pop_int();
			model = oc.pop_string();
			showEffect = oc.pop_boolean();
			killIcon = oc.pop_string();
			gunIcon = oc.pop_string();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				GunLevelUpItemInfo item = new GunLevelUpItemInfo(oc);
				levelupItemInfos.Add(item);
			}
			chip2Open = oc.pop_boolean();
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				int item2 = oc.pop_int();
				gunproperty.Add(item2);
			}
			nextLevelId = oc.pop_int();
			string text = oc.pop_string();
			string[] array = text.Split('|');
			int k = 0;
			for (int num3 = array.Length; k < num3; k++)
			{
				int item3;
				try
				{
					item3 = int.Parse(array[k]);
				}
				catch (Exception)
				{
					continue;
				}
				idsProp.Add(item3);
			}
			string text2 = oc.pop_string();
			string[] array2 = text2.Split('|');
			int l = 0;
			for (int num4 = array2.Length; l < num4; l++)
			{
				float item4;
				try
				{
					item4 = float.Parse(array2[l]);
				}
				catch (Exception)
				{
					continue;
				}
				valuesProp.Add(item4);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, GunLevelCfg>();
			allList = new List<GunLevelCfg>();
			while (!octets.is_empty())
			{
				GunLevelCfg gunLevelCfg = new GunLevelCfg(octets);
				all.Add(gunLevelCfg.id, gunLevelCfg);
				allList.Add(gunLevelCfg);
			}
		}

		public static GunLevelCfg Get(int key)
		{
			GunLevelCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, GunLevelCfg> GetAll()
		{
			return all;
		}

		public static List<GunLevelCfg> GetAllList()
		{
			return allList;
		}
	}
}
