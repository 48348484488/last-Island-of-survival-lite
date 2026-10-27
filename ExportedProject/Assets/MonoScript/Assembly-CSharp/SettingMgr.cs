using System;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using gs.online.scmsg;

public class SettingMgr : Singleton<SettingMgr>
{
	public enum TrusteeshipPickType
	{
		All = 0,
		PeiFang = 1,
		CaiLiao = 2,
		WuQi = 3,
		ZhuangBei = 4,
		DanYao = 5,
		QiangXiePeiJian = 6,
		SheShi = 7,
		YaoPin = 8,
		QiTa = 9
	}

	public enum TrusteeshipCollectType
	{
		Mine = 1,
		Plant = 2,
		Trashcan = 3,
		Tree = 8
	}

	public enum OperateModeEnum
	{
		Simple = 0,
		Complex = 1
	}

	public enum DriveModeEnum
	{
		None = 0,
		Btn = 1,
		Joystack = 2
	}

	private const string MusicKey = "musicValume";

	private const string SoundEffectKey = "soundValume";

	private const string LeftShootModeKey = "LeftShootMode";

	public const int LeftShootModeOpen = 0;

	public const int LeftShootModeByAim = 1;

	public const int LeftShootModeClose = 2;

	private const string ZuDuiYaoQing = "ZuDuiYaoQing";

	private const string FrameRateQualityKey = "FrameRateQuality";

	public const int FrameRateQualityLow = 0;

	public const int FrameRateQualityMid = 1;

	public const int FrameRateQualityHigh = 2;

	private static int DefaultFrameRateQuality = 2;

	private const string ResolutionKey = "ResolutionKey";

	public const int Resolution1 = 0;

	public const int Resolution2 = 1;

	public const int Resolution3 = 2;

	private static int DefaultResolutionKey = 2;

	private const string RenderQualityKey = "RenderQuality";

	public const int RenderQuality1 = 1;

	public const int RenderQuality2 = 2;

	public const int RenderQuality3 = 3;

	public const int RenderQuality4 = 4;

	public const int RenderQuality5 = 5;

	private static int DefaultRenderQualityQuality = 1;

	private const string PhotoQualityKey = "QualityLevel";

	public const int QualityLevelLow = 0;

	public const int QualityLevelMedium = 1;

	public const int QualityLevelHigh = 2;

	public const int QualityLevelSelf = 3;

	private static int DefaultQualityLevel;

	private const string ShadowQualityKey = "ShadowQuality";

	public const int ShadowQuality1 = 1;

	public const int ShadowQuality2 = 2;

	public const int ShadowQuality3 = 3;

	public const int ShadowQuality4 = 4;

	public const int ShadowQuality5 = 5;

	private static int DefaultShadowQuality = 1;

	private const string SensitivityHorizontalKey = "SensitivityHorizontal";

	private const string SensitivityVerticalKey = "SensitivityVertical";

	private const string SensitivityAimRedKey = "SensitivityAimRed";

	private const string SensitivityAimHoloKey = "SensitivityAimHolo";

	private const string SensitivityAim2Key = "SensitivityAim2";

	private const string SensitivityAim4Key = "SensitivityAim4";

	private const string SensitivityAim8Key = "SensitivityAim8";

	private const string ChangeGunModeKey = "ChangeGunMode";

	public const int ChangeGunModeMode1 = 1;

	public const int ChangeGunModeMode2 = 2;

	private const string OperationModeKey = "OperationMode";

	public const int OperationMode1 = 1;

	public const int OperationMode2 = 2;

	private const string OperationMode1OpenLookBtnKey = "OperationMode1OpenLookBtn";

	private const string OperationMode2OpenLookBtnKey = "OperationMode2OpenLookBtn";

	private const string OpenLookBtnKey = "OpenLookBtn";

	private const string FireBtnFixKey = "FireBtnFix";

	private const string HelpAimKey = "HelpAim";

	private const string AutoAimKey = "AutoAim";

	private const string AutoOpenDoorKey = "AutoOpenDoor";

	private const string DriveModeKey = "DriveMode";

	public const int DriveModeButton = 0;

	public const int DriveModejoystick = 1;

	private const string OpenCameraAIKey = "OpenCameraAI";

	private const string TrusteeshipHpStr = "TrusteeshipHpDefault";

	private const string TrusteeshipHungerStr = "TrusteeshipEatDefault";

	private const string LoadTrusteeshipDefaultValue = "LoadTrusteeshipDefaultValue";

	private const string TrusteeshipCollectStartStr = "TrusteeshipCollectSetting";

	private const string TrusteeshipPickStartStr = "TrusteeshipPickSetting";

