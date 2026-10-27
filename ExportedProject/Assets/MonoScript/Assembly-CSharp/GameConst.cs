using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameConst
{
	public static string[] PreloadAbs = new string[0];

	public const string VIEW_AB_PATH = "ui/";

	public const string AUDIO_AB_PATH = "audio/";

	public const string ServerTxtPath = "gameconfig/server.txt";

	public const string OtherPlayerColliderTag = "OtherPlayerCollider";

	public const string SelfColliderTag = "SelfCollider";

	public const string CarColliderTag = "Car";

	public const string WeiqiangTag = "Weiqiang";

	public const string ResRootPath = "res/";

	public const float CanRushInputValue = 0.917f;

	public const string FeedbackPath = "";

	public const string BasicRole = "role/role.ab";

	public const bool IsOpenActive = true;

	public static bool IsShowCountry = true;

	public const string LocalVisionPath = "version/version.txt";

	public static string Version = "1.0.0.0";

	public static string OldVersion = "0.0.0.0";

	public static string LocalFileListpath = "version/FileList.txt";

	public static string LocalDatapath;

	public static string CodeVersion = "0.83.0.0";

	public const string FONT_AB_PATH = "fonts/fonts.ab";

	public const string COMMON_AB_PATH = "icon/common.ab";

	public const string EMOJI_AB_PATH = "icon/emoji.ab";

	public const string Shader_AB_PATH = "shader/shader.ab";

	public const string Texture_Woman_PATH = "texture/woman.ab";

	public const string Texture_Man_PATH = "texture/man.ab";

	public const string OUTBATTLEMC = "Bgm_loading";

	public const string INBATTLEMC = "youxineibgm";

	public const string SmallHeadSprite = "";

	public static List<Color> _OwnerMarkColors = new List<Color>();

	public const string RoleResPath = "role/";

	public const string EffectResPath = "effect/";

	public const string WaterMarkResPath = "other/";

	public static bool IsCheck = false;

	public static bool IsSdkLogin = false;

	public static string ChatCommonRecordPath = "chatcommonrecord.oc";

	public static string ChatRecordPath = "chatrecord.oc";

	public static string ChatVoiceSavePath = "voice";

	public const string VoiceUpPath = "voicedown";

	public const string VoiceDownPath = "voiceup";

	public const string SensitiveWordPath = "sensitiveWord/sw.txt";

	public static bool FirstSdk = false;

	public static Color HobbySelect = new Color(4f / 15f, 0.58431375f, 0.5882353f);

	public static Color HobbyBg = new Color(0.42745098f, 0.42745098f, 0.42745098f);

	public static Color ShopItemBg = new Color(0.64705884f, 0.9254902f, 0.45490196f);

	public static Color BattleInAirportBlue = new Color(12f / 85f, 35f / 51f, 1f);

	public static Color BattleInAirportRed = new Color(1f, 22f / 85f, 22f / 85f);

	public static string CurrentTime
	{
		get
		{
			DateTime now = DateTime.Now;
			return now.Year + string.Empty + now.Month + now.Day + now.Hour + now.Minute + now.Second + now.Millisecond;
		}
	}

	public static List<Color> OwnerMarkColors
	{
		get
		{
			if (_OwnerMarkColors.Count <= 0)
			{
				_OwnerMarkColors.Add(new Color(1f, 0.41960785f, 0.41960785f));
				_OwnerMarkColors.Add(new Color(1f, 11f / 15f, 0.49803922f));
				_OwnerMarkColors.Add(new Color(1f, 1f, 43f / 85f));
				_OwnerMarkColors.Add(new Color(59f / 85f, 1f, 25f / 51f));
				_OwnerMarkColors.Add(new Color(0.5803922f, 1f, 40f / 51f));
				_OwnerMarkColors.Add(new Color(0.5803922f, 46f / 51f, 1f));
				_OwnerMarkColors.Add(new Color(0f, 10f / 51f, 1f));
				_OwnerMarkColors.Add(new Color(0.5803922f, 31f / 51f, 1f));
				_OwnerMarkColors.Add(new Color(46f / 51f, 56f / 85f, 1f));
				_OwnerMarkColors.Add(new Color(1f, 56f / 85f, 0.78039217f));
			}
			return _OwnerMarkColors;
		}
	}

	public static void Init()
	{
	}

	public static void CreatPath(string directory)
	{
		string persistentPath = Utils.GetPersistentPath(directory);
		if (!Directory.Exists(persistentPath))
		{
			Directory.CreateDirectory(persistentPath.Trim());
		}
	}

	public static float[] culcPos(int count, int width)
	{
		int num = width >> 1;
		float[] array = new float[count];
		if (count < 2)
		{
			return array;
		}
		int num2 = count - 1;
		for (int i = 0; i < count; i++)
		{
			array[i] = num2 * num;
			num2 -= 2;
		}
		return array;
	}

	public static Vector3[] culcPos(int count, int width, int height, int rows, int cols)
	{
		Vector3[] result = new Vector3[count];
		for (int i = 0; i < count; i++)
		{
		}
		return result;
	}

	public static string GetPhotoPath(long roleid, int vision)
	{
		return string.Empty;
	}

	public static float GetDistance(int lat1, int lng1, int lat2, int lng2)
	{
		float num = rad(lat1);
		float num2 = rad(lat2);
		float num3 = num - num2;
		float num4 = rad(lng1) - rad(lng2);
		float num5 = 2f * Mathf.Asin(Mathf.Sqrt(Mathf.Pow(Mathf.Sin(num3 / 2f), 2f) + Mathf.Cos(num) * Mathf.Cos(num2) * Mathf.Pow(Mathf.Sin(num4 / 2f), 2f)));
		num5 *= 6378.137f;
		return Mathf.Round(num5 * 10000f) / 10000f;
	}

	private static float rad(float d)
	{
		return d * (float)Math.PI / 180000f;
	}

	public static string FigureChinese(int num)
	{
		if (num > 10)
		{
			int num2 = num / 10;
			int num3 = num % 10;
			string text = ((num2 <= 1) ? Utils.GetString(117) : (Utils.GetString(num2 + 107) + Utils.GetString(117)));
			return text + ((num3 <= 0) ? string.Empty : Utils.GetString(num3 + 107));
		}
		return Utils.GetString(num + 107);
	}

	public static int GetVision(string num)
	{
		if (string.IsNullOrEmpty(num))
		{
			return 0;
		}
		string[] array = num.Split('_');
		if (array.Length == 2)
		{
			return int.Parse(array[1]);
		}
		return 1;
	}

	public static bool GetBool(string key, bool defaultValue = false)
	{
		return GetInt(key, defaultValue ? 1 : 0) == 1;
	}

	public static void SetBool(string key, bool value)
	{
		PlayerPrefs.SetInt(key, value ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static int GetInt(string key, int defaultValue = 0)
	{
		if (!PlayerPrefs.HasKey(key))
		{
			return defaultValue;
		}
		return PlayerPrefs.GetInt(key);
	}

	public static void SetInt(string key, int value, Utils.IntDelegate callBack = null)
	{
		PlayerPrefs.SetInt(key, value);
		PlayerPrefs.Save();
		Utils.TriggerEvent(callBack, value);
	}
}
