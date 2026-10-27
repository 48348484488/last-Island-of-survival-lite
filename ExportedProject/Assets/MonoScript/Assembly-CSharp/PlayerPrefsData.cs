using UnityEngine;

public class PlayerPrefsData
{
	public enum OpenCondition
	{
		Voice_open_all = 1,
		Voice_close = 2,
		Voice_open_only_wifi = 3,
		Photo_open_all = 4,
		Photo_close = 5,
		Photo_open_only_wifi = 6
	}

	public enum RecvChatMessageCondition
	{
		RecvChatMsg_all = 1,
		RecvChatMsg_only_eachother = 2
	}

	public enum NearbyRoleFilterSex
	{
		sex_unlimited = -1,
		sex_male = 1,
		sex_female = 2
	}

	public enum NearbyRoleFilterAge
	{
		age_unlimited = -1,
		age_less_20 = 0,
		age_20_30 = 1,
		age_30_40 = 2
	}

	public enum NearbyRoleFilterAffection
	{
		affection_unlimited = -1,
		affection_secret = 1,
		affection_single = 2,
		affection_find_friends = 3
	}

	public enum NearbyRoleFilterOnTime
	{
		ontime_unlimited = -1,
		ontime_30 = 0,
		ontime_1_hour = 1,
		ontime_2_days = 2
	}

	private const string m_strMusicKey = "set_music";

	private const string m_strSoundEffectKey = "set_soundeffect";

	private const string m_strVoiceKey = "set_voice";

	private const string m_strPhotoKey = "set_photo";

	private const string m_strChatKey = "set_chat";

	private const string m_strBoxKey = "set_box";

	private const string m_strNearbyFilterSexKey = "nearbyfilter_sex";

	private const string m_strNearbyFilterAgeKey = "nearbyfilter_age";

	private const string m_strNearbyFilterAffectionKey = "nearbyfilter_affection";

	private const string m_strNearbyFilterOntimeKey = "nearbyfilter_ontime";

	private const string IsTeachedKey = "isTeached";

	public static string Account
	{
		get
		{
			if (PlayerPrefs.HasKey("Account"))
			{
				return PlayerPrefs.GetString("Account");
			}
			return string.Empty;
		}
		set
		{
			PlayerPrefs.SetString("Account", value);
			PlayerPrefs.Save();
		}
	}

	public static string SessionKey
	{
		get
		{
			if (PlayerPrefs.HasKey("SessionKey"))
			{
				return PlayerPrefs.GetString("SessionKey");
			}
			return string.Empty;
		}
		set
		{
			PlayerPrefs.SetString("SessionKey", value);
		}
	}

	public static string Name
	{
		get
		{
			if (PlayerPrefs.HasKey("AccountName"))
			{
				return PlayerPrefs.GetString("AccountName");
			}
			return string.Empty;
		}
		set
		{
			PlayerPrefs.SetString("AccountName", value);
		}
	}

	public static int ServerId
	{
		get
		{
			if (PlayerPrefs.HasKey("ServerId"))
			{
				return PlayerPrefs.GetInt("ServerId");
			}
			return 0;
		}
		set
		{
			PlayerPrefs.SetInt("ServerId", value);
			PlayerPrefs.Save();
		}
	}

	public static bool IsHasServerId
	{
		get
		{
			return PlayerPrefs.HasKey("ServerId");
		}
	}

	public static OpenCondition VoiceOpencondition
	{
		get
		{
			if (PlayerPrefs.HasKey("set_voice"))
			{
				return (OpenCondition)PlayerPrefs.GetInt("set_voice");
			}
			return OpenCondition.Voice_open_all;
		}
		set
		{
			PlayerPrefs.SetInt("set_voice", (int)value);
			PlayerPrefs.Save();
		}
	}

	public static OpenCondition PhotoOpencondition
	{
		get
		{
			if (PlayerPrefs.HasKey("set_photo"))
			{
				return (OpenCondition)PlayerPrefs.GetInt("set_photo");
			}
			return OpenCondition.Photo_open_all;
		}
		set
		{
			PlayerPrefs.SetInt("set_photo", (int)value);
			PlayerPrefs.Save();
		}
	}