	public const string BattleCustomSettingPath = "newcustomsetting.oc";

	private const string IsResertBattleCustomKey = "ResertBattleCustom";

	private const string FirstOpenMapKey = "FirstOpenMap";

	public static float MusicValume
	{
		get
		{
			return GetFloat("musicValume");
		}
		set
		{
			SetFloat("musicValume", value, SettingEvent.MusicDelegate);
			SingletonMono<AudioManager>.Ins.BGVolume = value;
		}
	}

	public static float SoundEffectValume
	{
		get
		{
			return GetFloat("soundValume");
		}
		set
		{
			SetFloat("soundValume", value, SettingEvent.SoundEffectDelegate);
			SingletonMono<AudioManager>.Ins.EffectVolume = value;
		}
	}

	public static int LeftShootMode
	{
		get
		{
			return GetInt("LeftShootMode");
		}
		set
		{
			SetInt("LeftShootMode", value, SettingEvent.LeftShootDelegate);
		}
	}

	public static bool ZuDui
	{
		get
		{
			return GetBool("ZuDuiYaoQing");
		}
		set
		{
			SetBool("ZuDuiYaoQing", value, null);
		}
	}

	public static int FrameRateQuality
	{
		get
		{
			return GetInt("FrameRateQuality", DefaultFrameRateQuality);
		}
		set
		{
			SetInt("FrameRateQuality", value, SettingEvent.FrameRateQualityDelegate);
		}
	}

	public static int ResolutionLevel
	{
		get
		{
			return GetInt("ResolutionKey", DefaultResolutionKey);
		}
		set
		{
			SetInt("ResolutionKey", value, null);
		}
	}

	public static int RenderQuality
	{
		get
		{
			return GetInt("RenderQuality", DefaultRenderQualityQuality);
		}
		set
		{
			SetInt("RenderQuality", value, SettingEvent.RenderQualityDelegate);
		}
	}

	public static int QualityLevel
	{
		get
		{
			return GetInt("QualityLevel", DefaultQualityLevel);
		}
		set
		{
			SetInt("QualityLevel", value, SettingEvent.PhotoQualityDelegate);
		}
	}

	public static int ShadowQuality
	{
		get
		{
			return GetInt("ShadowQuality", DefaultShadowQuality);
		}
		set
		{
			SetInt("ShadowQuality", value, SettingEvent.ShadowQualityDelegate);
		}
	}

	public static float SensitivityHorizontal
	{
		get
		{
			return GetFloat("SensitivityHorizontal", 0.4f);
		}
		set
		{
			SetFloat("SensitivityHorizontal", value, SettingEvent.SensitivityHorizontalDelegate);
		}
	}

	public static float SensitivityVertical
	{
		get
		{
			return GetFloat("SensitivityVertical", 0.4f);
		}
		set
		{
			SetFloat("SensitivityVertical", value, SettingEvent.SensitivityVerticalDelegate);
		}
	}

	public static float SensitivityAimRed
	{
		get
		{
			return GetFloat("SensitivityAimRed", 0.4f);
		}
		set
		{
			SetFloat("SensitivityAimRed", value, SettingEvent.SensitivityAimRedDelegate);
		}
	}

	public static float SensitivityAimHolo
	{
		get
		{
			return GetFloat("SensitivityAimHolo", 0.4f);
		}
		set
		{
			SetFloat("SensitivityAimHolo", value, SettingEvent.SensitivityAimHoloDelegate);
		}
	}

	public static float SensitivityAim2
	{
		get
		{
			return GetFloat("SensitivityAim2", 0.4f);
		}
		set
		{
			SetFloat("SensitivityAim2", value, SettingEvent.SensitivityAim2Delegate);
		}
	}

	public static float SensitivityAim4
	{
		get
		{
			return GetFloat("SensitivityAim4", 0.4f);
		}
		set
		{
			SetFloat("SensitivityAim4", value, SettingEvent.SensitivityAim4Delegate);
		}
	}

	public static float SensitivityAim8
	{
		get
		{
			return GetFloat("SensitivityAim8", 0.4f);
		}
		set
		{
			SetFloat("SensitivityAim8", value, SettingEvent.SensitivityAim8Delegate);
		}
	}

	public static int ChangeGunMode
	{
		get
		{
			return GetInt("ChangeGunMode", 2);
		}
		set
		{
			SetInt("ChangeGunMode", value, SettingEvent.ChangeGunModeDelegate);
		}
	}

	public static int OperationMode
	{
		get
		{
			return GetInt("OperationMode", 2);
		}
		set
		{
			SetInt("OperationMode", value, SettingEvent.OperationDelegate);
		}
	}

