using System;
using System.Collections.Generic;
using Share;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SmallScene
{
	public const int MaxBoundingSphereNum = 3000;

	private static readonly float[] DistancesArray = new float[1] { 100f };

	private string m_Name;

	private Octets m_Data;

	private readonly List<SceneObject> m_SceneObjects = new List<SceneObject>();

	private readonly BetterList<string> m_Prefabs = new BetterList<string>();

	private readonly CullingGroup m_CullingGroup;

	private readonly BoundingSphere[] m_BoundingSphereArray;

	private readonly List<SceneObject> m_GrassList = new List<SceneObject>();

	private bool m_WorkerRunning;

	private bool m_Loading;

	private bool m_Unloading;

	public Scene UnityScene;

	public int m_sceneLightmapOffet;

	public MeshRenderer m_terrainMeshRender;

	public SmallScene()
	{
		m_CullingGroup = new CullingGroup();
		m_BoundingSphereArray = new BoundingSphere[3000];
		m_CullingGroup.SetBoundingSpheres(m_BoundingSphereArray);
		m_CullingGroup.SetBoundingDistances(DistancesArray);
		m_CullingGroup.onStateChanged = OnGrassCullingStateChanged;
		m_CullingGroup.SetBoundingSphereCount(0);
	}

	public BetterList<string> ReadDependedABs(Octets oc)
	{
		m_Prefabs.Clear();
		m_sceneLightmapOffet = oc.pop_byte();
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
		if (m_Loading)
		{
			return;
		}
		m_Name = scene.name;
		m_Data = data;
		m_Loading = true;
		SmallSceneMgr.Ins.AddUpdateListener(Update);
		ExecuteInWorker(_DoLoad);
		if (!UnityScene.IsValid())
		{
			Debug.LogError(string.Format("[SmallScene]{0} UnityScene is valid OnLoaded. Init", m_Name));
			return;
		}
		try
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			GameObject[] array = rootGameObjects;
			foreach (GameObject gameObject in array)
			{
				if (gameObject.name == scene.name)
				{
					m_terrainMeshRender = gameObject.GetComponent<MeshRenderer>();
					break;
				}
			}
		}
		catch (Exception)
		{
			Debug.LogError("[SmallScene] Init get terrain error:" + scene.name);
		}
	}

	public void UnLoad(bool reset = false)
	{
		if (m_Unloading)
		{
			return;
		}
		m_Unloading = true;
		if (m_Loading)
		{
			return;
		}
		if (!reset)
		{
			int i = 0;
			for (int count = m_GrassList.Count; i < count; i++)
			{
				SceneObject sceneObject = m_GrassList[i];
				if (sceneObject.Holder != null)
				{
					sceneObject.RemoveFromScene(this);
				}
			}
			int j = 0;
			for (int count2 = m_SceneObjects.Count; j < count2; j++)
			{
				SceneObject sceneObject2 = m_SceneObjects[j];
				if (sceneObject2.Holder != null)
				{
					sceneObject2.RemoveFromScene(this);
				}
			}
		}
		m_CullingGroup.SetBoundingSphereCount(0);
		ExecuteInWorker(_DoUnLoad);
	}

	public void Destroy()
	{
		m_CullingGroup.Dispose();
	}

	private void OnLoaded()
	{
		if (!UnityScene.IsValid())
		{
			Debug.LogError(string.Format("[SmallScene]{0} UnityScene is valid OnLoaded.", m_Name));
			return;
		}
		m_Data = null;
		int num = Mathf.Min(m_GrassList.Count, 3000);
		for (int i = 0; i < num; i++)
		{
			SceneObject sceneObject = m_GrassList[i];
			BoundingSphere boundingSphere = m_BoundingSphereArray[i];
			boundingSphere.position = sceneObject.Pos;
			boundingSphere.radius = 15f;
			m_BoundingSphereArray[i] = boundingSphere;
		}
		m_CullingGroup.SetBoundingSphereCount(num);
		m_CullingGroup.targetCamera = Battle.Ins.MainCamera.Camera;
		m_CullingGroup.SetDistanceReferencePoint(Battle.Ins.MainCamera.SelfTransform);
		int j = 0;
		for (int count = m_SceneObjects.Count; j < count; j++)
		{
			SceneObject sceneObject2 = m_SceneObjects[j];
			sceneObject2.AddToScene(this);
		}
		if (SceneEvent.SceneLoadFinished != null)
		{
			SceneEvent.SceneLoadFinished(UnityScene);
		}
	}

	private void OnUnLoaded()
	{
		m_WorkerRunning = false;
		m_Loading = false;
		m_Unloading = false;
		SmallSceneMgr.Ins.RemoveUpdateListener(Update);
		SmallSceneMgr.Ins.ReturnScene(this);
	}

	private void ExecuteInWorker(Utils.VoidDelegate func)
	{
		m_WorkerRunning = true;
		Worker.Ins.Execute(func);
	}

	public void Update()
	{
		if (m_WorkerRunning)
		{
			return;
		}
		if (m_Loading)
		{
			m_Loading = false;
			if (!m_Unloading)
			{
				OnLoaded();
			}
			else
			{
				UnLoad();
			}
		}
		else if (m_Unloading)
		{
			m_Unloading = false;
			OnUnLoaded();
		}
	}

	private void _DoLoad()
	{
		int num = m_Data.pop_int();
		if (num > 3000)
		{
			Debug.LogError(string.Format("[SmallScene]grass count {0} exceed max {1}", num, 3000));
		}
		while (num-- > 0)
		{
			GrassObject grassObject = SmallSceneMgr.Ins.GrassObjectPool.Get();
			grassObject.Unmarshal(m_Data, m_Prefabs);
			m_GrassList.Add(grassObject);
		}
		num = m_Data.pop_int();
		while (num-- > 0)
		{
			TreeObject treeObject = SmallSceneMgr.Ins.TreeObjectPool.Get();
			treeObject.Unmarshal(m_Data, m_Prefabs);
			m_SceneObjects.Add(treeObject);
		}
		num = m_Data.pop_int();
		while (num-- > 0)
		{
			BuildingObject buildingObject = SmallSceneMgr.Ins.BuildingObjectPool.Get();
			buildingObject.sceneName = m_Name;
			buildingObject.Unmarshal(m_Data, m_Prefabs);
			m_SceneObjects.Add(buildingObject);
		}
		num = m_Data.pop_int();
		while (num-- > 0)
		{
			ThingsObject thingsObject = SmallSceneMgr.Ins.ThingsObjectPool.Get();
			thingsObject.sceneName = m_Name;
			thingsObject.UnmarshalNoLightmapInfo(m_Data, m_Prefabs);
			m_SceneObjects.Add(thingsObject);
		}
		num = m_Data.pop_int();
		while (num-- > 0)
		{
			ThingsObject thingsObject2 = SmallSceneMgr.Ins.ThingsObjectPool.Get();
			thingsObject2.sceneName = m_Name;
			thingsObject2.Unmarshal(m_Data, m_Prefabs);
			m_SceneObjects.Add(thingsObject2);
		}
		num = m_Data.pop_int();
		while (num-- > 0)
		{
			StaticGroupObject staticGroupObject = SmallSceneMgr.Ins.StaticGroupObjectPool.Get();
			staticGroupObject.sceneName = m_Name;
			staticGroupObject.Unmarshal(m_Data, m_Prefabs);
			m_SceneObjects.Add(staticGroupObject);
		}
		m_WorkerRunning = false;
	}

	private void _DoUnLoad()
	{
		m_Prefabs.Clear();
		int i = 0;
		for (int count = m_GrassList.Count; i < count; i++)
		{
			m_GrassList[i].Recycle();
		}
		int j = 0;
		for (int count2 = m_SceneObjects.Count; j < count2; j++)
		{
			m_SceneObjects[j].Recycle();
		}
		m_GrassList.Clear();
		m_SceneObjects.Clear();
		m_WorkerRunning = false;
	}

	public void OnGrassCullingStateChanged(CullingGroupEvent e)
	{
		SceneObject sceneObject = m_GrassList[e.index];
		if (e.isVisible)
		{
			sceneObject.AddToScene(this);
		}
		else
		{
			sceneObject.RemoveFromScene(this);
		}
	}
}
