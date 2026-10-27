using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SC.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using cfg;

public class Main : MonoBehaviour
{
	public static IntVector2 ScreenSize;

	public static IntVector2 OriginalScreenSize;

	private readonly string[] preloadAbs = new string[3] { "fonts/fonts.ab", "shader/shaders.ab", "icon/common.ab" };

	private Dictionary<string, CfgManager.OnLoaded> someCfgs;

	[CompilerGenerated]
	private static CfgManager.OnLoaded _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static CfgManager.OnLoaded _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static CfgManager.OnLoaded _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static CfgManager.OnLoaded _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static CfgManager.OnLoaded _003C_003Ef__mg_0024cache4;

	private void Awake()
	{
		Application.targetFrameRate = 60;
		QualitySettings.vSyncCount = 0;
		OriginalScreenSize.x = Screen.width;
		OriginalScreenSize.y = Screen.height;
		SetResolution();
		Utils.Init(ScreenSize.x, ScreenSize.y);
		Utils.SetCanvasMatchWidthOrHeight(base.gameObject);
		Singleton<SdkMgr>.Ins.Init();
		Worker.Ins.Init();
		Client2Gs.Ins.Init();
		ResMgr.Init();
		GameConst.Init();
		ViewMgr.Init(base.gameObject);
		Singleton<LoginMgr>.Ins.Init();
		Singleton<ServerMgr>.Ins.Init();
		Singleton<BattleScMgr>.Ins.Init();
		MailMgr.Ins.Init();
		initModules();
		Singleton<BasicInfoScMgr>.Ins.Init();
		Singleton<RoleMgr>.Ins.Init();
		Singleton<ChatScMgr>.Ins.Init();
		Singleton<FriendScMgr>.Ins.Init();
		Singleton<BagMgr>.Ins.Init();
		Singleton<ScProduceMgr>.Ins.Init();
		Singleton<WorkbenchPanelMgr>.Ins.Init();
		Singleton<TeamScMgr>.Ins.Init();
		Singleton<BoxMgr>.Ins.Init();
		Singleton<DrawMgr>.Ins.Init();
		Singleton<FurnaceMgr>.Ins.Init();
		Singleton<CookMgr>.Ins.Init();
		Singleton<LadderMgr>.Ins.Init();
		Singleton<TaskMgr>.Ins.Init();
		Singleton<BattleDropMgr>.Ins.Init();
		Singleton<TeamateMgr>.Ins.Init();
		Singleton<MapMgr>.Ins.Init();
		Singleton<StructureMenuMgr>.Ins.Init();
		Singleton<DoorMgr>.Ins.Init();
		Singleton<LockMgr>.Ins.Init();
		Singleton<FriendPermitMgr>.Ins.Init();
		Singleton<ManorChestMgr>.Ins.Init();
		Singleton<TurretMgr>.Ins.Init();
		Singleton<LandMineMgr>.Ins.Init();
		Singleton<SteelTrapMgr>.Ins.Init();
		Singleton<TrapMgr>.Ins.Init();
		Singleton<SharpWoodMgr>.Ins.Init();
		Singleton<WoodenPlacardMgr>.Ins.Init();
		Singleton<TimeShopMgr>.Ins.Init();
		Singleton<NormalShopMgr>.Ins.Init();
		Singleton<RechargeMgr>.Ins.Init();
		Singleton<ElectricityMgr>.Ins.Init();
		Singleton<ActivityMgr>.Ins.Init();
		Singleton<RebirthMgr>.Ins.Init();
		Singleton<TrashcanStationMgr>.Ins.Init();
		Singleton<GuideMgr>.Ins.Init();
		Singleton<SettingMgr>.Ins.Init();
		Singleton<FacilityStatusMgr>.Ins.Init();
	}