	public static bool OperationMode1OpenLookBtn
	{
		get
		{
			return GetBool("OperationMode1OpenLookBtn");
		}
		set
		{
			SetBool("OperationMode1OpenLookBtn", value, SettingEvent.OpenLookBtnDelegate);
		}
	}

	public static bool OperationMode2OpenLookBtn
	{
		get
		{
			return GetBool("OperationMode2OpenLookBtn");
		}
		set
		{
			SetBool("OperationMode2OpenLookBtn", value, SettingEvent.OpenLookBtnDelegate);
		}
	}

	public static bool OpenLookBtn
	{
		get
		{
			if (OperationMode == 1)
			{
				return OperationMode1OpenLookBtn;
			}
			return OperationMode2OpenLookBtn;
		}
		set
		{
			if (OperationMode == 1)
			{
				OperationMode1OpenLookBtn = value;
			}
			else
			{
				OperationMode2OpenLookBtn = value;
			}
		}
	}

	public static bool FireBtnFix
	{
		get
		{
			return GetBool("FireBtnFix");
		}
		set
		{
			SetBool("FireBtnFix", value, SettingEvent.IsFixFireBtnDelegate);
		}
	}

	public static bool HelpAim
	{
		get
		{
			return GetBool("HelpAim");
		}
		set
		{
			SetBool("HelpAim", value, SettingEvent.IsHelpAimDelegate);
		}
	}

	public static bool AutoAim
	{
		get
		{
			return GetBool("AutoAim");
		}
		set
		{
			SetBool("AutoAim", value, SettingEvent.IsAutoAimDelegate);
		}
	}

	public static bool AutoOpenDoor
	{
		get
		{
			return GetBool("AutoOpenDoor");
		}
		set
		{
			SetBool("AutoOpenDoor", value, SettingEvent.IsAutoOpenDoorDelegate);
		}
	}

	public static int DriveMode
	{
		get
		{
			return GetInt("DriveMode");
		}
		set
		{
			SetInt("DriveMode", value, SettingEvent.DriveDelegate);
		}
	}

	public static bool OpenCameraAI
	{
		get
		{
			return GetBool("OpenCameraAI");
		}
		set
		{
			SetBool("OpenCameraAI", value, SettingEvent.OnCameraAI);
		}
	}

	public bool IsPickupNearWeapon { get; internal set; }

	public bool IsPickupGun { get; internal set; }

	public bool IsPickupBullet { get; internal set; }

	public static bool FirstOpenMap
	{
		get
		{
			return GetBool("FirstOpenMap");
		}
		set
		{
			SetBool("FirstOpenMap", value, null);
		}
	}

	public void Init()
	{
		SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Combine(SLoginFinished.handler, new SLoginFinished.Handler(OnSLoginFinished));
	}

	public void SetDefault()
	{
		if (!SystemInfo.supportsInstancing || Utils.IsLowEndProduct())
		{
			DefaultQualityLevel = 0;
		}
		else if (SystemInfo.systemMemorySize <= 6144)
		{
			DefaultQualityLevel = 1;
		}
		else
		{
			DefaultQualityLevel = 2;
		}
		PerformanceUtils.SetQualityLevel();
	}

	private void OnSLoginFinished(SLoginFinished msg)
	{
		if (!GetBool("LoadTrusteeshipDefaultValue", false))
		{
			SetBool("LoadTrusteeshipDefaultValue", true, null);
			InitTrusteeshipValue();
		}
	}

	public static void SetFrameRate(int frameRateQuality)
	{
		Application.targetFrameRate = ((frameRateQuality != 2) ? 30 : 60);
	}

	public static void DelBasicKeys()
	{
		PlayerPrefs.DeleteKey("musicValume");
		PlayerPrefs.DeleteKey("soundValume");
		PlayerPrefs.DeleteKey("LeftShootMode");
		PlayerPrefs.DeleteKey("FrameRateQuality");
		PlayerPrefs.DeleteKey("RenderQuality");
		PlayerPrefs.DeleteKey("SensitivityHorizontal");
		PlayerPrefs.DeleteKey("SensitivityVertical");
		PlayerPrefs.DeleteKey("SensitivityAimRed");
		PlayerPrefs.DeleteKey("SensitivityAimHolo");
		PlayerPrefs.DeleteKey("SensitivityAim2");
		PlayerPrefs.DeleteKey("SensitivityAim4");
		PlayerPrefs.DeleteKey("SensitivityAim8");
	}

