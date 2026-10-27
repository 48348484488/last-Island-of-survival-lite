using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class ScenePreloadAb
{
	public delegate void ABCallback(string strAbName, AssetBundle ab);

	public Utils.Int2Delegate abLoadCallback;

	public List<string> preloadAbList;

	public static string path = "gameconfig/preabs.obj";

	public static string partprepath = "gameconfig/partpreabs.obj";

	public static ScenePreloadAb Ins;

	private bool bLoading;

	private bool bLoadAll;

	private Dictionary<string, AssetBundle> sceneAllPrefabAbs;

	public Material grass_material;

	public Material grass_dark_material;

	public Material xiaomai_material;

	public Material xiaomai_dark_material;

	public static bool bVivoAlphaBlend;

	public static float grass_lightbright_dark = 0.8f;

	public static bool showDarkGrass;

	[CompilerGenerated]
	private static Utils.StringDelegate _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Utils.StringDelegate _003C_003Ef__mg_0024cache1;

	public int nLoadIndex { get; set; }

	public ScenePreloadAb()
	{
		nLoadIndex = 0;
		preloadAbList = new List<string>();
		sceneAllPrefabAbs = new Dictionary<string, AssetBundle>();
		string deviceModel = SystemInfo.deviceModel;
		if (deviceModel.StartsWith("vivo", StringComparison.OrdinalIgnoreCase) && SystemInfo.systemMemorySize < 3072)
		{
			bVivoAlphaBlend = true;
		}
	}

	public static Coroutine LoadPreloadAbInfoFile()
	{
		string filePathForWWW = Utils.GetFilePathForWWW(path);
		if (_003C_003Ef__mg_0024cache0 == null)
		{
			_003C_003Ef__mg_0024cache0 = LoadInfoFileComplete;
		}
		return FileOperation.LoadText(filePathForWWW, _003C_003Ef__mg_0024cache0);
	}

	private static void LoadInfoFileComplete(string text)
	{
		if (Ins != null)
		{
			return;
		}
		try
		{
			Ins = JsonUtility.FromJson<ScenePreloadAb>(text);
		}
		catch (Exception)
		{
		}
	}

	public static Coroutine LoadPartsPreloadAbInfoFile()
	{
		string filePathForWWW = Utils.GetFilePathForWWW(partprepath);
		if (_003C_003Ef__mg_0024cache1 == null)
		{
			_003C_003Ef__mg_0024cache1 = LoadPartsInfoFileComplete;
		}
		return FileOperation.LoadText(filePathForWWW, _003C_003Ef__mg_0024cache1);
	}

	private static void LoadPartsInfoFileComplete(string text)
	{
		try
		{
			ScenePreloadAb scenePreloadAb = JsonUtility.FromJson<ScenePreloadAb>(text);
			Ins.preloadAbList.AddRange(scenePreloadAb.preloadAbList);
		}
		catch (Exception)
		{
		}
	}

	public void ToLoadAbOnebyone()
	{
		if (!bLoading && !bLoadAll && nLoadIndex < preloadAbList.Count)
		{
			bLoading = true;
			Utils.StartConroutine(LoadSceneUseAb(preloadAbList[nLoadIndex], LoadSceneDependAb));
		}
	}

	private void LoadSceneDependAb(string strAbName, AssetBundle ab)
	{
		string fileName = FileOperation.GetFileName(strAbName);
		sceneAllPrefabAbs[fileName] = ab;
		bLoading = false;
		nLoadIndex++;
		if (abLoadCallback != null)
		{
			abLoadCallback(nLoadIndex, preloadAbList.Count);
		}
	}

	public void LoadAllAbs()
	{
		bLoadAll = true;
		if (nLoadIndex >= preloadAbList.Count && abLoadCallback != null)
		{
			abLoadCallback(nLoadIndex, preloadAbList.Count);
		}
		for (int i = nLoadIndex; i < preloadAbList.Count; i++)
		{
			Utils.StartConroutine(LoadSceneUseAb(preloadAbList[i], LoadSceneDependAb));
		}
	}

	private IEnumerator LoadSceneUseAb(string abPath, ABCallback callback)
	{
		AssetBundleCreateRequest assetRequest = AssetBundle.LoadFromFileAsync(Utils.GetFilePath(abPath));
		yield return assetRequest;
		if (callback != null)
		{
			callback(abPath, assetRequest.assetBundle);
		}
		if (!assetRequest.isDone || !(assetRequest.assetBundle != null))
		{
			Debug.LogError("ScenePreloadAb load " + abPath + " failed.");
		}
	}

	public AssetBundle GetScenePrefabAb(string strKey)
	{
		AssetBundle value;
		if (sceneAllPrefabAbs.TryGetValue(strKey, out value))
		{
			return value;
		}
		return null;
	}
}
