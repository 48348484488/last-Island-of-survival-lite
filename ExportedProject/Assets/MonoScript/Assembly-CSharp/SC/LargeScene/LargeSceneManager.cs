using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SC.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SC.LargeScene
{
	public class LargeSceneManager : MonoBehaviour
	{
		public int showSceneDistance = 300;

		public int maxSenceDistanceToLoad = 600;

		public int checkToPreloadsceneDistance = 2500;

		private List<SmallSceneInfo> checkHadSceneList;

		private List<SmallSceneInfo> toLoadSceneList;

		private Vector2 lastCaluPos;

		private Vector3 lastupdateCameraPos;

		private Transform mainCamera;

		private static SmallSceneList smallSceneList;

		public static LargeSceneManager Ins;

		public Dictionary<string, SmallSceneInfo> loadSceneList;

		private static bool bLoadSceneTexture;

		public static readonly float fbeginLoadSceneHeightSkydiving = 580f;

		private bool bToPos;

		private static SceneCellList sceneCellList;

		private int nFrameCount;

		private SceneCell cellPlayerOn;

		private Material m_dituDixing;

		private GameObject m_haidi;

		private GameObject m_haidiKongzhong;

		public static AssetBundle battlesceneAb;

		private Coroutine loadingCoroutine;

		private Utils.VoidDelegate onReloadingSceneCallBack;

		[CompilerGenerated]
		private static Utils.StringDelegate _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Utils.StringDelegate _003C_003Ef__mg_0024cache1;

		public int nHadLoadedSceneIndex { get; set; }

		public bool bLoadAllSceneing { get; set; }

		public int toInitLoadSceneCount { get; set; }

		public bool bUnloadAllScene { get; set; }

		public bool bInitFinish { get; set; }

		private void Awake()
		{
			Ins = this;
			bLoadAllSceneing = false;
			bInitFinish = false;
			bUnloadAllScene = false;
			loadSceneList = new Dictionary<string, SmallSceneInfo>();
			toLoadSceneList = new List<SmallSceneInfo>();
			checkHadSceneList = new List<SmallSceneInfo>();
		}

		private void Start()
		{
			BattleBegin(Battle.Ins.StartPos);
		}

		private void LoadDependAbCallback(int nHadFinish, int nTotalCount)
		{
			float arg = (float)nHadFinish / (float)(nTotalCount + 30);
			Utils.TriggerEvent(SceneEvent.LoadSceneProcess, arg);
			nHadLoadedSceneIndex = nHadFinish;
			if (nHadFinish == nTotalCount)
			{
				InitCellWhenBegin();
			}
		}

		private void InitCellWhenBegin()
		{
			if (mainCamera != null && mainCamera.position.y > fbeginLoadSceneHeightSkydiving)
			{
				return;
			}
			cellPlayerOn = sceneCellList.GetCellByPos(lastCaluPos.x, lastCaluPos.y);
			foreach (SmallSceneInfo item in cellPlayerOn.smallSceneInfo)
			{
				item.Load();
				toInitLoadSceneCount++;
			}
		}

		private void OnDestroy()
		{
			BattleEnd();
			Ins = null;
			if (battlesceneAb != null)
			{
				battlesceneAb.Unload(true);
				battlesceneAb = null;
			}
		}

		private IEnumerator CheckSceneToActive()
		{
			while (true)
			{
				ChageSceneAys();
				yield return new WaitForSeconds(0.5f);
			}
		}

		private void ChageSceneAys()
		{
			foreach (SmallSceneInfo toLoadScene in toLoadSceneList)
			{
				if (toLoadScene.asy != null && !toLoadScene.asy.allowSceneActivation && toLoadScene.asy.progress == 0.9f)
				{
					toLoadScene.asy.allowSceneActivation = true;
				}
			}
		}

		public static Coroutine LoadSceneInfoFile()
		{
			string filePathForWWW = Utils.GetFilePathForWWW(SmallSceneList.sceneInfoPath);
			if (_003C_003Ef__mg_0024cache0 == null)
			{
				_003C_003Ef__mg_0024cache0 = LoadSceneInfoFileComplete;
			}
			return FileOperation.LoadText(filePathForWWW, _003C_003Ef__mg_0024cache0);
		}

		public static Coroutine LoadSceneCellInfoFile()
		{
			string filePathForWWW = Utils.GetFilePathForWWW(SceneCellList.jsonFilePath);
			if (_003C_003Ef__mg_0024cache1 == null)
			{
				_003C_003Ef__mg_0024cache1 = LoadSceneCellFileComplete;
			}
			return FileOperation.LoadText(filePathForWWW, _003C_003Ef__mg_0024cache1);
		}

		private static void LoadSceneInfoFileComplete(string text)
		{
			try
			{
				smallSceneList = JsonUtility.FromJson<SmallSceneList>(text);
			}
			catch (Exception)
			{
				Debug.LogError("sceneinfo error, resetup game");
				Application.Quit();
			}
		}

		private static void LoadSceneCellFileComplete(string text)
		{
			try
			{
				sceneCellList = JsonUtility.FromJson<SceneCellList>(text);
				InitSceneCellIncludeSmallScene();
			}
			catch (Exception)
			{
				Debug.LogError("sceneinfo error, resetup game");
				Application.Quit();
			}
		}

		private static void InitSceneCellIncludeSmallScene()
		{
			foreach (SceneCell scene in sceneCellList.sceneList)
			{
				foreach (string smallScene in scene.smallSceneList)
				{
					SmallSceneInfo sceneByName = smallSceneList.GetSceneByName(smallScene);
					if (sceneByName != null)
					{
						scene.smallSceneInfo.Add(sceneByName);
					}
				}
			}
		}

		public void SmallSceneLoadedCallback(Scene scene)
		{
			if (scene.name == "mditu")
			{
				InitLoadSomeScene(Battle.Ins.StartPos);
			}
			else if (scene.name == "chushengdao_LOD0")
			{
				bLoadAllSceneing = false;
				Utils.TriggerEvent(SceneEvent.InitLoadSceneFinish);
			}
			else if (bLoadAllSceneing)
			{
				nHadLoadedSceneIndex++;
				if (SceneEvent.LoadSceneProcess != null && toInitLoadSceneCount != 0)
				{
					float arg = (float)nHadLoadedSceneIndex / (float)toInitLoadSceneCount;
					SceneEvent.LoadSceneProcess(arg);
				}
				if (nHadLoadedSceneIndex == toInitLoadSceneCount)
				{
					bLoadAllSceneing = false;
					Debug.LogError("scene load finish!!!");
					Utils.TriggerEvent(SceneEvent.InitLoadSceneFinish);
				}
			}
		}

		private void SceneLoadCompleteCallBack(Scene scene, LoadSceneMode mode)
		{
			string text = null;
			int newlodLevel = 0;
			text = scene.name;
			if (scene.name == "chushengdao_LOD0")
			{
				bLoadAllSceneing = false;
				Utils.TriggerEvent(SceneEvent.InitLoadSceneFinish);
				return;
			}
			SmallSceneInfo sceneByName = smallSceneList.GetSceneByName(text);
			if (sceneByName == null)
			{
				return;
			}
			if (bUnloadAllScene)
			{
				SceneManager.UnloadSceneAsync(scene);
				return;
			}
			sceneByName.asy = null;
			if (toLoadSceneList.Contains(sceneByName))
			{
				toLoadSceneList.Remove(sceneByName);
			}
			loadSceneList[sceneByName.scenename] = sceneByName;
			sceneByName.scene = scene;
			sceneByName.SceneLoadComplete(newlodLevel);
			if (!bLoadAllSceneing)
			{
				toLoadSceneList.Remove(sceneByName);
				if (toLoadSceneList.Count > 0)
				{
					toLoadSceneList[0].Load();
				}
			}
		}

		public void BattleBegin(Vector3 playerPos)
		{
			bToPos = false;
			SmallSceneInfo.LoadScene("mditu", null);
			ScenePreloadAb.Ins.abLoadCallback = LoadDependAbCallback;
			SceneManager.sceneLoaded += SceneLoadCompleteCallBack;
			SceneEvent.SceneLoadFinished = (SceneEvent.SceneCallBack)Delegate.Combine(SceneEvent.SceneLoadFinished, new SceneEvent.SceneCallBack(SmallSceneLoadedCallback));
			StartCoroutine(CheckSceneToActive());
			TreeBillboardMgr.Ins.Init();
			InstancingMgr.Ins.Init(Camera.main);
		}

		public void BattleEnd()
		{
			if (ScenePreloadAb.Ins.abLoadCallback != null)
			{
				ScenePreloadAb.Ins.abLoadCallback = null;
				toLoadSceneList.Clear();
				bInitFinish = false;
				UnloadAllScene();
				SceneManager.sceneLoaded -= SceneLoadCompleteCallBack;
				SceneEvent.SceneLoadFinished = (SceneEvent.SceneCallBack)Delegate.Remove(SceneEvent.SceneLoadFinished, new SceneEvent.SceneCallBack(SmallSceneLoadedCallback));
				StopAllCoroutines();
				TreeBillboardMgr.Ins.UnInit();
			}
		}

		public void UnloadScene(string strPath)
		{
			if (loadSceneList.ContainsKey(strPath))
			{
				loadSceneList.Remove(strPath);
			}
		}

		public void GetPreloadSceneList(List<SmallSceneInfo> sceneList)
		{
			SceneCell cellByPos = sceneCellList.GetCellByPos(mainCamera.transform.position.x, mainCamera.transform.position.z);
			foreach (SmallSceneInfo item in cellByPos.smallSceneInfo)
			{
				if (item.state == SmallSceneInfo.SceneState.scene_no_load && !sceneList.Contains(item))
				{
					sceneList.Add(item);
				}
			}
		}

		public void InitLoadSomeScene(Vector3 initpos)
		{
			if (smallSceneList.sceneList.Count != 0)
			{
				bUnloadAllScene = false;
				nHadLoadedSceneIndex = 0;
				bLoadAllSceneing = true;
				toInitLoadSceneCount = 0;
				lastCaluPos.x = initpos.x;
				lastCaluPos.y = initpos.z;
				if (!bLoadSceneTexture && ScenePreloadAb.Ins != null)
				{
					toInitLoadSceneCount = ScenePreloadAb.Ins.preloadAbList.Count;
					bLoadSceneTexture = true;
					ScenePreloadAb.Ins.LoadAllAbs();
				}
				else
				{
					InitCellWhenBegin();
				}
			}
		}

		public void UnloadAllScene()
		{
			bUnloadAllScene = true;
			foreach (SmallSceneInfo scene in smallSceneList.sceneList)
			{
				scene.UnLoad();
				scene.state = SmallSceneInfo.SceneState.scene_no_load;
			}
			loadSceneList.Clear();
		}

		private void Update()
		{
			InstancingMgr.Ins.Update();
		}

		private IEnumerator HideBattleLoading()
		{
			yield return Utils.WaitForSeconds(1f);
			Battle.Ins.SelfPlayer.SetGroundPos(lastupdateCameraPos);
			yield return Utils.WaitForSeconds(1f);
			ViewMgr.Ins.HideView<BattleLoadingPanel>();
			if (onReloadingSceneCallBack != null)
			{
				onReloadingSceneCallBack();
			}
			bToPos = false;
		}

		private void LateUpdate()
		{
			if (!bInitFinish)
			{
				return;
			}
			if (bToPos)
			{
				if (toLoadSceneList.Count <= 0 && loadingCoroutine == null)
				{
					loadingCoroutine = StartCoroutine(HideBattleLoading());
				}
				return;
			}
			if (!mainCamera && (bool)Battle.Ins.SelfPlayer)
			{
				mainCamera = Battle.Ins.SelfPlayer.gameObject.transform;
			}
			if ((bool)mainCamera)
			{
				nFrameCount++;
				if (NeedUpdate())
				{
					UpdateSceneCell();
					CheckHadloadScenes();
				}
			}
		}

		private void UpdateSceneCell()
		{
			SceneCell cellByPos = sceneCellList.GetCellByPos(lastupdateCameraPos.x, lastupdateCameraPos.z);
			if (cellByPos == cellPlayerOn)
			{
				return;
			}
			for (int num = toLoadSceneList.Count - 1; num >= 0; num--)
			{
				if (toLoadSceneList[num].state != SmallSceneInfo.SceneState.scene_loading)
				{
					toLoadSceneList.RemoveAt(num);
				}
			}
			foreach (SmallSceneInfo item in cellByPos.smallSceneInfo)
			{
				if (!toLoadSceneList.Contains(item) && !loadSceneList.ContainsKey(item.scenename))
				{
					toLoadSceneList.Add(item);
				}
			}
			foreach (SmallSceneInfo toLoadScene in toLoadSceneList)
			{
				toLoadScene.Load();
			}
			Debug.Log("this frame load scene count:" + toLoadSceneList.Count);
			cellPlayerOn = cellByPos;
		}

		private bool NeedUpdate()
		{
			if (mainCamera.position.y > fbeginLoadSceneHeightSkydiving)
			{
				return false;
			}
			if (nFrameCount % 90 == 0)
			{
				return true;
			}
			float sqrMagnitude = (lastupdateCameraPos - mainCamera.position).sqrMagnitude;
			if (sqrMagnitude > (float)checkToPreloadsceneDistance)
			{
				lastupdateCameraPos = mainCamera.position;
				return true;
			}
			return false;
		}

		private void CheckHadloadScenes()
		{
			checkHadSceneList.Clear();
			foreach (KeyValuePair<string, SmallSceneInfo> loadScene in loadSceneList)
			{
				checkHadSceneList.Add(loadScene.Value);
			}
			foreach (SmallSceneInfo checkHadScene in checkHadSceneList)
			{
				if (!cellPlayerOn.smallSceneInfo.Contains(checkHadScene))
				{
					SmallSceneMgr.Ins.BeforeSceneUnLoaded(checkHadScene.scenename);
					checkHadScene.UnLoad();
				}
			}
		}

		private void GPUIntanceUpdate(SmallSceneInfo smallScene, bool bAdd)
		{
		}

		public void ToSceneCell(float x, float y, float z, Utils.VoidDelegate callback = null)
		{
			lastupdateCameraPos.x = x;
			lastupdateCameraPos.y = y;
			lastupdateCameraPos.z = z;
			bool flag = false;
			SceneCell cellByPos = sceneCellList.GetCellByPos(x, z);
			if (cellByPos != cellPlayerOn)
			{
				foreach (SmallSceneInfo item in cellByPos.smallSceneInfo)
				{
					if (!toLoadSceneList.Contains(item) && !loadSceneList.ContainsKey(item.scenename))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				Vector3 position = new Vector3(lastupdateCameraPos.x, y, lastupdateCameraPos.z);
				Battle.Ins.SelfPlayer.SetPosition(position);
				if (callback != null)
				{
					callback();
				}
				Debug.Log("donot need show loading");
			}
			else
			{
				loadingCoroutine = null;
				bToPos = true;
				BattleLodingInfo battleLodingInfo = new BattleLodingInfo();
				battleLodingInfo.IsRebirth = true;
				ViewMgr.Ins.ShowTopView<BattleLoadingPanel>(battleLodingInfo);
				onReloadingSceneCallBack = callback;
				UpdateSceneCell();
			}
		}

		public void ChangeMdituShader(bool inPlane)
		{
			float value = 0.825f;
			float value2 = 0.938f;
			float value3 = 0.0009f;
			if (!inPlane)
			{
				value = 0.861f;
				value2 = 0.926f;
				value3 = 0.0019f;
			}
			m_dituDixing.SetFloat("_LerpMin", value);
			m_dituDixing.SetFloat("_LerpMax", value2);
			m_dituDixing.SetFloat("_FogFactor", value3);
		}
	}
}
