using System;
using System.Collections;
using Share;
using UnityEngine;
using cfg;

public class SdkMessager : MonoBehaviour
{
	private readonly string SDK_FILE = "usesdk.txt";

	public static bool joinRoomAfterInited;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		StartCoroutine(CheckUseSdk());
	}

	public void OnHgLogin(string data)
	{
		Singleton<PlatformMgr>.Ins.SetLoginData(new Octets().push(data));
		Singleton<AuthMgr>.Ins.Start();
	}

	public void OnHgLogout(string arg)
	{
		Singleton<PlatformMgr>.Ins.OnPlatformLogoff();
	}

	public void OnQuickLoginSuccess(string data)
	{
		string[] array = data.Split(',');
		Singleton<PlatformMgr>.Ins.SetLoginData(new Octets().push(array[0]).push(array[1]).push(array[2]));
		Singleton<AuthMgr>.Ins.Start();
	}

	public void OnOppOLogin(string data)
	{
		string[] array = data.Split(',');
		Singleton<PlatformMgr>.Ins.SetLoginData(new Octets().push(array[0]).push(array[1]));
		Singleton<AuthMgr>.Ins.Start();
	}

	public void OnLogin(string data)
	{
		string[] array = data.Split(',');
		Octets octets = new Octets();
		string[] array2 = array;
		foreach (string x in array2)
		{
			octets.push(x);
		}
		Singleton<PlatformMgr>.Ins.SetLoginData(octets);
		Singleton<AuthMgr>.Ins.Start();
	}

	public void OnLogout(string data)
	{
		Singleton<PlatformMgr>.Ins.OnPlatformLogoff();
	}

	public void SetPlatformId(string data)
	{
		int platformId = int.Parse(data);
		Singleton<PlatformMgr>.Ins.SetPlatformId(platformId);
	}

	public void LogMessage(string content)
	{
		Debug.LogError(content);
	}

	public void OnClickback(string data)
	{
		Singleton<PlatformMgr>.Ins.OnClickBack();
	}

	public void OnBatteryChange(string data)
	{
	}

	public void OnChinaAppStoreUnFinishedOrder(string data)
	{
		try
		{
			int result;
			if (int.TryParse(data, out result))
			{
				RechargeCfg rechargeCfg = RechargeCfg.Get(result);
				if (rechargeCfg != null)
				{
					Singleton<PlatformMgr>.Ins.ShowPayView(result);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public void OnVoiceSdkInitFinished(string data)
	{
	}

	public void OnSetIdfa(string data)
	{
		Debug.LogError("idfa=" + data);
		PlatformMgr.idfa = data;
	}

	public void OnSetIdfv(string data)
	{
		Debug.LogError("idfv=" + data);
		PlatformMgr.idfv = data;
	}

	public void OnShareSuccess(string data)
	{
	}

	public void OnWatchAd(string data)
	{
	}

	private IEnumerator CheckUseSdk()
	{
		WWW www = new WWW(Utils.GetFilePathForWWW(SDK_FILE));
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			Singleton<PlatformMgr>.Ins.SetUseSdk();
		}
		else
		{
			Debug.LogError("NotNeedUseSdk");
		}
	}

	public void SetChannelInfo(string data)
	{
		string[] array = data.Split(',');
		Singleton<PlatformMgr>.Ins.SetChannelId(array[0]);
		Singleton<PlatformMgr>.Ins.SetAppChannelId(array[1]);
	}
}
