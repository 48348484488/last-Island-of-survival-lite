using System;
using System.Collections.Generic;
using Share;

namespace cfg
{
	public class BuildPart
	{
		public const string Path = "cfg.BuildPart.oc";

		private static Dictionary<int, BuildPart> all;

		private static List<BuildPart> allList;

		public int id;

		public int type;

		public int functionType;

		public string name;

		public int lv;

		public int lv1Id;

		public int nextId;

		public string icon;

		public bool canUpgrade;

		public string model;

		public bool basic;

		public int needSupportValue;

		public int destoryEffectId;

		public int buildEffectId;

		public int upLvEffectId;

		public bool needCheck;

		public List<int> btns = new List<int>();

		public List<int> otherPlayerbtns = new List<int>();

		public MaterialInfo materialInfo;

		public int buildSoundId;

		public int destorySoundId;

		public int upSoundId;

		public bool isDoor;

		public int OnceAngle;

		public bool canFix;

		public List<FixBuildMaterialInfo> fixMaterialInfos = new List<FixBuildMaterialInfo>();

		public bool canRename;

		public bool canAutoUpgrade;

		public BuildPart(Octets oc)
		{
			id = oc.pop_int();
			type = oc.pop_int();
			functionType = oc.pop_int();
			name = oc.pop_string();
			lv = oc.pop_int();
			lv1Id = oc.pop_int();
			nextId = oc.pop_int();
			icon = oc.pop_string();
			canUpgrade = oc.pop_boolean();
			model = oc.pop_string();
			basic = oc.pop_boolean();
			needSupportValue = oc.pop_int();
			destoryEffectId = oc.pop_int();
			buildEffectId = oc.pop_int();
			upLvEffectId = oc.pop_int();
			needCheck = oc.pop_boolean();
			string text = oc.pop_string();
			string[] array = text.Split('|');
			int i = 0;
			for (int num = array.Length; i < num; i++)
			{
				int item;
				try
				{
					item = int.Parse(array[i]);
				}
				catch (Exception)
				{
					continue;
				}
				btns.Add(item);
			}
			string text2 = oc.pop_string();
			string[] array2 = text2.Split('|');
			int j = 0;
			for (int num2 = array2.Length; j < num2; j++)
			{
				int item2;
				try
				{
					item2 = int.Parse(array2[j]);
				}
				catch (Exception)
				{
					continue;
				}
				otherPlayerbtns.Add(item2);
			}
			materialInfo = new MaterialInfo(oc);
			buildSoundId = oc.pop_int();
			destorySoundId = oc.pop_int();
			upSoundId = oc.pop_int();
			isDoor = oc.pop_boolean();
			OnceAngle = oc.pop_int();
			canFix = oc.pop_boolean();
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				FixBuildMaterialInfo item3 = new FixBuildMaterialInfo(oc);
				fixMaterialInfos.Add(item3);
			}
			canRename = oc.pop_boolean();
			canAutoUpgrade = oc.pop_boolean();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, BuildPart>();
			allList = new List<BuildPart>();
			while (!octets.is_empty())
			{
				BuildPart buildPart = new BuildPart(octets);
				all.Add(buildPart.id, buildPart);
				allList.Add(buildPart);
			}
		}

		public static BuildPart Get(int key)
		{
			BuildPart value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, BuildPart> GetAll()
		{
			return all;
		}

		public static List<BuildPart> GetAllList()
		{
			return allList;
		}
	}
}
