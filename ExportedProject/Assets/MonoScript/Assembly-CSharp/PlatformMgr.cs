using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Share;
using UnityEngine;
using cfg;
using gs.online.scmsg;
using gs.shop.scmsg;

public class PlatformMgr : Singleton<PlatformMgr>
{
	private int mPlatform;

	private Octets mLoginData = new Octets();

	private bool useSdk;

	private bool isCreateRole;

	public static string idfa = string.Empty;

	public static string idfv = string.Empty;

	private CReportCharge cReportCharge = new CReportCharge();

	private string[] closeSharePackageNames = new string[0];

	private const string SHOW_ADS_INFO = "SHOW_ADS_INFO";

	private Dictionary<int, string> mDicErrorCodeToMsg = new Dictionary<int, string>();

	private float mTimeToConnectToGs;

	private string channelId = string.Empty;

	private string appChannelId = string.Empty;

	public bool ShowShare { get; set; }

	public bool ShowShareTip { get; set; }

	public bool IsTest
	{
		get
		{
			return mPlatform == 0;
		}
	}

	public bool IsHgAndroid
	{
		get
		{
			return mPlatform == 3;
		}
	}

	public bool IsHgIos
	{
		get
		{
			return mPlatform == 4;
		}
	}

	public bool IsHeroUSdkAndroid
	{
		get
		{
			return mPlatform == 1;
		}
	}

	public bool IsChinaAppStore
	{
		get
		{
			return mPlatform == 2;
		}
	}

	private bool IsRunTimeAndroid
	{
		get
		{
			return Application.platform == RuntimePlatform.Android;
		}
	}

	public int Platform
	{
		get
		{
			return mPlatform;
		}
	}

	public Octets LoginData
	{
		get
		{
			return mLoginData;
		}
	}

	public void SetPlatformId(int platformId)
	{
		mPlatform = platformId;
		if (!IsTest)
		{
			InitErrorCodeDic();
			SCreateRole.handler = (SCreateRole.Handler)Delegate.Combine(SCreateRole.handler, new SCreateRole.Handler(_003CSetPlatformId_003Em__0));
			SLoginError.handler = (SLoginError.Handler)Delegate.Combine(SLoginError.handler, new SLoginError.Handler(_003CSetPlatformId_003Em__1));
			SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Combine(SLoginFinished.handler, new SLoginFinished.Handler(_003CSetPlatformId_003Em__2));
			SChargeSuccess.handler = (SChargeSuccess.Handler)Delegate.Combine(SChargeSuccess.handler, new SChargeSuccess.Handler(OnChargeSuccess));
		}
	}

	private void InitErrorCodeDic()
	{
		mDicErrorCodeToMsg.Clear();
		mDicErrorCodeToMsg[1] = "NameContainsSensitiveWord";
		mDicErrorCodeToMsg[2] = "NameIsTooShort";
		mDicErrorCodeToMsg[3] = "NameIsTooLong";
		mDicErrorCodeToMsg[8] = "NameIsInUse";
		mDicErrorCodeToMsg[10] = "ServerIsBusy";
	}

	public int GetPlatformId()
	{
		return mPlatform;
	}

	public void SetChannelId(string id)
	{
		channelId = id;
	}

	public string GetChannelId()
	{
		if (IsTest || IsHgAndroid)
		{
			return "899";
		}
		return channelId;
	}

	public void SetAppChannelId(string id)
	{
		appChannelId = id;
	}

	public string GetAppChannelId()
	{
		if (IsTest || IsHgAndroid)
		{
			return "0";
		}
		return appChannelId;
	}

	public void Login()
	{
		if (mPlatform == 0)
		{
			if (LoginData.Size == 0)
			{
				SetLoginData(new Octets().push(PlayerPrefsData.Account).push(PlayerPrefsData.SessionKey));
			}
			Singleton<AuthMgr>.Ins.SetAuth(PlayerPrefsData.Account, PlayerPrefsData.SessionKey);
			Singleton<AuthMgr>.Ins.Start();
		}
		else if (!IsTest && IsRunTimeAndroid)
		{
			AndroidSDKInterface.Instance.Login();
		}
	}

	public void OnPlatformLogoff()
	{
	}

	public void OnClickBack()
	{
	}

	public bool UseSdk()
	{
		return useSdk;
	}

