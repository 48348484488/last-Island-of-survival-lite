using System.Collections.Generic;
using Share;

namespace cfg
{
	public class SceneSoundCfg
	{
		public const string Path = "cfg.SceneSoundCfg.oc";

		private static Dictionary<int, SceneSoundCfg> all;

		private static List<SceneSoundCfg> allList;

		public int id;

		public int soundId;

		public int deltaTime;

		public int spaceTime;

		public string sceneName;

		public int dayType;

		public SceneSoundCfg(Octets oc)
		{
			id = oc.pop_int();
			soundId = oc.pop_int();
			deltaTime = oc.pop_int();
			spaceTime = oc.pop_int();
			sceneName = oc.pop_string();
			dayType = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, SceneSoundCfg>();
			allList = new List<SceneSoundCfg>();
			while (!octets.is_empty())
			{
				SceneSoundCfg sceneSoundCfg = new SceneSoundCfg(octets);
				all.Add(sceneSoundCfg.id, sceneSoundCfg);
				allList.Add(sceneSoundCfg);
			}
		}

		public static SceneSoundCfg Get(int key)
		{
			SceneSoundCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, SceneSoundCfg> GetAll()
		{
			return all;
		}

		public static List<SceneSoundCfg> GetAllList()
		{
			return allList;
		}
	}
}
