using System.Collections.Generic;
using Share;

namespace cfg
{
	public class EstablishRole
	{
		public const string Path = "cfg.EstablishRole.oc";

		private static Dictionary<int, EstablishRole> all;

		private static List<EstablishRole> allList;

		public int id;

		public bool sex;

		public string icon;

		public List<int> head = new List<int>();

		public List<int> Clothes = new List<int>();

		public List<int> Pants = new List<int>();

		public List<int> Shoes = new List<int>();

		public List<int> Glove = new List<int>();

		public List<int> face = new List<int>();

		public List<int> basicEquip = new List<int>();

		public EstablishRole(Octets oc)
		{
			id = oc.pop_int();
			sex = oc.pop_bool();
			icon = oc.pop_string();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				int item = oc.pop_int();
				head.Add(item);
			}
			int j = 0;
			for (int num2 = oc.pop_int(); j < num2; j++)
			{
				int item2 = oc.pop_int();
				Clothes.Add(item2);
			}
			int k = 0;
			for (int num3 = oc.pop_int(); k < num3; k++)
			{
				int item3 = oc.pop_int();
				Pants.Add(item3);
			}
			int l = 0;
			for (int num4 = oc.pop_int(); l < num4; l++)
			{
				int item4 = oc.pop_int();
				Shoes.Add(item4);
			}
			int m = 0;
			for (int num5 = oc.pop_int(); m < num5; m++)
			{
				int item5 = oc.pop_int();
				Glove.Add(item5);
			}
			int n = 0;
			for (int num6 = oc.pop_int(); n < num6; n++)
			{
				int item6 = oc.pop_int();
				face.Add(item6);
			}
			int num7 = 0;
			for (int num8 = oc.pop_int(); num7 < num8; num7++)
			{
				int item7 = oc.pop_int();
				basicEquip.Add(item7);
			}
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, EstablishRole>();
			allList = new List<EstablishRole>();
			while (!octets.is_empty())
			{
				EstablishRole establishRole = new EstablishRole(octets);
				all.Add(establishRole.id, establishRole);
				allList.Add(establishRole);
			}
		}

		public static EstablishRole Get(int key)
		{
			EstablishRole value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, EstablishRole> GetAll()
		{
			return all;
		}

		public static List<EstablishRole> GetAllList()
		{
			return allList;
		}
	}
}