	public void SetUseSdk()
	{
		useSdk = true;
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidSDKInterface.Instance.Init();
			Utils.SetPesistentPathFromJava();
		}
	}

	public void SetLoginData(Octets data)
	{
		mLoginData = data;
	}

	public void Logout()
	{
		if (IsRunTimeAndroid)
		{
			AndroidSDKInterface.Instance.Logout();
		}
	}

	public void OnLoginToServer()
	{
		OnLoginRole(isCreateRole);
		isCreateRole = false;
	}

	public void OnCreateRole()
	{
		isCreateRole = true;
	}

	public void OnFinishNewBee()
	{
		if (IsHgAndroid)
		{
			AndroidSDKInterface.Instance.OnFinishNewBee();
		}
	}

	public void OnConfigLoadComplete()
	{
		if (IsHgAndroid)
		{
			AndroidSDKInterface.Instance.OnConfigLoadComplete();
		}
	}

	public void OnResourceLoaded()
	{
		if (IsHgAndroid)
		{
			AndroidSDKInterface.Instance.OnResourceLoaded();
		}
	}

	public void OnChargeSuccess(SChargeSuccess sChargeSuccess)
	{
		if (IsHgAndroid)
		{
			AndroidSDKInterface.Instance.OnChargeSuccess(sChargeSuccess.amount);
		}
		if (!IsTest && !IsHgAndroid)
		{
			AndroidSDKInterface.Instance.OnPaySuccess(sChargeSuccess.orderId);
		}
	}

	public void BindAccount()
	{
		if (IsHgAndroid)
		{
			AndroidSDKInterface.Instance.BindAccount();
		}
	}

	public bool IsOverSea()
	{
		if (mPlatform == 3 || mPlatform == 4)
		{
			return true;
		}
		return false;
	}

	public void ShowPayView(int goodsId)
	{
		int currentServerId = Singleton<ServerMgr>.Ins.GetCurrentServerId();
		RechargeCfg rechargeCfg = RechargeCfg.Get(goodsId);
		int num = 0;
		bool flag = false;
		if (IsHgAndroid)
		{
			AndroidSDKInterface.Instance.ShowPayView(goodsId, rechargeCfg.priceInfos[0].name, currentServerId, Singleton<RoleMgr>.Ins.info.userId);
		}
		else if (IsHeroUSdkAndroid)
		{
			string name = Singleton<RoleMgr>.Ins.info.name;
			long roleId = Singleton<RoleMgr>.Ins.info.roleId;
			ShopPriceInfo shopPriceInfo = rechargeCfg.priceInfos[1];
			int count = shopPriceInfo.num;
			if (rechargeCfg.rechargeType == 2)
			{
				count = 1;
			}
			string goodsName = shopPriceInfo.name;
			if (rechargeCfg.rechargeType == 1)
			{
				goodsName = "点券";
			}
			if (flag && shopPriceInfo.firstZengArg > 0 && shopPriceInfo.firstZengType == 2)
			{
				num += shopPriceInfo.firstZengArg;
			}
			if (shopPriceInfo.zengArg > 0 && shopPriceInfo.zengType == 2)
			{
				num += shopPriceInfo.zengArg;
			}
			AndroidSDKInterface.Instance.ShowPayView(currentServerId, name, roleId, goodsId, goodsName, count, (int)shopPriceInfo.price, num);
		}
	}

	public void OnLoginRole(bool isCreateRole)
	{
		float num = Time.fixedTime - mTimeToConnectToGs;
		if (num < 0f)
		{
			num = 1f;
		}
		else if (num > 300f)
		{
			num = Utils.Random(1, 300);
		}
		if (!IsTest)
		{
			AndroidSDKInterface.Instance.OnLoginRole(isCreateRole, (int)num);
		}
	}

	public void ShareContent(string title, string imagePath)
	{
		AndroidSDKInterface.Instance.ShareContent(title, imagePath);
	}

	public void ShareWeChatFriend(string title, string path)
	{
		AndroidSDKInterface.Instance.ShareToWeChatFriend(title, path);
	}

	public void ShareWeChatZone(string title, string path)
	{
		AndroidSDKInterface.Instance.ShareToWeChatZone(title, path);
	}

	public bool IsOpenCharge()
	{
		return false;
	}

	public bool IsOpenShare()
	{
		if (IsOverSea() || Client2Gs.isServerInReview)
		{
			return false;
		}
		try
		{
			string packageName = AndroidSDKInterface.Instance.GetPackageName();
			string[] array = closeSharePackageNames;
			foreach (string text in array)
			{
				if (text.Equals(packageName))
				{
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
		return true;
	}

	public void ShowScreenAds()
	{
		try
		{
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	public void onGetTaskAward(int taskId)
	{
		if (!IsTest)
		{
			AndroidSDKInterface.Instance.onGetTaskAward(taskId);
		}
	}

	public void onCreateRole(int status, string reason)
	{
		if (!IsTest)
		{
			AndroidSDKInterface.Instance.onCreateRole(status, reason);
		}
	}

	public void onCancelWait()
	{
		if (!IsTest)
		{
			AndroidSDKInterface.Instance.onCancelWait();
		}
	}

	public void onStartWait()
	{
		if (!IsTest)
		{
			AndroidSDKInterface.Instance.onStartWait();
		}
	}

	public void OnConnectToGs()
	{
		if (!IsTest)
		{
			mTimeToConnectToGs = Time.fixedTime;
			AndroidSDKInterface.Instance.OnConnectToGs();
		}
	}

	public void OnDisconnectedFromGs()
	{
		if (!IsTest)
		{
			AndroidSDKInterface.Instance.OnDisconnectedFromGs();
		}
	}

	public void onUpdateGameResource(int actionId, int status, int time)
	{
		if (!IsTest)
		{
			AndroidSDKInterface.Instance.onUpdateGameResource(actionId, status, time);
		}
	}

	public void onSelectServer(int serverId)
	{
		if (!IsTest)
		{
			AndroidSDKInterface.Instance.onSelectServer(serverId);
		}
	}

	[CompilerGenerated]
	private void _003CSetPlatformId_003Em__0(SCreateRole msg)
	{
		OnCreateRole();
	}

	[CompilerGenerated]
	private void _003CSetPlatformId_003Em__1(SLoginError msg)
	{
		string value = null;
		if (mDicErrorCodeToMsg.TryGetValue(msg.code, out value))
		{
			onCreateRole(3, value);
		}
	}

	[CompilerGenerated]
	private void _003CSetPlatformId_003Em__2(SLoginFinished msg)
	{
		OnLoginToServer();
	}
}
