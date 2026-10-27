using System.Collections.Generic;
using Share;

namespace cfg
{
	public class DrawingCfg
	{
		public const string Path = "cfg.DrawingCfg.oc";

		private static Dictionary<int, DrawingCfg> all;

		private static List<DrawingCfg> allList;

		public int id;

		public string name;

		public int noUseId;

		public int typeShow;

		public int bigType;

		public int smallType;

		public bool isNormal;

		public bool isShowLocked;

		public bool isDefault;

		public int levelLimit;

		public int needWorkbench;

		public int needWorkbenchLevel;

		public int needTime;

		public List<DrawingNeedMaterial> material = new List<DrawingNeedMaterial>();

		public int targetItemId;

		public int onceMaxCreate;

		public int order;

		public int baseNumber;

		public int productNum;

		public DrawingCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			noUseId = oc.pop_int();
			typeShow = oc.pop_int();
			bigType = oc.pop_int();
			smallType = oc.pop_int();
			isNormal = oc.pop_boolean();
			isShowLocked = oc.pop_boolean();
			isDefault = oc.pop_boolean();
			levelLimit = oc.pop_int();
			needWorkbench = oc.pop_int();
			needWorkbenchLevel = oc.pop_int();
			needTime = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				DrawingNeedMaterial item = new DrawingNeedMaterial(oc);
				material.Add(item);
			}
			targetItemId = oc.pop_int();
			onceMaxCreate = oc.pop_int();
			order = oc.pop_int();
			baseNumber = oc.pop_int();
			productNum = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, DrawingCfg>();
			allList = new List<DrawingCfg>();
			while (!octets.is_empty())
			{
				DrawingCfg drawingCfg = new DrawingCfg(octets);
				all.Add(drawingCfg.id, drawingCfg);
				allList.Add(drawingCfg);
			}
		}

		public static DrawingCfg Get(int key)
		{
			DrawingCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, DrawingCfg> GetAll()
		{
			return all;
		}

		public static List<DrawingCfg> GetAllList()
		{
			return allList;
		}
	}
}
