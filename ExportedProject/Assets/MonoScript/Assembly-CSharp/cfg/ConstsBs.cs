using Share;

namespace cfg
{
	public sealed class ConstsBs
	{
		public const string Path = "cfg.ConstsBs.oc";

		public static float ToukuiDamage = 1.5f;

		public static float ToukuiReduceDamage = 20f;

		public static float FangdanyiDamage = 1.5f;

		public static float FangdanyiReduceDamage = 70f;

		public static float VehicleDamage = 1.5f;

		public static float VehicleDamage1 = 0.35f;

		public static float VehicleDamage2 = 1f;

		public static float VehicleReduceDamage = 19f;

		public static float VehicleReduceDamage1 = 20f;

		public static float VehicleReduceDamage2 = 2f;

		public static float BulletHeadDamageFactor = 3f;

		public static float BulletBodyDamageFactor = 1f;

		public static float BulletArmDamageFactor = 0.8f;

		public static float BulletLegDamageFactor = 0.8f;

		public static float BulletLowArmDamageFactor = 0.75f;

		public static float BulletLowLegDamageFactor = 0.75f;

		public static int HpPlayer = 15000;

		public static int EpPlayer = 10000;

		public static int SAVE_PLAYER_NEED_TIME = 10;

		public static int MAP_SIZE = 7000;

		public static int VEHICLE_EXPLOSION_DAMAGE_TO_PLAYER = 50000;

		public static int VEHICLE_EXPLOSION_DAMAGE_TO_VEHICLE = 50000;

		public static int BODY_ID_HEAD = 9;

		public static int CONTINUE_KILL_TIME_PEROID = 60;

		public static int PLAYER_DIE_BOX_TIME;

		public static int PLAYER_DIE_RECEIVE_MSG_TIME = 5;

		public static int HpDoor = 15000;

		public static bool OpenVoiceRoom = true;

		public static float VoiceRoomRadius = 50f;

		public static int VoiceRoomSendMax = 6;

		public static float VoiceTongue;

		public static float MINVoiceTongue = -12f;

		public static float MAXVoiceTongue = 12f;

		public static int QuickInstructionsShowTime = 10;

		public static int HpEquip = 10000;

		public static float CARTURNSPEED = 5f;

		public static int FLAG_ITEM_ID = 32017;

		public static int KillScore = 3;

		public static float AUTO_PICK_INTERVAL_TIME = 0.5f;

		public static int AUTO_PICK_CLOSE_TIME = 20;

		public static int DEAD_BOX_ITEM_ID = 30000;

		public static float TIME_DOOR_ACTION = 0.65f;

		public static int MAX_HUNGER = 500;

		public static int DEFAULT_HUNGER = 100;

		public static int CONSUME_HUNGER_SPEED = 150;

		public static int CONSUME_HP_SPEED_WHEN_NO_HUNGER = 15;

		public static int MAX_STRENGTH = 100;

		public static int STRENGTH_CONSUME_SPEED = 10;

		public static int CUT_TREE_CONSUME_HUNGER = 1;

		public static int MINE_CONSUME_HUNGER = 1;

		public static int SHOOT_CONSUME_HUNGER = 1;

		public static int CUT_PLANT_CONSUME_HUNGER = 1;

		public static int PASSWORD_WRONG_HP = 10;

		public static int PASSWORD_WRONG_NUM = 3;

		public static int PASSWORD_WRONG_COOL_SECONDS = 3600;

		public static int AUTO_REBIRTH_TIME = 60;

		public static int MAX_CONTROLL_MONSTER_NUMBER = 20;

		public static void Load(byte[] bytes)
		{
			Octets octets = new Octets(bytes, bytes.Length);
			ToukuiDamage = octets.pop_float();
			ToukuiReduceDamage = octets.pop_float();
			FangdanyiDamage = octets.pop_float();
			FangdanyiReduceDamage = octets.pop_float();
			VehicleDamage = octets.pop_float();
			VehicleDamage1 = octets.pop_float();
			VehicleDamage2 = octets.pop_float();
			VehicleReduceDamage = octets.pop_float();
			VehicleReduceDamage1 = octets.pop_float();
			VehicleReduceDamage2 = octets.pop_float();
			BulletHeadDamageFactor = octets.pop_float();
			BulletBodyDamageFactor = octets.pop_float();
			BulletArmDamageFactor = octets.pop_float();
			BulletLegDamageFactor = octets.pop_float();
			BulletLowArmDamageFactor = octets.pop_float();
			BulletLowLegDamageFactor = octets.pop_float();
			HpPlayer = octets.pop_int();
			EpPlayer = octets.pop_int();
			SAVE_PLAYER_NEED_TIME = octets.pop_int();
			MAP_SIZE = octets.pop_int();
			VEHICLE_EXPLOSION_DAMAGE_TO_PLAYER = octets.pop_int();
			VEHICLE_EXPLOSION_DAMAGE_TO_VEHICLE = octets.pop_int();
			BODY_ID_HEAD = octets.pop_int();
			CONTINUE_KILL_TIME_PEROID = octets.pop_int();
			PLAYER_DIE_BOX_TIME = octets.pop_int();
			PLAYER_DIE_RECEIVE_MSG_TIME = octets.pop_int();
			HpDoor = octets.pop_int();
			OpenVoiceRoom = octets.pop_boolean();
			VoiceRoomRadius = octets.pop_float();
			VoiceRoomSendMax = octets.pop_int();
			VoiceTongue = octets.pop_float();
			MINVoiceTongue = octets.pop_float();
			MAXVoiceTongue = octets.pop_float();
			QuickInstructionsShowTime = octets.pop_int();
			HpEquip = octets.pop_int();
			CARTURNSPEED = octets.pop_float();
			FLAG_ITEM_ID = octets.pop_int();
			KillScore = octets.pop_int();
			AUTO_PICK_INTERVAL_TIME = octets.pop_float();
			AUTO_PICK_CLOSE_TIME = octets.pop_int();
			DEAD_BOX_ITEM_ID = octets.pop_int();
			TIME_DOOR_ACTION = octets.pop_float();
			MAX_HUNGER = octets.pop_int();
			DEFAULT_HUNGER = octets.pop_int();
			CONSUME_HUNGER_SPEED = octets.pop_int();
			CONSUME_HP_SPEED_WHEN_NO_HUNGER = octets.pop_int();
			MAX_STRENGTH = octets.pop_int();
			STRENGTH_CONSUME_SPEED = octets.pop_int();
			CUT_TREE_CONSUME_HUNGER = octets.pop_int();
			MINE_CONSUME_HUNGER = octets.pop_int();
			SHOOT_CONSUME_HUNGER = octets.pop_int();
			CUT_PLANT_CONSUME_HUNGER = octets.pop_int();
			PASSWORD_WRONG_HP = octets.pop_int();
			PASSWORD_WRONG_NUM = octets.pop_int();
			PASSWORD_WRONG_COOL_SECONDS = octets.pop_int();
			AUTO_REBIRTH_TIME = octets.pop_int();
			MAX_CONTROLL_MONSTER_NUMBER = octets.pop_int();
		}
	}
}
