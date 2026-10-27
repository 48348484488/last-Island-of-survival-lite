using System.Collections.Generic;
using Share;

namespace cfg
{
	public class SoundCfg
	{
		public const string Path = "cfg.SoundCfg.oc";

		private static Dictionary<int, SoundCfg> all;

		private static List<SoundCfg> allList;

		public int id;

		public string path;

		public int volume;

		public int radius;

		public SoundCfg(Octets oc)
		{
			id = oc.pop_int();
			path = oc.pop_string();
			volume = oc.pop_int();
			radius = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, SoundCfg>();
			allList = new List<SoundCfg>();
			while (!octets.is_empty())
			{
				SoundCfg soundCfg = new SoundCfg(octets);
				all.Add(soundCfg.id, soundCfg);
				allList.Add(soundCfg);
			}
		}

		public static SoundCfg Get(int key)
		{
			SoundCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, SoundCfg> GetAll()
		{
			return all;
		}

		public static List<SoundCfg> GetAllList()
		{
			return allList;
		}
	}
}