	public static void DelOperateKeys()
	{
		PlayerPrefs.DeleteKey("HelpAim");
		PlayerPrefs.DeleteKey("FireBtnFix");
		PlayerPrefs.DeleteKey("OpenLookBtn");
		PlayerPrefs.DeleteKey("OperationMode2OpenLookBtn");
		PlayerPrefs.DeleteKey("OperationMode1OpenLookBtn");
		PlayerPrefs.DeleteKey("OperationMode");
		PlayerPrefs.DeleteKey("ChangeGunMode");
		PlayerPrefs.DeleteKey("AutoAim");
		PlayerPrefs.DeleteKey("AutoOpenDoor");
	}

	public static void DelDriveKeys()
	{
		PlayerPrefs.DeleteKey("DriveMode");
		PlayerPrefs.DeleteKey("OpenCameraAI");
	}

	public static void SetTrusteeshipCollectSetByIndex(TrusteeshipCollectType type, int indexInCfg, bool isOn)
	{
		SetBool(GetCollectKey((int)type, indexInCfg), isOn, SingletonMono<TrusteeshipMgr>.Ins.OnCacheDataChange);
	}

	public static bool GetTrusteeshipCollectSetByIndex(TrusteeshipCollectType type, int indexInCfg)
	{
		return GetBool(GetCollectKey((int)type, indexInCfg));
	}

	public static bool GetTrusteeshipCollectSetById(TrusteeshipCollectType type, int selfCfgId)
	{
		if (type == TrusteeshipCollectType.Tree)
		{
			return GetBool(GetCollectKey((int)type, 0));
		}
		int num = -1;
		TrusteeshipCollectCfg trusteeshipCollectCfg = TrusteeshipCollectCfg.Get((int)type);
		List<TrusteeshipCollectItem> items = trusteeshipCollectCfg.items;
		int i = 0;
		for (int count = items.Count; i < count; i++)
		{
			if (items[i].selfCfgId == selfCfgId)
			{
				num = i;
				break;
			}
		}
		if (num > -1)
		{
			return GetBool(GetCollectKey((int)type, num));
		}
		return true;
	}

	private static string GetCollectKey(int collectCfgId, int index)
	{
		return "TrusteeshipCollectSetting" + collectCfgId + index;
	}

	public static bool GetTrusteeshipHeal()
	{
		return GetBoolTrusteeship("TrusteeshipHpDefault");
	}

	public static void SetTrusteeshipHeal(bool isOn)
	{
		SetBoolTrusteeship("TrusteeshipHpDefault", isOn, SingletonMono<TrusteeshipMgr>.Ins.OnCacheDataChange);
	}

	public static int GetTrusteeshipHealValue()
	{
		return (int)Math.Abs(GetFloat("TrusteeshipHpDefault"));
	}

	public static void SetTrusteeshipHealValue(int value)
	{
		SetFloat("TrusteeshipHpDefault", (value != 0) ? ((float)value) : ((!GetTrusteeshipHeal()) ? (-0.1f) : 0.1f), SingletonMono<TrusteeshipMgr>.Ins.OnCacheDataChange);
	}

	public static bool GetTrusteeshipEat()
	{
		return GetBoolTrusteeship("TrusteeshipEatDefault");
	}

	public static void SetTrusteeshipEat(bool isOn)
	{
		SetBoolTrusteeship("TrusteeshipEatDefault", isOn, SingletonMono<TrusteeshipMgr>.Ins.OnCacheDataChange);
	}

	public static int GetTrusteeshipEatValue()
	{
		return (int)Math.Abs(GetFloat("TrusteeshipEatDefault"));
	}

	public static void SetTrusteeshipEatValue(int value)
	{
		SetFloat("TrusteeshipEatDefault", (value != 0) ? ((float)value) : ((!GetTrusteeshipEat()) ? (-0.1f) : 0.1f), SingletonMono<TrusteeshipMgr>.Ins.OnCacheDataChange);
	}

	private static void SetBoolTrusteeship(string key, bool value, Action<bool> act)
	{
		if (PlayerPrefs.GetFloat(key, 0.1f) > 0f != value)
		{
			PlayerPrefs.SetFloat(key, PlayerPrefs.GetFloat(key, 0.1f) * -1f);
		}
		if (act != null)
		{
			act(value);
		}
	}

