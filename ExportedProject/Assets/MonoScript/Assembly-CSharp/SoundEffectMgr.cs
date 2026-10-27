public class SoundEffectMgr
{
	public const string CommonBtn = "ui_dianji";

	public const string CloseBtn = "ui_close";

	public const string StartFightBtn = "ui_fight";

	public const string TeamAdd = "ui_team_join";

	public const string TeamRemove = "ui_team_leave";

	public const string GetAward = "ui_reward";

	public const string FriendOnline = "ui_fried";

	public const string NoSound = "no";

	public static void CommonBtnSound()
	{
		SingletonMono<AudioManager>.Ins.Play2D("ui_dianji");
	}

	public static void CloseBtnSound()
	{
		SingletonMono<AudioManager>.Ins.Play2D("ui_close");
	}

	public static void StartFightBtnSound()
	{
		SingletonMono<AudioManager>.Ins.Play2D("ui_fight");
	}

	public static void TeamAddSound()
	{
		SingletonMono<AudioManager>.Ins.Play2D("ui_team_join");
	}

	public static void TeamRemoveSound()
	{
		SingletonMono<AudioManager>.Ins.Play2D("ui_team_leave");
	}

	public static void GetAwardSound()
	{
		SingletonMono<AudioManager>.Ins.Play2D("ui_reward");
	}

	public static void FriendOnlineSound()
	{
		SingletonMono<AudioManager>.Ins.Play2D("ui_fried");
	}
}
