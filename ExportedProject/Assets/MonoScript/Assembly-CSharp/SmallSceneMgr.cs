using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Share;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SmallSceneMgr
{
	public static SmallSceneMgr Ins = new SmallSceneMgr();

	public Scene BattleScene;

	private Dictionary<string, SmallScene> m_Scenes;

	private HashSet<string> m_LoadingScenes;

	private HashSet<string> m_LoadedUnityScenes;

	private LRU<string, ScenePrefabPool> m_Pools;

	private HashSet<string> m_LoadingABs;

	private HashSet<AssetBundle> m_LoadingAssetABs;

	private HashSet<Coroutine> m_LoadingCoroutines;

	private LinkedList<ScenePrefabPool> m_DestroyingPools;

	private ObjectPool<SmallScene> m_ScenePools;

	public ObjectPool<GrassObject> GrassObjectPool;

	public ObjectPool<TreeObject> TreeObjectPool;

	public ObjectPool<ThingsObject> ThingsObjectPool;

	public ObjectPool<BuildingObject> BuildingObjectPool;

	public ObjectPool<StaticGroupObject> StaticGroupObjectPool;

	private Utils.VoidDelegate m_UpdateTrigger;

	private readonly List<SceneLod> m_Lods = new List<SceneLod>(1000);

	private readonly Dictionary<SceneLod, int> m_Lod2Index = new Dictionary<SceneLod, int>();

	private int m_LastUpdateIndex;

	public MdituScene dituScene;

	[CompilerGenerated]
	private static ObjectPool<SmallScene>.CreateObject<SmallScene> _003C_003Ef__am_0024cache0;

	[CompilerGenerated]
	private static ObjectPool<SmallScene>.DestroyObject<SmallScene> _003C_003Ef__am_0024cache1;

	[CompilerGenerated]
	private static ObjectPool<GrassObject>.CreateObject<GrassObject> _003C_003Ef__am_0024cache2;

	[CompilerGenerated]
	private static ObjectPool<GrassObject>.DestroyObject<GrassObject> _003C_003Ef__am_0024cache3;

	[CompilerGenerated]
	private static ObjectPool<TreeObject>.CreateObject<TreeObject> _003C_003Ef__am_0024cache4;

	[CompilerGenerated]
	private static ObjectPool<TreeObject>.DestroyObject<TreeObject> _003C_003Ef__am_0024cache5;

	[CompilerGenerated]
	private static ObjectPool<ThingsObject>.CreateObject<ThingsObject> _003C_003Ef__am_0024cache6;

	[CompilerGenerated]
	private static ObjectPool<ThingsObject>.DestroyObject<ThingsObject> _003C_003Ef__am_0024cache7;

	[CompilerGenerated]
	private static ObjectPool<BuildingObject>.CreateObject<BuildingObject> _003C_003Ef__am_0024cache8;

	[CompilerGenerated]
	private static ObjectPool<BuildingObject>.DestroyObject<BuildingObject> _003C_003Ef__am_0024cache9;

	[CompilerGenerated]
	private static ObjectPool<StaticGroupObject>.CreateObject<StaticGroupObject> _003C_003Ef__am_0024cacheA;

	[CompilerGenerated]
	private static ObjectPool<StaticGroupObject>.DestroyObject<StaticGroupObject> _003C_003Ef__am_0024cacheB;

	private SmallSceneMgr()
	{
	}

	public void Init()
	{
		m_Scenes = new Dictionary<string, SmallScene>();
		m_LoadingScenes = new HashSet<string>();
		m_LoadedUnityScenes = new HashSet<string>();
		int capacity = ((SystemInfo.systemMemorySize < 2000) ? 60 : ((SystemInfo.systemMemorySize >= 3000) ? 300 : 150));
		m_Pools = new LRU<string, ScenePrefabPool>(capacity);
		m_Pools.onRemoveEntry = OnPoolCacheOverflow;
		m_LoadingABs = new HashSet<string>();
		m_LoadingAssetABs = new HashSet<AssetBundle>();
		m_LoadingCoroutines = new HashSet<Coroutine>();
		m_DestroyingPools = new LinkedList<ScenePrefabPool>();
		if (_003C_003Ef__am_0024cache0 == null)
		{
			_003C_003Ef__am_0024cache0 = _003CInit_003Em__0;
		}
		ObjectPool<SmallScene>.CreateObject<SmallScene> createFun = _003C_003Ef__am_0024cache0;
		if (_003C_003Ef__am_0024cache1 == null)
		{
			_003C_003Ef__am_0024cache1 = _003CInit_003Em__1;
		}
		m_ScenePools = new ObjectPool<SmallScene>(16, createFun, _003C_003Ef__am_0024cache1);
		if (_003C_003Ef__am_0024cache2 == null)
		{
			_003C_003Ef__am_0024cache2 = _003CInit_003Em__2;
		}
		ObjectPool<GrassObject>.CreateObject<GrassObject> createFun2 = _003C_003Ef__am_0024cache2;
		if (_003C_003Ef__am_0024cache3 == null)
		{
			_003C_003Ef__am_0024cache3 = _003CInit_003Em__3;
		}
		GrassObjectPool = new ObjectPool<GrassObject>(20000, createFun2, _003C_003Ef__am_0024cache3);
		if (_003C_003Ef__am_0024cache4 == null)
		{
			_003C_003Ef__am_0024cache4 = _003CInit_003Em__4;
		}
		ObjectPool<TreeObject>.CreateObject<TreeObject> createFun3 = _003C_003Ef__am_0024cache4;
		if (_003C_003Ef__am_0024cache5 == null)
		{
			_003C_003Ef__am_0024cache5 = _003CInit_003Em__5;
		}
		TreeObjectPool = new ObjectPool<TreeObject>(1000, createFun3, _003C_003Ef__am_0024cache5);
		if (_003C_003Ef__am_0024cache6 == null)
		{
			_003C_003Ef__am_0024cache6 = _003CInit_003Em__6;
		}
		ObjectPool<ThingsObject>.CreateObject<ThingsObject> createFun4 = _003C_003Ef__am_0024cache6;
		if (_003C_003Ef__am_0024cache7 == null)
		{
			_003C_003Ef__am_0024cache7 = _003CInit_003Em__7;
		}
		ThingsObjectPool = new ObjectPool<ThingsObject>(3000, createFun4, _003C_003Ef__am_0024cache7);
		if (_003C_003Ef__am_0024cache8 == null)
		{
			_003C_003Ef__am_0024cache8 = _003CInit_003Em__8;
		}
		ObjectPool<BuildingObject>.CreateObject<BuildingObject> createFun5 = _003C_003Ef__am_0024cache8;
		if (_003C_003Ef__am_0024cache9 == null)
		{
			_003C_003Ef__am_0024cache9 = _003CInit_003Em__9;
		}
		BuildingObjectPool = new ObjectPool<BuildingObject>(1000, createFun5, _003C_003Ef__am_0024cache9);
		if (_003C_003Ef__am_0024cacheA == null)
		{
			_003C_003Ef__am_0024cacheA = _003CInit_003Em__A;
		}
		ObjectPool<StaticGroupObject>.CreateObject<StaticGroupObject> createFun6 = _003C_003Ef__am_0024cacheA;
		if (_003C_003Ef__am_0024cacheB == null)
		{
			_003C_003Ef__am_0024cacheB = _003CInit_003Em__B;
		}
		StaticGroupObjectPool = new ObjectPool<StaticGroupObject>(200, createFun6, _003C_003Ef__am_0024cacheB);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public void AddUpdateListener(Utils.VoidDelegate func)
	{
		m_UpdateTrigger = (Utils.VoidDelegate)Delegate.Combine(m_UpdateTrigger, func);
	}

	public void RemoveUpdateListener(Utils.VoidDelegate func)
	{
		m_UpdateTrigger = (Utils.VoidDelegate)Delegate.Remove(m_UpdateTrigger, func);
	}

	public void RegisSceneLod(SceneLod lod)
	{
		if (!m_Lod2Index.ContainsKey(lod))
		{
			m_Lods.Add(lod);
			m_Lod2Index[lod] = m_Lods.Count - 1;
		}
	}

	public void UnRegisSceneLod(SceneLod lod)
	{
		int value;
		if (m_Lod2Index.TryGetValue(lod, out value))
		{
			int index = m_Lods.Count - 1;
			SceneLod sceneLod = m_Lods[index];
			m_Lods[value] = sceneLod;
			m_Lod2Index[sceneLod] = value;
			m_Lods.RemoveAt(index);
			m_Lod2Index.Remove(lod);
		}
	}

	private void UpdateLod()
	{
		if (m_LastUpdateIndex >= m_Lods.Count)
		{
			m_LastUpdateIndex = 0;
		}
		int lastUpdateIndex = m_LastUpdateIndex;
		int num = m_Lods.Count / 30 + 1;
		int num2 = lastUpdateIndex + num;
		if (num2 >= m_Lods.Count)
		{
			num2 = m_Lods.Count;
			m_LastUpdateIndex = 0;
		}
		else
		{
			m_LastUpdateIndex = num2;
		}
		for (int i = lastUpdateIndex; i < num2; i++)
		{
			m_Lods[i].DoCheckLod();
		}
	}

	public void ReturnScene(SmallScene scene)
	{
		m_ScenePools.Recycle(scene);
	}

	public void Clear()
	{
		foreach (Coroutine loadingCoroutine in m_LoadingCoroutines)
		{
			Utils.StopConroutine(loadingCoroutine);
		}
		m_LoadingCoroutines.Clear();
		foreach (KeyValuePair<string, SmallScene> scene in m_Scenes)
		{
			scene.Value.UnLoad(true);
		}
		m_Scenes.Clear();
		m_LoadingScenes.Clear();
		m_LoadedUnityScenes.Clear();
		List<ScenePrefabPool> values = m_Pools.GetValues();
		foreach (ScenePrefabPool item in values)
		{
			item.Destroy();
		}
		m_Pools.Clear();
		foreach (ScenePrefabPool destroyingPool in m_DestroyingPools)
		{
			destroyingPool.Destroy();
		}
		m_DestroyingPools.Clear();
		m_LoadingABs.Clear();
		foreach (AssetBundle loadingAssetAB in m_LoadingAssetABs)
		{
			loadingAssetAB.Unload(true);
		}
		m_LoadingAssetABs.Clear();
		m_LastUpdateIndex = 0;
		m_Lods.Clear();
		m_Lod2Index.Clear();
		dituScene.OnUnLoaded();
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		string name = scene.name;
		switch (name)
		{
		case "empty":
			return;
		case "movie":
			return;
		case "battle":
		{
			BattleScene = scene;
			GameObject gameObject = new GameObject("staticpos");
			Transform transform = gameObject.transform;
			transform.position = Vector3.zero;
			transform.localScale = Vector3.one;
			transform.rotation = Quaternion.identity;
			gameObject.AddComponent<StaticBatchingGroup>().enabled = false;
			ScenePrefabPool scenePrefabPool = new ScenePrefabPool("staticpos", gameObject, null, null);
			scenePrefabPool.CanUnload = false;
			m_Pools.Set("staticpos", scenePrefabPool);
			return;
		}
		}
		if (Battle.Ins == null)
		{
			return;
		}
		if (name == "mditu")
		{
			if (dituScene != null)
			{
				dituScene.UnityScene = scene;
				dituScene.OnLoaded();
			}
			else
			{
				Utils.StartConroutine(LoadMdituSceneData(scene));
			}
			return;
		}
		m_LoadedUnityScenes.Add(name);
		if (!m_Scenes.ContainsKey(name) && !m_LoadingScenes.Contains(name))
		{
			m_LoadingScenes.Add(name);
			m_LoadingCoroutines.Add(Utils.StartConroutine(LoadSceneData(scene)));
		}
	}

	public void BeforeSceneUnLoaded(string sceneName)
	{
		m_LoadedUnityScenes.Remove(sceneName);
		SmallScene value;
		if (!m_Scenes.TryGetValue(sceneName, out value))
		{
			Debug.LogError("[SmallSceneMgr] unload oc, scene not load finish:" + sceneName);
			return;
		}
		value.UnLoad();
		m_Scenes.Remove(sceneName);
		ReduceABRefCount(value.GetDependedABs());
	}

	private IEnumerator LoadSceneData(Scene scene)
	{
		string sceneName = scene.name;
		string ocName2 = string.Empty;
		ocName2 = "sceneinfo/" + sceneName + ".oc";
		WWW www = new WWW(Utils.GetFilePathForWWW(ocName2));
		yield return www;
		if (!m_LoadedUnityScenes.Contains(sceneName))
		{
			m_LoadingScenes.Remove(sceneName);
			www.Dispose();
			yield break;
		}
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogError(string.Format("[SmallSceneMgr]{0} LoadSceneData failed:{1}", sceneName, www.error));
			m_LoadingScenes.Remove(sceneName);
			yield break;
		}
		SmallScene smallScene = m_ScenePools.Get();
		Octets oc = new Octets(www.bytes, www.bytes.Length);
		BetterList<string> abs = smallScene.ReadDependedABs(oc);
		yield return PrepareAllABOfScene(abs);
		if (!m_LoadedUnityScenes.Contains(sceneName))
		{
			ReduceABRefCount(smallScene.GetDependedABs());
			m_LoadingScenes.Remove(sceneName);
			m_ScenePools.Recycle(smallScene);
			www.Dispose();
		}
		else
		{
			m_Scenes[sceneName] = smallScene;
			m_LoadingScenes.Remove(sceneName);
			smallScene.Init(scene, oc);
			www.Dispose();
		}
	}

	private IEnumerator LoadMdituSceneData(Scene scene)
	{
		string sceneName = scene.name;
		WWW www = new WWW(Utils.GetFilePathForWWW("sceneinfo/" + sceneName + ".oc"));
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			Debug.LogError(string.Format("[SmallSceneMgr]{0} LoadSceneData failed:{1}", sceneName, www.error));
			yield break;
		}
		dituScene = new MdituScene();
		Octets oc = new Octets(www.bytes, www.bytes.Length);
		dituScene.Init(scene, oc);
		www.Dispose();
	}

	public ObjectHolder GetProtype(string prefabName)
	{
		ScenePrefabPool value;
		if (m_Pools.TryGetValue(prefabName, out value))
		{
			return value.GetProtype();
		}
		Debug.LogError("[SmallSceneMgr]GetProtype failed." + prefabName);
		return null;
	}

	public ObjectHolder BorrowObject(string prefabName)
	{
		ScenePrefabPool value;
		if (m_Pools.TryGetValue(prefabName, out value))
		{
			return value.Get();
		}
		Debug.LogError("[SmallSceneMgr]BorrowObject failed." + prefabName);
		return null;
	}

	public void ReturnObject(string prefabName, ObjectHolder obj)
	{
		ScenePrefabPool value;
		if (m_Pools.TryGetValue(prefabName, out value))
		{
			value.Recycle(obj);
		}
		else
		{
			obj.Destroy();
		}
	}

	private IEnumerator PrepareAllABOfScene(BetterList<string> abs)
	{
		int i = 0;
		for (int size = abs.size; i < size; i++)
		{
			string ab = abs[i];
			while (m_LoadingABs.Contains(ab))
			{
				yield return null;
			}
			ScenePrefabPool pool;
			if (m_Pools.TryGetValue(ab, out pool))
			{
				pool.RefCount++;
				continue;
			}
			pool = RemovePrefabPoolFromDestroying(ab);
			if (pool != null)
			{
				pool.RefCount++;
				m_Pools.Set(ab, pool);
				continue;
			}
			m_LoadingABs.Add(ab);
			AssetBundleCreateRequest abRequest = AssetBundle.LoadFromFileAsync(Utils.GetFilePath("sceneprefab/" + ab + ".ab"));
			yield return abRequest;
			if (abRequest.assetBundle == null)
			{
				Debug.LogError("SmallSceneMgr load " + ab + " failed.");
				m_LoadingABs.Remove(ab);
				continue;
			}
			m_LoadingAssetABs.Add(abRequest.assetBundle);
			AssetBundleRequest assetRequest = abRequest.assetBundle.LoadAssetAsync(ab);
			yield return assetRequest;
			m_LoadingAssetABs.Remove(abRequest.assetBundle);
			if (assetRequest.asset == null)
			{
				m_LoadingABs.Remove(ab);
				Debug.LogError("SmallSceneMgr load asset " + ab + " failed.");
				abRequest.assetBundle.Unload(true);
			}
			else if (!m_LoadingABs.Contains(ab))
			{
				abRequest.assetBundle.Unload(true);
			}
			else
			{
				m_LoadingABs.Remove(ab);
				pool = new ScenePrefabPool(ab, assetRequest.asset as GameObject, null, abRequest.assetBundle)
				{
					CanUnload = true,
					RefCount = 1
				};
				m_Pools.Set(ab, pool);
			}
		}
	}

	public void ReduceABRefCount(BetterList<string> abs)
	{
		int i = 0;
		for (int size = abs.size; i < size; i++)
		{
			ScenePrefabPool value;
			if (m_Pools.TryGetValue(abs[i], out value))
			{
				value.RefCount--;
			}
		}
	}

	private ScenePrefabPool RemovePrefabPoolFromDestroying(string abName)
	{
		foreach (ScenePrefabPool destroyingPool in m_DestroyingPools)
		{
			if (destroyingPool.ABName == abName)
			{
				m_DestroyingPools.Remove(destroyingPool);
				return destroyingPool;
			}
		}
		return null;
	}

	private bool OnPoolCacheOverflow(string name, ScenePrefabPool pool)
	{
		if (!pool.CanUnload)
		{
			return false;
		}
		if (pool.RefCount > 0)
		{
			return false;
		}
		if (m_LoadingABs.Contains(name))
		{
			return false;
		}
		m_DestroyingPools.AddLast(pool);
		return true;
	}

	public void Update()
	{
		if (m_UpdateTrigger != null)
		{
			m_UpdateTrigger();
		}
		if (m_DestroyingPools.Count > 0)
		{
			ScenePrefabPool value = m_DestroyingPools.First.Value;
			if (value.countInactive == 0)
			{
				m_DestroyingPools.RemoveFirst();
				value.Destroy();
				return;
			}
			value.DestroySome(3);
		}
		if (m_Lods.Count > 0)
		{
			UpdateLod();
		}
	}

	[CompilerGenerated]
	private static SmallScene _003CInit_003Em__0()
	{
		return new SmallScene();
	}

	[CompilerGenerated]
	private static void _003CInit_003Em__1(SmallScene scene)
	{
		scene.Destroy();
	}

	[CompilerGenerated]
	private static GrassObject _003CInit_003Em__2()
	{
		return new GrassObject();
	}

	[CompilerGenerated]
	private static void _003CInit_003Em__3(GrassObject o)
	{
	}

	[CompilerGenerated]
	private static TreeObject _003CInit_003Em__4()
	{
		return new TreeObject();
	}

	[CompilerGenerated]
	private static void _003CInit_003Em__5(TreeObject o)
	{
	}

	[CompilerGenerated]
	private static ThingsObject _003CInit_003Em__6()
	{
		return new ThingsObject();
	}

	[CompilerGenerated]
	private static void _003CInit_003Em__7(ThingsObject o)
	{
	}

	[CompilerGenerated]
	private static BuildingObject _003CInit_003Em__8()
	{
		return new BuildingObject();
	}

	[CompilerGenerated]
	private static void _003CInit_003Em__9(BuildingObject o)
	{
	}

	[CompilerGenerated]
	private static StaticGroupObject _003CInit_003Em__A()
	{
		return new StaticGroupObject();
	}

	[CompilerGenerated]
	private static void _003CInit_003Em__B(StaticGroupObject o)
	{
	}
}
