using System.Collections.Generic;
using Share;

namespace cfg
{
	public class FriendIntimacyCfg
	{
		public const string Path = "cfg.FriendIntimacyCfg.oc";

		private static Dictionary<int, FriendIntimacyCfg> all;

		private static List<FriendIntimacyCfg> allList;

		public int id;

		public int needValue;

		public int mailId;

		public FriendIntimacyCfg(Octets oc)
		{
			id = oc.pop_int();
			needValue = oc.pop_int();
			mailId = oc.pop_int();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, FriendIntimacyCfg>();
			allList = new List<FriendIntimacyCfg>();
			while (!octets.is_empty())
			{
				FriendIntimacyCfg friendIntimacyCfg = new FriendIntimacyCfg(octets);
				all.Add(friendIntimacyCfg.id, friendIntimacyCfg);
				allList.Add(friendIntimacyCfg);
			}
		}

		public static FriendIntimacyCfg Get(int key)
		{
			FriendIntimacyCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, FriendIntimacyCfg> GetAll()
		{
			return all;
		}

		public static List<FriendIntimacyCfg> GetAllList()
		{
			return allList;
		}
	}
}
