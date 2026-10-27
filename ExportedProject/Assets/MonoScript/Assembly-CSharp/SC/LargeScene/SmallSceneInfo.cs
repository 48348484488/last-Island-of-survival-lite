using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SC.LargeScene
{
	[Serializable]
	public class SmallSceneInfo
	{
		public enum SceneState
		{
			scene_no_load = 0,
			scene_loading = 1,
			scene_loaded = 2
		}

		public string scenename;

		private AssetBundle sceneAb;

		private Vector2 posToCalu;

		public AsyncOperation asy;

		public Scene scene { get; set; }

		public SceneState state { get; set; }

		public SmallSceneInfo()
		{
			Init();
		}

		private void Init()
		{
			state = SceneState.scene_no_load;
		}

		public void Load()
		{
			if (state == SceneState.scene_no_load)
			{
				state = SceneState.scene_loading;
				LoadScene(scenename, this);
			}
		}

		public void SceneLoadComplete(int newlodLevel)
		{
			state = SceneState.scene_loaded;
		}

		public void UnLoad()
		{
			if (state == SceneState.scene_loaded)
			{
				SceneManager.UnloadSceneAsync(scenename);
				LargeSceneManager.Ins.UnloadScene(scenename);
				if (sceneAb != null)
				{
					sceneAb.Unload(true);
					sceneAb = null;
				}
				state = SceneState.scene_no_load;
			}
		}

		public static void LoadScene(string fileName, SmallSceneInfo sceneInfo)
		{
			string filePath = Utils.GetFilePath("scene/" + fileName.ToLower() + ".ab");
			Utils.StartConroutine(LoadSceneAb(filePath, fileName, sceneInfo));
		}

		private static IEnumerator LoadSceneAb(string path, string sceneName, SmallSceneInfo sceneInfo)
		{
			if (sceneInfo != null && sceneInfo.sceneAb != null)
			{
				sceneInfo.sceneAb.Unload(true);
				sceneInfo.sceneAb = null;
			}
			AssetBundleCreateRequest assetRequest = AssetBundle.LoadFromFileAsync(path);
			yield return assetRequest;
			if (assetRequest.isDone && assetRequest.assetBundle != null)
			{
				AssetBundle ab = assetRequest.assetBundle;
				AsyncOperation asy = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
				if (sceneInfo != null)
				{
					sceneInfo.asy = asy;
					if (LargeSceneManager.Ins != null && LargeSceneManager.Ins.bInitFinish)
					{
						asy.allowSceneActivation = false;
					}
				}
				yield return asy;
				if (sceneInfo != null)
				{
					sceneInfo.sceneAb = assetRequest.assetBundle;
					if (sceneInfo.sceneAb == null)
					{
						Debug.LogError(sceneName + " load ab is null");
					}
				}
				else
				{
					assetRequest.assetBundle.Unload(false);
				}
			}
			else
			{
				Debug.Log("load scene error:" + sceneName);
			}
		}

		public void LoadSceneSync(string fileName)
		{
			string filePath = Utils.GetFilePath("scene/" + fileName.ToLower() + ".ab");
			AssetBundle assetBundle = AssetBundle.LoadFromFile(filePath);
			SceneManager.LoadScene(fileName, LoadSceneMode.Additive);
			state = SceneState.scene_loaded;
			sceneAb = assetBundle;
		}

		public static AssetBundle LoadSceneSync(string fileName, LoadSceneMode mode)
		{
			string filePath = Utils.GetFilePath("scene/" + fileName.ToLower() + ".ab");
			AssetBundle result = AssetBundle.LoadFromFile(filePath);
			SceneManager.LoadScene(fileName, mode);
			return result;
		}
	}
}