	private IEnumerator Start()
	{
		Screen.sleepTimeout = -1;
		yield return ServerPath.LoadServerPath();
		string[] array = preloadAbs;
		foreach (string ab in array)
		{
			yield return ResMgr.Ins.LoadAB(ab, null, false);
		}
		someCfgs = new Dictionary<string, CfgManager.OnLoaded>();
		Dictionary<string, CfgManager.OnLoaded> dictionary = someCfgs;
		if (_003C_003Ef__mg_0024cache0 == null)
		{
			_003C_003Ef__mg_0024cache0 = Strings.Load;
		}
		dictionary.Add("cfg.Strings.oc", _003C_003Ef__mg_0024cache0);
		Dictionary<string, CfgManager.OnLoaded> dictionary2 = someCfgs;
		if (_003C_003Ef__mg_0024cache1 == null)
		{
			_003C_003Ef__mg_0024cache1 = LoadPanelCfg.Load;
		}
		dictionary2.Add("cfg.LoadPanelCfg.oc", _003C_003Ef__mg_0024cache1);
		Dictionary<string, CfgManager.OnLoaded> dictionary3 = someCfgs;
		if (_003C_003Ef__mg_0024cache2 == null)
		{
			_003C_003Ef__mg_0024cache2 = MultiLanguageCfg.Load;
		}
		dictionary3.Add("cfg.MultiLanguageCfg.oc", _003C_003Ef__mg_0024cache2);
		Dictionary<string, CfgManager.OnLoaded> dictionary4 = someCfgs;
		if (_003C_003Ef__mg_0024cache3 == null)
		{
			_003C_003Ef__mg_0024cache3 = MultiLanguageIndexCfg.Load;
		}
		dictionary4.Add("cfg.MultiLanguageIndexCfg.oc", _003C_003Ef__mg_0024cache3);
		Dictionary<string, CfgManager.OnLoaded> dictionary5 = someCfgs;
		if (_003C_003Ef__mg_0024cache4 == null)
		{
			_003C_003Ef__mg_0024cache4 = BuildPart.Load;
		}
		dictionary5.Add("cfg.BuildPart.oc", _003C_003Ef__mg_0024cache4);
		yield return StartCoroutine(LoadSomeCfg());
		ViewMgr.Ins.CanvasSize = ViewMgr.Ins.CanvasTransfrom.sizeDelta;
		GameObject eventSystem = GameObject.Find("EventSystem");
		if (eventSystem != null)
		{
			EventSystem component = eventSystem.GetComponent<EventSystem>();
			if (component != null)
			{
				component.pixelDragThreshold = 5;
			}
		}
		int nPos = ServerPath.UdpServerAddress.IndexOf(':');
		if (nPos > 0)
		{
			UdpSession.Ins.Connect(ServerPath.UdpServerAddress.Substring(0, nPos), ServerPath.UdpServerAddress.Substring(nPos + 1));
		}
		ViewMgr.Ins.ShowView<LoadingPanel>();
		AfterInitCfgDoSomething();
		Shader.WarmupAllShaders();
		Utils.PrinteSystemInfo();
		Singleton<SettingMgr>.Ins.SetDefault();
		UdpSession.strLogHead = SystemInfo.deviceModel + " " + SystemInfo.systemMemorySize;
	}

	private IEnumerator LoadSomeCfg()
	{
		foreach (KeyValuePair<string, CfgManager.OnLoaded> pair in someCfgs)
		{
			WWW www = new WWW(Utils.GetFilePathForWWW("cfg/" + pair.Key));
			yield return www;
			if (string.IsNullOrEmpty(www.error))
			{
				try
				{
					pair.Value(www.bytes);
				}
				catch (Exception)
				{
					Debug.LogError("oc error, to delete it:" + pair.Key);
					UdpSession.Ins.Send("[LoadingPanel] oc error:" + pair.Key);
					Singleton<UpdateMgr>.Ins.ToRepairClient();
					MessageBoxPanel.ShowConfirm(411, AndroidSDKInterface.Instance.RestartApplication);
				}
				continue;
			}
			Debug.LogError(www.error);
			break;
		}
	}

	private void Update()
	{
		Client2Gs.Ins.Update();
		ViewMgr.Ins.CanvasSize = ViewMgr.Ins.CanvasTransfrom.sizeDelta;
		if (ScenePreloadAb.Ins != null)
		{
			ScenePreloadAb.Ins.ToLoadAbOnebyone();
		}
		SmallSceneMgr.Ins.Update();
	}

	private void initModules()
	{
		SmallSceneMgr.Ins.Init();
	}

	private void OnDestroy()
	{
		Worker.Ins.Destroy();
	}

	public static void SetResolution()
	{
		if (Screen.dpi > 500f && SystemInfo.systemMemorySize < 4096)
		{
			ScreenSize.x = Screen.width / 2;
			ScreenSize.y = Screen.height / 2;
			Screen.SetResolution(ScreenSize.x, ScreenSize.y, true);
		}
		else
		{
			ScreenSize.x = Screen.width;
			ScreenSize.y = Screen.height;
		}
		Utils.SetScreenWidthHeight(ScreenSize.x, ScreenSize.y);
	}

	private void LowEndPhoneAlert()
	{
		if (Utils.PhoneLevel == Utils.PhoneLevelEnum.LowEndPhone)
		{
			MessageBoxPanel.ShowConfirm(406);
		}
	}

	private void AfterInitCfgDoSomething()
	{
		PreLoadPartAb();
	}

	private void PreLoadPartAb()
	{
		List<BuildPart> allList = BuildPart.GetAllList();
		foreach (BuildPart item in allList)
		{
			if (item.model != null)
			{
				ResMgr.Ins.LoadAB(item.model, null, false);
			}
		}
	}
}
