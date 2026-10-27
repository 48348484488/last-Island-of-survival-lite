using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SC.UI;
using UnityEngine;
using gs.online.scmsg;

public class LoginMgr : Singleton<LoginMgr>
{
	public string Account;

	public string SessionKey;

	private bool _needCancelBattle;

	public bool IsReConnectGs;

	public bool IsShowReConnectGsInMainPanel;

	public bool IsActiveDisconnectBs;

	private const string TimeOutName = "login";

	public int nLoginServerWaitNum;

	private int _loginTimeOutCount;

	[CompilerGenerated]
	private static Utils.VoidDelegate _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Utils.VoidDelegate _003C_003Ef__mg_0024cache1;

	public void Init()
	{
		nLoginServerWaitNum = 0;
		SNoCreateRole.handler = (SNoCreateRole.Handler)Delegate.Combine(SNoCreateRole.handler, new SNoCreateRole.Handler(SNoCreateRoleHandle));
		Ping_Gs_Client.handler = (Ping_Gs_Client.Handler)Delegate.Combine(Ping_Gs_Client.handler, new Ping_Gs_Client.Handler(KeepLiveHandle));
		SLoginError.handler = (SLoginError.Handler)Delegate.Combine(SLoginError.handler, new SLoginError.Handler(SLoginErrorHandle));
		OnlineEvent.onConnectedGs = (Utils.VoidDelegate)Delegate.Combine(OnlineEvent.onConnectedGs, new Utils.VoidDelegate(OnConnectedGs));
		OnlineEvent.onDisconnectedGs = (Utils.VoidDelegate)Delegate.Combine(OnlineEvent.onDisconnectedGs, new Utils.VoidDelegate(OnDisconnectedGs));
		OnlineEvent.onConnectGsFailed = (Utils.VoidDelegate)Delegate.Combine(OnlineEvent.onConnectGsFailed, new Utils.VoidDelegate(OnConnectGsFailed));
		OnlineEvent.onDisconnectedBs = (Utils.VoidDelegate)Delegate.Combine(OnlineEvent.onDisconnectedBs, new Utils.VoidDelegate(onDisconnectedBs));
		OnlineEvent.onConnectedBs = (Utils.VoidDelegate)Delegate.Combine(OnlineEvent.onConnectedBs, new Utils.VoidDelegate(onConnectedBs));
		OnlineEvent.onConnectBsFailed = (Utils.VoidDelegate)Delegate.Combine(OnlineEvent.onConnectBsFailed, new Utils.VoidDelegate(onConnectBsFailed));
		OnlineEvent.onConnectedAs = (Utils.VoidDelegate)Delegate.Combine(OnlineEvent.onConnectedAs, new Utils.VoidDelegate(OnConnectedAs));
		SLoginFinished.handler = (SLoginFinished.Handler)Delegate.Combine(SLoginFinished.handler, new SLoginFinished.Handler(SLoginFinishedHandle));
		SWaitRank.handler = (SWaitRank.Handler)Delegate.Combine(SWaitRank.handler, new SWaitRank.Handler(onSWaitRank));
	}

	private void onSWaitRank(SWaitRank msg)
	{
		nLoginServerWaitNum = msg.rank;
		Singleton<PlatformMgr>.Ins.onStartWait();
		ViewMgr.Ins.ShowTopView<LoginLinePanel>();
	}

	private void SLoginFinishedHandle(SLoginFinished msg)
	{
		IsReConnectGs = true;
		_loginTimeOutCount = 0;
		TimeManager.UnregisterCountDown("login");
	}

	private void SLoginErrorHandle(SLoginError msg)
	{
		switch (msg.code)
		{
		case 1:
			AlertBox.Show(254);
			break;
		case 2:
			AlertBox.Show(255);
			break;
		case 3:
			AlertBox.Show(256);
			break;
		case 4:
			AccountError();
			AlertBox.Show(257);
			break;
		case 5:
			AccountError();
			AlertBox.Show(258);
			break;
		case 6:
			AccountError();
			AlertBox.Show(259);
			break;
		case 7:
			AccountError();
			MessageBoxPanel.ShowConfirm(411, AndroidSDKInterface.Instance.RestartApplication);
			break;
		case 8:
			AlertBox.Show(273);
			break;
		}
		_needCancelBattle = false;
	}

