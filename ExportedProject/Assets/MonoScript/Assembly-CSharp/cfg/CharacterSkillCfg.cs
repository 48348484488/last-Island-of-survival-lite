using System.Collections.Generic;
using Share;

namespace cfg
{
	public class CharacterSkillCfg
	{
		public const string Path = "cfg.CharacterSkillCfg.oc";

		private static Dictionary<int, CharacterSkillCfg> all;

		private static List<CharacterSkillCfg> allList;

		public int id;

		public string icon;

		public string name;

		public string desc;

		public string type;

		public int skilltype;

		public List<float> skillvalues = new List<float>();

		public CharacterSkillCfg(Octets oc)
		{
			id = oc.pop_int();
			icon = oc.pop_string();
			name = oc.pop_string();
			desc = oc.pop_string();
			type = oc.pop_string();
			skilltype = oc.pop_int();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				float item = oc.pop_float();
				skillvalues.Add(item);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, CharacterSkillCfg>();
			allList = new List<CharacterSkillCfg>();
			while (!octets.is_empty())
			{
				CharacterSkillCfg characterSkillCfg = new CharacterSkillCfg(octets);
				all.Add(characterSkillCfg.id, characterSkillCfg);
				allList.Add(characterSkillCfg);
			}
		}

		public static CharacterSkillCfg Get(int key)
		{
			CharacterSkillCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, CharacterSkillCfg> GetAll()
		{
			return all;
		}

		public static List<CharacterSkillCfg> GetAllList()
		{
			return allList;
		}
	}
}