	private static bool GetBoolTrusteeship(string key, bool defaultValue = true)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return defaultValue;
		}
		return PlayerPrefs.GetFloat(key) > 0f;
	}

	public static bool GetTrusteeshipPick(int itemType)
	{
		return GetBool(GetPickKey(GetPickTypeByItemType(itemType)));
	}

	public static bool GetTrusteeshipPick(TrusteeshipPickType type)
	{
		return GetBool(GetPickKey(type));
	}

	public static void SetTrusteeshipPick(TrusteeshipPickType type, bool isOn)
	{
		SetBool(GetPickKey(type), isOn, SingletonMono<TrusteeshipMgr>.Ins.OnCacheDataChange);
	}

	private static TrusteeshipPickType GetPickTypeByItemType(int itemType)
	{
		switch (itemType)
		{
		case 32:
			return TrusteeshipPickType.YaoPin;
		case 79:
			return TrusteeshipPickType.PeiFang;
		case 107:
			return TrusteeshipPickType.CaiLiao;
		case 13:
		case 17:
		case 25:
			return TrusteeshipPickType.WuQi;
		case 119:
			return TrusteeshipPickType.ZhuangBei;
		case 24:
			return TrusteeshipPickType.DanYao;
		case 18:
		case 19:
		case 20:
		case 21:
			return TrusteeshipPickType.QiangXiePeiJian;
		case 78:
			return TrusteeshipPickType.SheShi;
		default:
			return TrusteeshipPickType.QiTa;
		}
	}

	private static string GetPickKey(TrusteeshipPickType type)
	{
		return "TrusteeshipPickSetting" + type;
	}

	private void InitTrusteeshipValue()
	{
		SetBoolTrusteeship("TrusteeshipHpDefault", true, null);
		SetBoolTrusteeship("TrusteeshipEatDefault", true, null);
		SetFloat("TrusteeshipHpDefault", cfg.Consts.TRUSTEESHIP_HP_DEFAULT, null);
		SetFloat("TrusteeshipEatDefault", cfg.Consts.TRUSTEESHIP_HUNGER_DEFAULT, null);
		List<TrusteeshipCollectCfg> allList = TrusteeshipCollectCfg.GetAllList();
		int i = 0;
		for (int count = allList.Count; i < count; i++)
		{
			TrusteeshipCollectCfg trusteeshipCollectCfg = allList[i];
			List<TrusteeshipCollectItem> items = trusteeshipCollectCfg.items;
			int j = 0;
			for (int count2 = items.Count; j < count2; j++)
			{
				TrusteeshipCollectItem trusteeshipCollectItem = items[j];
				SetBool(GetCollectKey(trusteeshipCollectCfg.id, j), trusteeshipCollectItem.defaultValue, null);
			}
		}
		string[] names = Enum.GetNames(typeof(TrusteeshipPickType));
		int k = 0;
		for (int num = names.Length; k < num; k++)
		{
			SetBool(GetPickKey((TrusteeshipPickType)k), true, null);
		}
	}

	public static int GetInt(string key, int defaultValue = 0)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return defaultValue;
		}
		return PlayerPrefs.GetInt(key);
	}

	public static void SetInt(string key, int value, Utils.IntDelegate callBack)
	{
		PlayerPrefs.SetInt(key, value);
		PlayerPrefs.Save();
		Utils.TriggerEvent(callBack, value);
	}

	public static void SetBool(string key, bool value, Utils.BoolDelegate callBack)
	{
		PlayerPrefs.SetInt(key, value ? 1 : 0);
		PlayerPrefs.Save();
		Utils.TriggerEvent(callBack, value);
	}

	public static bool GetBool(string key, bool defaultValue = true)
	{
		return GetInt(key, defaultValue ? 1 : 0) == 1;
	}

	public static float GetFloat(string key, float defaultValue = 1f)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return defaultValue;
		}
		return PlayerPrefs.GetFloat(key);
	}

	public static void SetFloat(string key, float value, Utils.FloatDelegate callBack)
	{
		PlayerPrefs.SetFloat(key, value);
		PlayerPrefs.Save();
		Utils.TriggerEvent(callBack, value);
	}

	public static bool IsResertBattleCustom()
	{
		if (PlayerPrefs.HasKey("ResertBattleCustom"))
		{
			return false;
		}
		PlayerPrefs.SetInt("ResertBattleCustom", 1);
		return true;
	}

	public static void DelKeys()
	{
		DelBasicKeys();
		DelDriveKeys();
		DelOperateKeys();
		PlayerPrefs.Save();
		Utils.TriggerEvent(SettingEvent.ResertSettingDelegate);
	}

	internal bool IsPickupEquips(int type)
	{
		throw new NotImplementedException();
	}

	internal bool IsPickupMedicine(int itemId)
	{
		throw new NotImplementedException();
	}

	internal bool IsPickupGunByType(int gunType)
	{
		throw new NotImplementedException();
	}

	internal bool IsPickupAccessoryByType(int type)
	{
		throw new NotImplementedException();
	}

	internal bool IsPickupThorwWeapon(int itemId)
	{
		throw new NotImplementedException();
	}
}
