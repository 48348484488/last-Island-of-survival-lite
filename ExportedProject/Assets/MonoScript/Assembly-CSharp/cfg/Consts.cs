using Share;

namespace cfg
{
	public sealed class Consts
	{
		public const string Path = "cfg.Consts.oc";

		public static int MIN_NAME_LENGTH = 1;

		public static int MAX_NAME_LENGTH = 12;

		public static int MAX_MAIL_KEEP_NUM = 30;

		public static bool IS_APPLE_CHECK;

		public static int MAX_ROLE_NAME_LENGTH = 12;

		public static int MIN_ROLE_NAME_LENGTH = 4;

		public static int GAIMINGKA_ITEM_ID = 7;

		public static int XIAOLABA_ITEM_ID = 5;

		public static int DALABA_ITEM_ID = 6;

		public static int CHAT_WORLD_MAX_TIME = 30;

		public static int BIG_HORN_SHOW_TIME_CLIENT = 5;

		public static int BIG_HORN_SHOW_TIME_SERVER = 3;

		public static float WAIT_SHOW_NEW_HORN = 0.1f;

		public static int MAX_CHAT_WORD_NUM = 30;

		public static int CAN_CHAT_LEVEL = 3;

		public static float TIME_PRESS_SHOW_DESC = 0.8f;

		public static int MAX_FRIEND_REC_NUM = 99;

		public static string FRIEND_FANS_ICON = "common/common_FriendPanel_weiguanzhu";

		public static string FRIEND_CARE_ICON = "common/common_FriendPanel_yiguanzhu";

		public static string FRIEND_EACH_ICON = "common/common_FriendPanel_xianghu";

		public static int FRIEND_MAX_NUM = 200;

		public static int DEFAULT_BAG_CAPACITY = 30;

		public static int MAX_QUICKUSEITEM = 7;

		public static int MAX_TEAMATE_NUM = 4;

		public static int MAX_TEAM_APPLY_NUM = 10;

		public static int TROOP_RECRUIT_TIME_LIMIT = 30;

		public static string WATER_ICON = "com_buff_shui";

		public static string HUNGER_ICON = "com_buff_jie";

		public static string HEALTH_ICON = "com_buff_xue";

		public static int COOK_DARK_DISHES_ITEMID = 55212;

		public static int COOK_DARK_DISHES_TIME = 10;

		public static string GOLD_ICON = "common_money_jinbi";

		public static string DIAMOND_ICON = "common_money_baoshi";

		public static string COUPON_ICON = "common_money_dianquan";

		public static int TASK_BOX_MAX_COUNT = 10;

		public static int BUY_SPECIAL_LADDER_TASK_PRICE = 100;

		public static int BUY_SUPER_LADDER_TASK_PRICE = 500;

		public static int BUY_SUPER_LADDER_TASK_DROPID = 1199;

		public static string WORKBENCH_LEVEL_ONE_ICON = "work_1";

		public static string WORKBENCH_LEVEL_TWO_ICON = "work_2";

		public static string WORKBENCH_LEVEL_THREE_ICON = "work_3";

		public static string LADDDER_LEVEL_IOCN = "com_sj_1";

		public static float GUN_SOUND_DISTANCE = 400f;

		public static float AIRDROP_SOUND_DISTANCE = 1000f;

		public static float CAR_SOUND_DISTANCE = 100f;

		public static string TREE1_ICON_IN_MAP = "common_money_jinbi";

		public static string TREE2_ICON_IN_MAP = "common_money_jinbi";

		public static string TREE3_ICON_IN_MAP = "common_money_jinbi";

		public static string LAJITONG1_ICON_IN_MAP = "common_money_jinbi";

		public static string LAJITONG2_ICON_IN_MAP = "common_money_jinbi";

		public static string XIANGZI1_ICON_IN_MAP = "common_money_jinbi";

		public static string XIANGZI2_ICON_IN_MAP = "common_money_jinbi";

		public static string XIANGZI3_ICON_IN_MAP = "common_money_jinbi";

		public static int LOCK_PASSWORD_LENGTH = 4;

		public static int BED_CD_TIME = 60;

		public static int SLEEPING_BED_CD_TIME = 120;

		public static int MANOR_CHEST_CAPACITY = 8;

		public static int MANOR_CHEST_MAX_SHARE_NUMBER = 20;

		public static int PASSWORD_LOCK_ITEM_ID = 26007;

