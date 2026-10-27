using System.Collections.Generic;
using Share;

namespace cfg
{
	public class GunCfg
	{
		public const string Path = "cfg.GunCfg.oc";

		private static Dictionary<int, GunCfg> all;

		private static List<GunCfg> allList;

		public int id;

		public string name;

		public int gunTypeId;

		public int gunType;

		public int gunTypeInGaunPanel;

		public string gunTypeName;

		public bool needLashuan;

		public int aimPointType;

		public List<int> audios = new List<int>();

		public List<int> effects = new List<int>();

		public List<float> moveSpeedFactors = new List<float>();

		public int lianfaBulletInterval;

		public int ZeroPointDistance;

		public bool canLianfa;

		public bool canAuto;

		public int lianfaBulletNum;

		public int sandanBulletNum;

		public List<float> factors = new List<float>();

		public bool bulletDown;

		public List<int> muzzleParts = new List<int>();

		public List<int> propParts = new List<int>();

		public List<int> aimParts = new List<int>();

		public List<int> clipParts = new List<int>();

		public List<int> qiangbaParts = new List<int>();

		public List<int> bulletIds = new List<int>();

		public int bulletNumAutoPickup;

		public string zeroChangeBulletPa;

		public string changeBulletPa;

		public string zeroChangeBulletZhan;

		public string changeBulletZhan;

		public string zhuangtianZhan01;

		public string zhuangtianZhan02;

		public string zhuangtianZhan03;

		public string zhuangtianPa01;

		public string zhuangtianPa02;

		public string zhuangtianPa03;

		public string shootZhan;

		public string shootDun;

		public string shootPa;

		public string aimZhan;

		public string aimZhanRush;

		public string aimDun;

		public string aimDunRush;

		public string aimPa;

		public int gunAimType;

		public string gunShootAim;

		public bool bShow;

		public int shopid;

		public List<GunProperty> gunproperty = new List<GunProperty>();

		public int skinid;

		public int upLevelStartId;

		public int composeClipId;

		public int composeClipCount;

		public int decomposeClipCount;

		public string levelBigStarShi;

		public string levelBigStarKong;

		public string levelSmallStar;

		public int clipId1;

		public int clipId2;

		public GunCfg(Octets oc)
		{
			id = oc.pop_int();
			name = oc.pop_string();
			gunTypeId = oc.pop_int();
			gunType = oc.pop_int();
			gunTypeInGaunPanel = oc.pop_int();
			gunTypeName = oc.pop_string();
			needLashuan = oc.pop_boolean();
			aimPointType = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int item = oc.pop_int();
				audios.Add(item);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				int item2 = oc.pop_int();
				effects.Add(item2);
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				float item3 = oc.pop_float();
				moveSpeedFactors.Add(item3);
			}
			lianfaBulletInterval = oc.pop_int();
			ZeroPointDistance = oc.pop_int();
			canLianfa = oc.pop_boolean();
			canAuto = oc.pop_boolean();
			lianfaBulletNum = oc.pop_int();
			sandanBulletNum = oc.pop_int();
			int l = 0;
			for (int num4 = oc.pop_int(); l < num4; l++)
			{
				float item4 = oc.pop_float();
				factors.Add(item4);
			}
			bulletDown = oc.pop_boolean();
			int m = 0;
			for (int num5 = oc.pop_int(); m < num5; m++)
			{
				int item5 = oc.pop_int();
				muzzleParts.Add(item5);
			}
			int n = 0;
			for (int num6 = oc.pop_int(); n < num6; n++)
			{
				int item6 = oc.pop_int();
				propParts.Add(item6);
			}
			int num7 = 0;
			for (int num8 = oc.pop_int(); num7 < num8; num7++)
			{
				int item7 = oc.pop_int();
				aimParts.Add(item7);
			}
			int num9 = 0;
			for (int num10 = oc.pop_int(); num9 < num10; num9++)
			{
				int item8 = oc.pop_int();
				clipParts.Add(item8);
			}
			int num11 = 0;
			for (int num12 = oc.pop_int(); num11 < num12; num11++)
			{
				int item9 = oc.pop_int();
				qiangbaParts.Add(item9);
			}
			int num13 = 0;
			for (int num14 = oc.pop_int(); num13 < num14; num13++)
			{
				int item10 = oc.pop_int();
				bulletIds.Add(item10);
			}
			bulletNumAutoPickup = oc.pop_int();
			zeroChangeBulletPa = oc.pop_string();
			changeBulletPa = oc.pop_string();
			zeroChangeBulletZhan = oc.pop_string();
			changeBulletZhan = oc.pop_string();
			zhuangtianZhan01 = oc.pop_string();
			zhuangtianZhan02 = oc.pop_string();
			zhuangtianZhan03 = oc.pop_string();
			zhuangtianPa01 = oc.pop_string();
			zhuangtianPa02 = oc.pop_string();
			zhuangtianPa03 = oc.pop_string();
			shootZhan = oc.pop_string();
			shootDun = oc.pop_string();
			shootPa = oc.pop_string();
			aimZhan = oc.pop_string();
			aimZhanRush = oc.pop_string();
			aimDun = oc.pop_string();
			aimDunRush = oc.pop_string();
			aimPa = oc.pop_string();
			gunAimType = oc.pop_int();
			gunShootAim = oc.pop_string();
			bShow = oc.pop_boolean();
			shopid = oc.pop_int();
			int num15 = 0;
			for (int num16 = oc.pop_int(); num15 < num16; num15++)
			{
				GunProperty item11 = new GunProperty(oc);
				gunproperty.Add(item11);
			}
			skinid = oc.pop_int();
			upLevelStartId = oc.pop_int();
			composeClipId = oc.pop_int();
			composeClipCount = oc.pop_int();
			decomposeClipCount = oc.pop_int();
			levelBigStarShi = oc.pop_string();
			levelBigStarKong = oc.pop_string();
			levelSmallStar = oc.pop_string();
			clipId1 = oc.pop_int();
			clipId2 = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, GunCfg>();
			allList = new List<GunCfg>();
			while (!octets.is_empty())
			{
				GunCfg gunCfg = new GunCfg(octets);
				all.Add(gunCfg.id, gunCfg);
				allList.Add(gunCfg);
			}
		}

		public static GunCfg Get(int key)
		{
			GunCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, GunCfg> GetAll()
		{
			return all;
		}

		public static List<GunCfg> GetAllList()
		{
			return allList;
		}
	}
}
