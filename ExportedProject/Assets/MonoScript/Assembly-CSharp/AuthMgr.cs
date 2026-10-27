using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Share;
using UnityEngine;
using auth.msg;

public class AuthMgr : Singleton<AuthMgr>
{
	private string userId = string.Empty;

	private string sessionKey = string.Empty;

	public static float AUTH_TIMEOUT = 45f;

	private SPlatformLogin response;

	public HashSet<int> HistoryGs
	{
		get
		{
			return response.historyGs;
		}
	}

	public HashSet<int> OpenGs
	{
		get
		{
			return response.openGs;
		}
	}

	public SPlatformLogin PlatformLogin
	{
		get
		{
			return response;
		}
	}

	public string UserId
	{
		get
		{
			return userId;
		}
	}

	public string SessionKey
	{
		get
		{
			return sessionKey;
		}
	}

	public bool IsAuth
	{
		get
		{
			return !string.IsNullOrEmpty(sessionKey);
		}
	}

	public void Start()
	{
		Utils.StartConroutine(DoLoginRequest());
	}

	public void SetAuth(string userId, string sessionKey)
	{
		this.userId = userId;
		this.sessionKey = sessionKey;
	}

	public void CancelAuth()
	{
		userId = string.Empty;
		sessionKey = string.Empty;
	}

	private IEnumerator DoLoginRequest()
	{
		Debug.LogError("Start Auth Request 111: Connect " + ServerPath.AuthServerAddress);
		CPlatformLogin platformLogin = new CPlatformLogin
		{
			data = Singleton<PlatformMgr>.Ins.LoginData,
			platform = Singleton<PlatformMgr>.Ins.Platform,
			deviceUniqueID = SystemInfo.deviceUniqueIdentifier,
			deviceModel = SystemInfo.deviceModel,
			mac = Application.productName + "_" + GetMacAddress()
		};
		Octets oc2 = new Octets();
		oc2.push(platformLogin);
		WWW www = new WWW(ServerPath.AuthServerAddress, oc2.getBytes());
		bool wwwFinished = true;
		float startTime = Time.timeSinceLevelLoad;
		while (!www.isDone)
		{
			if (Time.timeSinceLevelLoad - startTime > AUTH_TIMEOUT)
			{
				wwwFinished = false;
				break;
			}
			yield return null;
		}
		if (wwwFinished && string.IsNullOrEmpty(www.error))
		{
			oc2 = new Octets(www.bytes, www.bytes.Length);
			response = new SPlatformLogin();
			response.unmarshal(oc2);
			OnSPlatformLogin(response);
		}
		else if (wwwFinished)
		{
			Debug.LogError("[auth]request auth error:" + www.error);
		}
		else
		{
			Debug.LogError("[auth]request auth time out");
		}
	}

	private void OnSPlatformLogin(SPlatformLogin msg)
	{
		Debug.LogError("SPlatformLogin code 111=" + msg.code + " userid=" + msg.userId + " session=" + msg.session);
		if (msg.code == 0)
		{
			SetAuth(msg.userId, msg.session);
			Utils.TriggerEvent(OnlineEvent.onConnectedAs);
			Singleton<LoginMgr>.Ins.Connect();
		}
		else if (msg.code != 2)
		{
			if (msg.code == 4)
			{
				MessageBoxPanel.ShowConfirm(492);
			}
			else if (msg.code == 5)
			{
				MessageBoxPanel.ShowConfirm(498);
			}
		}
	}

	public string GetMacAddress()
	{
		string text = string.Empty;
		switch (Application.platform)
		{
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.IPhonePlayer:
			try
			{
				NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
				BetterList<string> betterList = new BetterList<string>();
				NetworkInterface[] array = allNetworkInterfaces;
				foreach (NetworkInterface networkInterface in array)
				{
					string text2 = networkInterface.NetworkInterfaceType.ToString().ToLower();
					string text3 = networkInterface.Description.ToString().ToLower();
					if (text2.Contains("ethernet"))
					{
						string text4 = networkInterface.GetPhysicalAddress().ToString();
						betterList.Add(text4);
						if (text3.Equals("en0"))
						{
							text = text4;
							break;
						}
					}
				}
				if (text.Length == 0 && betterList.size != 0)
				{
					text = betterList[0];
				}
				if (text.Length == 12)
				{
					int num = 6;
					for (int j = 0; j < num - 1; j++)
					{
						text = text.Insert(3 * j + 2, ":");
					}
				}
			}
			catch (Exception message2)
			{
				Debug.LogError(message2);
			}
			break;
		case RuntimePlatform.Android:
			try
			{
				AndroidJavaObject androidJavaObject = null;
				using (AndroidJavaObject androidJavaObject2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
				{
					androidJavaObject = androidJavaObject2.Call<AndroidJavaObject>("getSystemService", new object[1] { "wifi" });
				}
				if (androidJavaObject != null)
				{
					text = androidJavaObject.Call<AndroidJavaObject>("getConnectionInfo", new object[0]).Call<string>("getMacAddress", new object[0]);
				}
				text = text + "_" + Utils.GetAndroidId();
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			break;
		}
		if (string.IsNullOrEmpty(text))
		{
			text = "00:00:00:00:00:00";
			text = text + "_" + Utils.GetAndroidId();
		}
		return text;
	}
}