		public static int MAX_BED_NUMBER = 2;

		public static int MAX_SLEEPING_BAG_NUMBER = 5;

		public static int MAX_TOOLBOX_NUMBER = 5;

		public static int PRODUCE_SHORT_CUT_NUM = 7;

		public static int PRODUCE_CREATE_NUM = 4;

		public static int CUPON_TO_DIAMOND = 10;

		public static int RANDOM_SHOP_REFRESH_TIME = 8;

		public static int BUILDING_NAME_LENGTH_MAX = 8;

		public static int ELECTRICAL_COMPONENTS_TREE_STRATUM_MAX = 8;

		public static int MAX_PILE_NUM_ELECTRIC_FUEL = 9999;

		public static int FIRST_CHARGE_DROPID = 3333;

		public static int SIGN_IN_MISSED_DAY_PRICE = 50;

		public static int GUIDE_PIC_TEXT_SHOW_TIME = 5;

		public static float TURRET_ROTATE_SPEED = 15f;

		public static int ELECTRIC_DELAY_SWITCH_DEFAULT_TIME = 3;

		public static int ELECTRIC_DELAY_SWITCH_TIME_MIN = 1;

		public static int ELECTRIC_DELAY_SWITCH_TIME_MAX = 10;

		public static int GUIDE_PICK_ITEM_ID = 50027;

		public static int STONE_ITEM_ID = 50006;

		public static int IRON_ITEM_ID = 50008;

		public static int STEEL_ITEM_ID = 50010;

		public static string LADDER_TASK_BTN_NAME = "Layer_0/BattlePanel/m_zhu/m_zsj/GameObject/Image/btn_ladder_task";

		public static int TRUSTEESHIP_HP_DEFAULT = 70;

		public static int TRUSTEESHIP_HUNGER_DEFAULT = 70;

		public static int TEAM_EARNINGREWARD_MAX_GETCOUNT = 5;

		public static int TEAM_EARNINGR_DROP_EWARDID = 55212;

		public static int TEAM_EARNING_MAX_VALUE = 100;

		public static int REFRESH_RECOMMENDFRIENDS_CD = 3600;

		public static int MAX_RECOMMENDFRIENDS_NUM = 9;

		public static int MANOR_CHEST_ITEM_ID = 26006;

