using System.Collections;
using UnityEngine;

public class AndroidSDKInterface
{
	public static AndroidSDKInterface Instance = new AndroidSDKInterface();

	private AndroidJavaObject mAndroidJavaObject;

	public void Init()
	{
		Debug.LogError("Init Android Sdk");
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		mAndroidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		SetPlatformId();
	}

	private void call(string methodName, params object[] args)
	{
		mAndroidJavaObject.Call(methodName, args);
	}

	private int callReturnInt(string methodName, params object[] args)
	{
		return mAndroidJavaObject.Call<int>(methodName, args);
	}

	private bool callReturnBool(string methodName, params object[] args)
	{
		return mAndroidJavaObject.Call<bool>(methodName, args);
	}

	private string callReturnString(string methodName, params object[] args)
	{
		return mAndroidJavaObject.Call<string>(methodName, args);
	}

	public void Login()
	{
		call("Login");
	}

	public void Logout()
	{
		call("Logout");
	}

	public void ShowPayView(int productId, string productDesc, int serverId, string userId)
	{
		call("ShowPayView", productId, productDesc, serverId, userId, Singleton<RoleMgr>.Ins.info.roleId, Singleton<RoleMgr>.Ins.info.name);
	}

	public void ShowPayView(int serverId, string roleName, long roleId, int goodsId, string goodsName, int count, int amount, int donate)
	{
		call("ShowPayView", serverId, roleName, roleId, goodsId, goodsName, count, amount, donate, (int)Singleton<RoleMgr>.Ins.info.level, 0, Singleton<RoleMgr>.Ins.Coupons, Singleton<RoleMgr>.Ins.Diamond, Singleton<RoleMgr>.Ins.info.userId);
	}

	public void ClickBack()
	{
		call("ClickBack");
	}

	public string GetObbAssetsPath(string relPath)
	{
		return callReturnString("GetObbAssetsPath", relPath);
	}

	public string GetObbAssetsPathForWWW(string relPath)
	{
		return callReturnString("GetObbAssetsPathForWWW", relPath);
	}

	public void OnLoginToServer()
	{
		call("OnLoginToServer");
	}

	public void OnCreateRole()
	{
		call("OnCreateRole");
	}

	public void OnFinishNewBee()
	{
		call("OnFinishNewBee");
	}

	public void OnConfigLoadComplete()
	{
		call("OnConfigLoadComplete");
	}

	public void OnResourceLoaded()
	{
		call("OnResourceLoaded");
	}

	public void OnChargeSuccess(int amount)
	{
		call("OnChargeSuccess", amount);
	}

	public void BindAccount()
	{
		call("BindAccount");
	}

	public void ShareContent(string title, string imagePath)
	{
		call("ShareContent", title, imagePath);
	}

	public void SetPlatformId()
	{
		call("SetPlatformId");
	}

	public void OnLoginRole(bool isCreateRole, int loginTime)
	{
		Utils.StartConroutine(ReportLoginRole(isCreateRole, loginTime));
	}

	private IEnumerator ReportLoginRole(bool isCreateRole, int loginTime)
	{
		int hp = 0;
		while (Battle.Ins == null || Battle.Ins.SelfPlayer == null)
		{
			yield return new WaitForSeconds(0.5f);
		}
		hp = Battle.Ins.SelfPlayer.HP;
		string userId = Singleton<RoleMgr>.Ins.info.userId;
		string roleName = Singleton<RoleMgr>.Ins.info.name;
		long roleId = Singleton<RoleMgr>.Ins.info.roleId;
		int cupon = Singleton<RoleMgr>.Ins.Coupons;
		int gold = Singleton<RoleMgr>.Ins.Gold;
		int vipLevel = 0;
		int level = Singleton<RoleMgr>.Ins.info.level;
		bool isMale = Singleton<RoleMgr>.Ins.info.sex;
		call("OnLoginRole", userId, roleName, roleId, cupon, gold, hp, level, vipLevel, isCreateRole, isMale, Singleton<RoleMgr>.Ins.createRoleTime, loginTime);
	}

	public void OnLevelChange(int level)
	{
		call("OnLevelChange", level);
	}

	public void OnPaySuccess(string cpOrderId)
	{
		call("OnPaySuccess", cpOrderId);
	}

	public void Exit()
	{
		call("Exit");
	}

	public void QuickShare()
	{
		call("QuickShare");
	}

	public void RestartApplication()
	{
		call("restartApplication");
	}

	public string GetStreamAssetsPath()
	{
		return callReturnString("GetStreamAssetsPath");
	}

	public void ShareToWeChatFriend(string title, string path)
	{
		call("ShareToWeChatFriend", title, path);
	}

	public void ShareToWeChatZone(string title, string path)
	{
		call("ShareToWeChatZone", title, path);
	}

	public string GetPackageName()
	{
		return callReturnString("GetPackageName");
	}

	public void CancelAlarm(int alarmId)
	{
		call("CancleAlarm", alarmId);
	}

	public void SetAlarm(int alarmId, int time, string title, string content)
	{
		call("SetAlarm", alarmId, time, title, content);
	}

	public bool IsFullApk()
	{
		return callReturnBool("IsFullApk");
	}

	public void ShowBannerAd()
	{
		call("ShowBannerAd");
	}

	public void HideBannerAd()
	{
		call("HideBannerAd");
	}

	public void ShowScreenAd()
	{
		call("ShowScreenAd");
	}

	public void ShowVideoAd()
	{
		call("ShowVideoAd");
	}

	public void onGetTaskAward(int taskId)
	{
		call("onGetTaskAward", taskId);
	}

	public void onCreateRole(int status, string reason)
	{
		call("onCreateRole", status, reason);
	}

	public void onCancelWait()
	{
		call("onCancelWait");
	}

	public void onStartWait()
	{
		call("onStartWait");
	}

	public void OnConnectToGs()
	{
		call("OnConnectToGs");
	}

	public void OnDisconnectedFromGs()
	{
		call("OnDisconnectedFromGs");
	}

	public void onUpdateGameResource(int actionId, int status, int time)
	{
		call("onUpdateGameResource", actionId, status, time);
	}

	public void onSelectServer(int serverId)
	{
		call("onSelectServer", serverId);
	}
}