	private void AccountError()
	{
		CloseGs();
		TimeManager.UnregisterCountDown("login");
	}

	private void onDisconnectedBs()
	{
	}

	private void onConnectedBs()
	{
		IsActiveDisconnectBs = false;
	}

	private void onConnectBsFailed()
	{
	}

	private void OnConnectGsFailed()
	{
		if (_003C_003Ef__mg_0024cache0 == null)
		{
			_003C_003Ef__mg_0024cache0 = Application.Quit;
		}
		MessageBoxPanel.ShowConfirm(140, _003C_003Ef__mg_0024cache0);
		ShowDisconnectGsMessageBox();
	}

	private void OnDisconnectedGs()
	{
		if (_003C_003Ef__mg_0024cache1 == null)
		{
			_003C_003Ef__mg_0024cache1 = Application.Quit;
		}
		MessageBoxPanel.ShowConfirm(140, _003C_003Ef__mg_0024cache1);
		Singleton<PlatformMgr>.Ins.OnDisconnectedFromGs();
	}

	private void OnConnectedGs()
	{
		Singleton<PlatformMgr>.Ins.OnConnectToGs();
		ViewMgr.Ins.HideView<MessageBoxPanel>();
		Login();
	}

	private void KeepLiveHandle(Ping_Gs_Client msg)
	{
		Client2Gs.Ins.Send(msg);
	}

	private void SNoCreateRoleHandle(SNoCreateRole msg)
	{
	}

	public void Login()
	{
		CAutoLoginRole cAutoLoginRole = new CAutoLoginRole();
		cAutoLoginRole.account = Singleton<AuthMgr>.Ins.UserId;
		cAutoLoginRole.sessionKey = Singleton<AuthMgr>.Ins.SessionKey;
		cAutoLoginRole.version = ReadyVeision();
		cAutoLoginRole.code_version = GameConst.CodeVersion;
		cAutoLoginRole.reLogin = IsReConnectGs;
		cAutoLoginRole.phoneID = SystemInfo.deviceUniqueIdentifier;
		if (Application.platform == RuntimePlatform.Android)
		{
			cAutoLoginRole.platformType = 1;
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
		{
			cAutoLoginRole.platformType = 0;
		}
		else
		{
			cAutoLoginRole.platformType = -1;
		}
		Client2Gs.Ins.Send(cAutoLoginRole);
		Utils.TriggerEvent(LoginEvent.OnLoginEvent);
	}

	public void SetIpAddress(string host, int port)
	{
		Client2Gs.Ins.SetHost(host, port);
	}

	public void Connect()
	{
		CloseGs();
		Client2Gs.Ins.Connect();
		TimeManager.RegisterCountDown("login", 60f, LoginTimeOut);
	}

	private void LoginTimeOut()
	{
		CloseGs();
		_loginTimeOutCount++;
		if (_loginTimeOutCount > 1)
		{
			_loginTimeOutCount = 0;
			LoginPlatformMgr();
		}
		ShowDisconnectGsMessageBox();
	}

	public void LoginPlatformMgr()
	{
		Debug.LogError("LoginPlatformMgr:");
		IsReConnectGs = false;
	}

	private void ShowDisconnectGsMessageBox(int tip = 168)
	{
	}

	public static string ReadyVeision()
	{
		return GameConst.Version;
	}

	public void CloseGs()
	{
		if (Client2Gs.Ins.IsConnecting())
		{
			Client2Gs.Ins.Close();
		}
	}

	public void OnConnectedAs()
	{
		ViewMgr.Ins.ShowView<LoginPanel>();
	}

	public void SetList(string key, List<int> values, Utils.VoidDelegate callBack = null)
	{
		string text = string.Empty;
		for (int i = 0; i < values.Count; i++)
		{
			text = text + values[i] + ",";
		}
		PlayerPrefs.SetString(key, text);
		PlayerPrefs.Save();
		Utils.TriggerEvent(callBack);
	}

	public List<int> GetList(string key)
	{
		List<int> list = new List<int>();
		string empty = string.Empty;
		empty = PlayerPrefs.GetString(key);
		string[] array = empty.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				list.Add(int.Parse(array[i]));
			}
		}
		return list;
	}
}
