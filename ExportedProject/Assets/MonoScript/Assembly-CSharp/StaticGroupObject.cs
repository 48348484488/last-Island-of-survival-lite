using System.Collections.Generic;
using Share;
using UnityEngine.SceneManagement;

public class StaticGroupObject : SceneObject
{
	private readonly List<ThingsObject> m_ChildObjects = new List<ThingsObject>();

	public override void Recycle()
	{
		Holder = null;
		int i = 0;
		for (int count = m_ChildObjects.Count; i < count; i++)
		{
			m_ChildObjects[i].Recycle();
		}
		m_ChildObjects.Clear();
		SmallSceneMgr.Ins.StaticGroupObjectPool.Recycle(this);
	}

	public override void AddToScene(SmallScene scene)
	{
		if (Holder != null)
		{
			return;
		}
		Holder = SmallSceneMgr.Ins.BorrowObject(PrefabName);
		if (Holder == null)
		{
			return;
		}
		Holder.Trans.position = Pos;
		Holder.Trans.eulerAngles = Angles;
		Holder.Trans.localScale = Scale;
		int i = 0;
		for (int count = m_ChildObjects.Count; i < count; i++)
		{
			ThingsObject thingsObject = m_ChildObjects[i];
			thingsObject.AddToScene(scene);
			if (thingsObject.Holder != null)
			{
				thingsObject.Holder.Trans.SetParent(Holder.Trans, true);
			}
		}
		Holder.BatchingGroup.enabled = true;
		SceneManager.MoveGameObjectToScene(Holder.Go, scene.UnityScene);
	}

	public override void RemoveFromScene(SmallScene scene)
	{
		if (Holder != null)
		{
			SceneManager.MoveGameObjectToScene(Holder.Go, SmallSceneMgr.Ins.BattleScene);
			int i = 0;
			for (int count = m_ChildObjects.Count; i < count; i++)
			{
				ThingsObject thingsObject = m_ChildObjects[i];
				thingsObject.RemoveFromScene(scene);
			}
			Holder.Trans.position = SceneObject.HidePos;
			SmallSceneMgr.Ins.ReturnObject(PrefabName, Holder);
			Holder.BatchingGroup.enabled = false;
			Holder = null;
		}
	}

	public override void Unmarshal(Octets oc, BetterList<string> prefabNames)
	{
		Pos.x = oc.pop_float();
		Pos.y = oc.pop_float();
		Pos.z = oc.pop_float();
		Scale.x = oc.pop_float();
		Scale.y = oc.pop_float();
		Scale.z = oc.pop_float();
		Angles.x = oc.pop_float();
		Angles.y = oc.pop_float();
		Angles.z = oc.pop_float();
		PrefabName = "staticpos";
		int num = oc.pop_int();
		while (num-- > 0)
		{
			ThingsObject thingsObject = SmallSceneMgr.Ins.ThingsObjectPool.Get();
			thingsObject.sceneName = sceneName;
			thingsObject.Unmarshal(oc, prefabNames);
			m_ChildObjects.Add(thingsObject);
		}
		num = oc.pop_int();
	}
}
