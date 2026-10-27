using System.Collections.Generic;
using Share;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MdituScene
{
	private string m_Name;

	private Octets m_Data;

	private readonly BetterList<string> m_Prefabs = new BetterList<string>();

	public const int MaxBoundingSphereNum = 2000;

	public List<MdituObject> mdituAllObjectPool = new List<MdituObject>(2000);

	public Dictionary<int, MdituObject> mdituObjectIndex = new Dictionary<int, MdituObject>();

	private bool m_WorkerRunning;

	private bool m_Loading;

	private bool m_Unloading;

	public Scene UnityScene;

	private Dictionary<string, ScenePrefabPool> abPrefabList;

	public BetterList<string> ReadDependedABs(Octets oc)
	{
		m_Prefabs.Clear();
		byte b = oc.pop_byte();
		int num = oc.pop_int();
		while (num-- > 0)
		{
			m_Prefabs.Add(oc.pop_string().ToLower());
		}
		return m_Prefabs;
	}

	public BetterList<string> GetDependedABs()
	{
		return m_Prefabs;
	}

	public void Init(Scene scene, Octets data)
	{
		UnityScene = scene;
		if (!m_Loading)
		{
			m_Name = scene.name;
			m_Data = data;
			m_Loading = true;
			BetterList<string> abs = ReadDependedABs(m_Data);
			PrepareAllABOfScene(abs);
			_DoMdituLoad();
			OnLoaded();
		}
	}

	public void UnLoad(bool reset = false)
	{
	}

	public void Destroy()
	{
	}

	public void OnLoaded()
	{
		if (!UnityScene.IsValid())
		{
			Debug.LogError(string.Format("!!!!!!!!!!!!!!!!!!!![SmallScene]{0} UnityScene is valid OnLoaed.", m_Name));
			return;
		}
		m_Data = null;
		int i = 0;
		for (int count = mdituAllObjectPool.Count; i < count; i++)
		{
			MdituObject mdituObject = mdituAllObjectPool[i];
			mdituObject.AddToScene(this);
			mdituObjectIndex[mdituObject.nBuilderId] = mdituObject;
		}
		if (SceneEvent.SceneLoadFinished != null)
		{
			SceneEvent.SceneLoadFinished(UnityScene);
		}
	}

	public void OnUnLoaded()
	{
		int i = 0;
		for (int count = mdituAllObjectPool.Count; i < count; i++)
		{
			mdituAllObjectPool[i].Holder = null;
		}
	}

	private void ExecuteInWorker(Utils.VoidDelegate func)
	{
		m_WorkerRunning = true;
		Worker.Ins.Execute(func);
	}

	public void Update()
	{
	}

	private void _DoMdituLoad()
	{
		int num = m_Data.pop_int();
		while (num-- > 0)
		{
			MdituObject mdituObject = new MdituObject();
			mdituObject.sceneName = m_Name;
			mdituObject.UnmarshalNoLightmapInfo(m_Data, m_Prefabs);
			mdituAllObjectPool.Add(mdituObject);
		}
		m_WorkerRunning = false;
	}

	private void _DoUnLoad()
	{
		m_Prefabs.Clear();
		mdituAllObjectPool.Clear();
		mdituAllObjectPool.Capacity = 2000;
		m_WorkerRunning = false;
	}

	private void PrepareAllABOfScene(BetterList<string> abs)
	{
		abPrefabList = new Dictionary<string, ScenePrefabPool>();
		int i = 0;
		for (int size = abs.size; i < size; i++)
		{
			string text = abs[i];
			AssetBundle assetBundle = AssetBundle.LoadFromFile(Utils.GetFilePath("sceneprefab/" + text + ".ab"));
			if (assetBundle == null)
			{
				Debug.LogError("MdituScene load asset0 " + text + " failed.");
				continue;
			}
			Object @object = assetBundle.LoadAsset(text);
			if (@object == null)
			{
				Debug.LogError("MdituScene load asset1 " + text + " failed.");
				continue;
			}
			ScenePrefabPool scenePrefabPool = new ScenePrefabPool(text, @object as GameObject, null, assetBundle);
			scenePrefabPool.CanUnload = false;
			abPrefabList[text] = scenePrefabPool;
		}
	}

	public ObjectHolder BorrowObject(string prefabName)
	{
		ScenePrefabPool value;
		if (abPrefabList.TryGetValue(prefabName, out value))
		{
			return value.Get();
		}
		Debug.LogError("[MdituScene]BorrowObject failed." + prefabName);
		return null;
	}
}