		public static float LOGIN_SDK_TIME = 30f;

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			MIN_NAME_LENGTH = octets.pop_int();
			MAX_NAME_LENGTH = octets.pop_int();
			MAX_MAIL_KEEP_NUM = octets.pop_int();
			IS_APPLE_CHECK = octets.pop_boolean();
			MAX_ROLE_NAME_LENGTH = octets.pop_int();
			MIN_ROLE_NAME_LENGTH = octets.pop_int();
			GAIMINGKA_ITEM_ID = octets.pop_int();
			XIAOLABA_ITEM_ID = octets.pop_int();
			DALABA_ITEM_ID = octets.pop_int();
			CHAT_WORLD_MAX_TIME = octets.pop_int();
			BIG_HORN_SHOW_TIME_CLIENT = octets.pop_int();
			BIG_HORN_SHOW_TIME_SERVER = octets.pop_int();
			WAIT_SHOW_NEW_HORN = octets.pop_float();
			MAX_CHAT_WORD_NUM = octets.pop_int();
			CAN_CHAT_LEVEL = octets.pop_int();
			TIME_PRESS_SHOW_DESC = octets.pop_float();
			MAX_FRIEND_REC_NUM = octets.pop_int();
			FRIEND_FANS_ICON = octets.pop_string();
			FRIEND_CARE_ICON = octets.pop_string();
			FRIEND_EACH_ICON = octets.pop_string();
			FRIEND_MAX_NUM = octets.pop_int();
			DEFAULT_BAG_CAPACITY = octets.pop_int();
			MAX_QUICKUSEITEM = octets.pop_int();
			MAX_TEAMATE_NUM = octets.pop_int();
			MAX_TEAM_APPLY_NUM = octets.pop_int();
			TROOP_RECRUIT_TIME_LIMIT = octets.pop_int();
			WATER_ICON = octets.pop_string();
			HUNGER_ICON = octets.pop_string();
			HEALTH_ICON = octets.pop_string();
			COOK_DARK_DISHES_ITEMID = octets.pop_int();
			COOK_DARK_DISHES_TIME = octets.pop_int();
			GOLD_ICON = octets.pop_string();
			DIAMOND_ICON = octets.pop_string();
			COUPON_ICON = octets.pop_string();
			TASK_BOX_MAX_COUNT = octets.pop_int();
			BUY_SPECIAL_LADDER_TASK_PRICE = octets.pop_int();
			BUY_SUPER_LADDER_TASK_PRICE = octets.pop_int();
			BUY_SUPER_LADDER_TASK_DROPID = octets.pop_int();
			WORKBENCH_LEVEL_ONE_ICON = octets.pop_string();
			WORKBENCH_LEVEL_TWO_ICON = octets.pop_string();
			WORKBENCH_LEVEL_THREE_ICON = octets.pop_string();
			LADDDER_LEVEL_IOCN = octets.pop_string();
			GUN_SOUND_DISTANCE = octets.pop_float();
			AIRDROP_SOUND_DISTANCE = octets.pop_float();
			CAR_SOUND_DISTANCE = octets.pop_float();
			TREE1_ICON_IN_MAP = octets.pop_string();
			TREE2_ICON_IN_MAP = octets.pop_string();
			TREE3_ICON_IN_MAP = octets.pop_string();
			LAJITONG1_ICON_IN_MAP = octets.pop_string();
			LAJITONG2_ICON_IN_MAP = octets.pop_string();
			XIANGZI1_ICON_IN_MAP = octets.pop_string();
			XIANGZI2_ICON_IN_MAP = octets.pop_string();
			XIANGZI3_ICON_IN_MAP = octets.pop_string();
			LOCK_PASSWORD_LENGTH = octets.pop_int();
			BED_CD_TIME = octets.pop_int();
			SLEEPING_BED_CD_TIME = octets.pop_int();
			MANOR_CHEST_CAPACITY = octets.pop_int();
			MANOR_CHEST_MAX_SHARE_NUMBER = octets.pop_int();
			PASSWORD_LOCK_ITEM_ID = octets.pop_int();
			MAX_BED_NUMBER = octets.pop_int();
			MAX_SLEEPING_BAG_NUMBER = octets.pop_int();
			MAX_TOOLBOX_NUMBER = octets.pop_int();
			PRODUCE_SHORT_CUT_NUM = octets.pop_int();
			PRODUCE_CREATE_NUM = octets.pop_int();
			CUPON_TO_DIAMOND = octets.pop_int();
			RANDOM_SHOP_REFRESH_TIME = octets.pop_int();
			BUILDING_NAME_LENGTH_MAX = octets.pop_int();
			ELECTRICAL_COMPONENTS_TREE_STRATUM_MAX = octets.pop_int();
			MAX_PILE_NUM_ELECTRIC_FUEL = octets.pop_int();
			FIRST_CHARGE_DROPID = octets.pop_int();
			SIGN_IN_MISSED_DAY_PRICE = octets.pop_int();
			GUIDE_PIC_TEXT_SHOW_TIME = octets.pop_int();
			TURRET_ROTATE_SPEED = octets.pop_float();
			ELECTRIC_DELAY_SWITCH_DEFAULT_TIME = octets.pop_int();
			ELECTRIC_DELAY_SWITCH_TIME_MIN = octets.pop_int();
			ELECTRIC_DELAY_SWITCH_TIME_MAX = octets.pop_int();
			GUIDE_PICK_ITEM_ID = octets.pop_int();
			STONE_ITEM_ID = octets.pop_int();
			IRON_ITEM_ID = octets.pop_int();
			STEEL_ITEM_ID = octets.pop_int();
			LADDER_TASK_BTN_NAME = octets.pop_string();
			TRUSTEESHIP_HP_DEFAULT = octets.pop_int();
			TRUSTEESHIP_HUNGER_DEFAULT = octets.pop_int();
			TEAM_EARNINGREWARD_MAX_GETCOUNT = octets.pop_int();
			TEAM_EARNINGR_DROP_EWARDID = octets.pop_int();
			TEAM_EARNING_MAX_VALUE = octets.pop_int();
			REFRESH_RECOMMENDFRIENDS_CD = octets.pop_int();
			MAX_RECOMMENDFRIENDS_NUM = octets.pop_int();
			MANOR_CHEST_ITEM_ID = octets.pop_int();
			LOGIN_SDK_TIME = octets.pop_float();
		}
	}
}
