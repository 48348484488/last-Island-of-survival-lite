using System;
using System.Collections.Generic;
using Share;

namespace cfg
{
	public class GuideCfg
	{
		public const string Path = "cfg.GuideCfg.oc";

		private static Dictionary<int, GuideCfg> all;

		private static List<GuideCfg> allList;

		public int id;

		public Dictionary<int, GuideCondition> guideConditions = new Dictionary<int, GuideCondition>();

		public int finishStepNextId;

		public int passStepNextId;

		public int taskCfgId;

		public int nextTaskCfgId;

		public string guideText;

		public string guideTextParent;

		public string arrowGuideText;

		public int arrowGuideDir;

		public string guidePicParent;

		public string effectBtnName;

		public List<float> effectScale = new List<float>();

		public int targetItemId;

		public int specialFinishCondition;

		public GuideCfg(Octets oc)
		{
			id = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int key = oc.pop_int();
				GuideCondition value = new GuideCondition(oc);
				guideConditions.Add(key, value);
			}
			finishStepNextId = oc.pop_int();
			passStepNextId = oc.pop_int();
			taskCfgId = oc.pop_int();
			nextTaskCfgId = oc.pop_int();
			guideText = oc.pop_string();
			guideTextParent = oc.pop_string();
			arrowGuideText = oc.pop_string();
			arrowGuideDir = oc.pop_int();
			guidePicParent = oc.pop_string();
			effectBtnName = oc.pop_string();
			string text = oc.pop_string();
			string[] array = text.Split('|');
			int j = 0;
			for (int num2 = array.Length; j < num2; j++)
			{
				float item;
				try
				{
					item = float.Parse(array[j]);
				}
				catch (Exception)
				{
					continue;
				}
				effectScale.Add(item);
			}
			targetItemId = oc.pop_int();
			specialFinishCondition = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, GuideCfg>();
			allList = new List<GuideCfg>();
			while (!octets.is_empty())
			{
				GuideCfg guideCfg = new GuideCfg(octets);
				all.Add(guideCfg.id, guideCfg);
				allList.Add(guideCfg);
			}
		}

		public static GuideCfg Get(int key)
		{
			GuideCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, GuideCfg> GetAll()
		{
			return all;
		}

		public static List<GuideCfg> GetAllList()
		{
			return allList;
		}
	}
}
