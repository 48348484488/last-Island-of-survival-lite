using System.Collections.Generic;
using Share;

namespace cfg
{
	public class ShopCfg
	{
		public const string Path = "cfg.ShopCfg.oc";

		private static Dictionary<int, ShopCfg> all;

		private static List<ShopCfg> allList;

		public int id;

		public string icon;

		public int itemBigType;

		public int itemSmallType;

		public int itemTag;

		public int moneyType;

		public int price;

		public int discount;

		public bool isDiscountXianShi;

		public string discountStartTime;

		public string discountEndTime;

		public int buyNumLimit;

		public bool isXianShiChuShou;

		public string shangJiaDate;

		public string xiaJiaDate;

		public int extraDropId;

		public string desc;

		public List<ShopBuyCondition> buyConditions = new List<ShopBuyCondition>();

		public int vipZeng;

		public string strJump;

		public string strJumpPage;

		public int sortArg;

		public bool isShowModel;

		public bool isShowInNormal;

		public ShopCfg(Octets oc)
		{
			id = oc.pop_int();
			icon = oc.pop_string();
			itemBigType = oc.pop_int();
			itemSmallType = oc.pop_int();
			itemTag = oc.pop_int();
			moneyType = oc.pop_int();
			price = oc.pop_int();
			discount = oc.pop_int();
			isDiscountXianShi = oc.pop_boolean();
			discountStartTime = oc.pop_string();
			discountEndTime = oc.pop_string();
			buyNumLimit = oc.pop_int();
			isXianShiChuShou = oc.pop_boolean();
			shangJiaDate = oc.pop_string();
			xiaJiaDate = oc.pop_string();
			extraDropId = oc.pop_int();
			desc = oc.pop_string();
			int i = 0;
			for (int num = oc.pop_int(); i < num; i++)
			{
				ShopBuyCondition item = new ShopBuyCondition(oc);
				buyConditions.Add(item);
			}
			vipZeng = oc.pop_int();
			strJump = oc.pop_string();
			strJumpPage = oc.pop_string();
			sortArg = oc.pop_int();
			isShowModel = oc.pop_boolean();
			isShowInNormal = oc.pop_boolean();
		}

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			all = new Dictionary<int, ShopCfg>();
			allList = new List<ShopCfg>();
			while (!octets.is_empty())
			{
				ShopCfg shopCfg = new ShopCfg(octets);
				all.Add(shopCfg.id, shopCfg);
				allList.Add(shopCfg);
			}
		}

		public static ShopCfg Get(int key)
		{
			ShopCfg value = null;
			all.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<int, ShopCfg> GetAll()
		{
			return all;
		}

		public static List<ShopCfg> GetAllList()
		{
			return allList;
		}
	}
}