	public static bool bMusic
	{
		get
		{
			if (!PlayerPrefs.HasKey("set_music"))
			{
				return true;
			}
			return PlayerPrefs.GetInt("set_music", 0) != 0;
		}
		set
		{
			PlayerPrefs.SetInt("set_music", value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static bool bSoundEffect
	{
		get
		{
			if (!PlayerPrefs.HasKey("set_soundeffect"))
			{
				return true;
			}
			return PlayerPrefs.GetInt("set_soundeffect", 0) != 0;
		}
		set
		{
			PlayerPrefs.SetInt("set_soundeffect", value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static bool NeedOpenNewPlayerBoot
	{
		get
		{
			if (!PlayerPrefs.HasKey("newPlayerBoot"))
			{
				return true;
			}
			return PlayerPrefs.GetInt("newPlayerBoot", 0) != 0;
		}
		set
		{
			PlayerPrefs.SetInt("newPlayerBoot", value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static bool NeedOpenGamePlayDes
	{
		get
		{
			if (!PlayerPrefs.HasKey("gamePlayDes"))
			{
				return true;
			}
			return PlayerPrefs.GetInt("gamePlayDes", 0) != 0;
		}
		set
		{
			PlayerPrefs.SetInt("gamePlayDes", value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static string ChatStick
	{
		get
		{
			if (!PlayerPrefs.HasKey("chatStick"))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString("chatStick");
		}
		set
		{
			PlayerPrefs.SetString("chatStick", value);
		}
	}

	public static RecvChatMessageCondition recvChatMsgCondition
	{
		get
		{
			if (!PlayerPrefs.HasKey("set_chat"))
			{
				return RecvChatMessageCondition.RecvChatMsg_all;
			}
			return (RecvChatMessageCondition)PlayerPrefs.GetInt("set_chat");
		}
		set
		{
			PlayerPrefs.SetInt("set_chat", (int)value);
			PlayerPrefs.Save();
		}
	}

	public static bool bSendMsgWhenBoxOpen
	{
		get
		{
			if (!PlayerPrefs.HasKey("set_box"))
			{
				return true;
			}
			return PlayerPrefs.GetInt("set_box", 0) != 0;
		}
		set
		{
			PlayerPrefs.SetInt("set_box", value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static NearbyRoleFilterSex nNearbyFilterSex
	{
		get
		{
			if (!PlayerPrefs.HasKey("nearbyfilter_sex"))
			{
				return NearbyRoleFilterSex.sex_unlimited;
			}
			return (NearbyRoleFilterSex)PlayerPrefs.GetInt("nearbyfilter_sex", -1);
		}
		set
		{
			PlayerPrefs.SetInt("nearbyfilter_sex", (int)value);
		}
	}

	public static NearbyRoleFilterAge nNearbyFilterAge
	{
		get
		{
			if (!PlayerPrefs.HasKey("nearbyfilter_age"))
			{
				return NearbyRoleFilterAge.age_unlimited;
			}
			return (NearbyRoleFilterAge)PlayerPrefs.GetInt("nearbyfilter_age", -1);
		}
		set
		{
			PlayerPrefs.SetInt("nearbyfilter_age", (int)value);
		}
	}

	public static NearbyRoleFilterAffection nNearbyFilterAffection
	{
		get
		{
			if (!PlayerPrefs.HasKey("nearbyfilter_affection"))
			{
				return NearbyRoleFilterAffection.affection_unlimited;
			}
			return (NearbyRoleFilterAffection)PlayerPrefs.GetInt("nearbyfilter_affection", -1);
		}
		set
		{
			PlayerPrefs.SetInt("nearbyfilter_affection", (int)value);
		}
	}

	public static NearbyRoleFilterOnTime nNearbyFilterOntime
	{
		get
		{
			if (!PlayerPrefs.HasKey("nearbyfilter_ontime"))
			{
				return NearbyRoleFilterOnTime.ontime_unlimited;
			}
			return (NearbyRoleFilterOnTime)PlayerPrefs.GetInt("nearbyfilter_ontime", -1);
		}
		set
		{
			PlayerPrefs.SetInt("nearbyfilter_ontime", (int)value);
		}
	}

	public static string RobotBattleLand
	{
		get
		{
			if (PlayerPrefs.HasKey("RobotBattleLand"))
			{
				return PlayerPrefs.GetString("RobotBattleLand");
			}
			return string.Empty;
		}
		set
		{
			PlayerPrefs.SetString("RobotBattleLand", value);
			PlayerPrefs.Save();
		}
	}

	public static bool IsTeached
	{
		get
		{
			if (!PlayerPrefs.HasKey("isTeached"))
			{
				return false;
			}
			return PlayerPrefs.GetInt("isTeached", 0) != 0;
		}
		set
		{
			PlayerPrefs.SetInt("isTeached", value ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static void Save()
	{
		PlayerPrefs.Save();
	}

	public static bool SetCanUsePhoto()
	{
		if (PhotoOpencondition == OpenCondition.Photo_open_all)
		{
			return true;
		}
		if (PhotoOpencondition == OpenCondition.Photo_close)
		{
			return false;
		}
		if (PhotoOpencondition == OpenCondition.Photo_open_only_wifi)
		{
			if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
