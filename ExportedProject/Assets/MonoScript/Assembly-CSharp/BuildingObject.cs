using Share;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildingObject : SceneObject
{
	private readonly BetterList<int> m_LightMapIndexList = new BetterList<int>();

	private readonly BetterList<Vector4> m_LightampScaleOffsetList = new BetterList<Vector4>();

	private int m_BuildingId;

	public override void Recycle()
	{
		Holder = null;
		m_LightMapIndexList.Clear();
		m_LightampScaleOffsetList.Clear();
		m_BuildingId = -1;
		SmallSceneMgr.Ins.BuildingObjectPool.Recycle(this);
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
		if (Holder.BuildingInfo != null)
		{
			Holder.BuildingInfo.id = m_BuildingId;
			Holder.BuildingInfo.enabled = true;
		}
		if (Holder.Lod != null)
		{
			Holder.Lod.enabled = true;
			MdituObject value;
			if (Holder.BuildingInfo != null && PrefabName != "building_001_dacangku" && SmallSceneMgr.Ins.dituScene.mdituObjectIndex.TryGetValue(m_BuildingId, out value))
			{
				if (Holder.Lod.Lod2Renderers == null || Holder.Lod.Lod2Renderers.Length != 1)
				{
					Holder.Lod.Lod2Renderers = new Renderer[1];
				}
				Holder.Lod.Lod2Renderers[0] = value.Lod3Render;
				Holder.Lod.Resetlod();
			}
			int num = Holder.Lod.RenderersWithLightmap.Length;
			if (num > 0 && num <= m_LightMapIndexList.size)
			{
				int num2 = scene.m_terrainMeshRender.lightmapIndex - scene.m_sceneLightmapOffet;
				for (int i = 0; i < num; i++)
				{
					Renderer renderer = Holder.Lod.RenderersWithLightmap[i];
					if (!(renderer == null))
					{
						renderer.lightmapIndex = m_LightMapIndexList[i] + num2;
						renderer.lightmapScaleOffset = m_LightampScaleOffsetList[i];
					}
				}
			}
		}
		else
		{
			Debug.LogError(string.Format("[BuildingObject]{0} not have SceneLod.", PrefabName));
		}
		Holder.Trans.position = Pos;
		Holder.Trans.eulerAngles = Angles;
		Holder.Trans.localScale = Scale;
		SceneManager.MoveGameObjectToScene(Holder.Go, scene.UnityScene);
	}

	public override void RemoveFromScene(SmallScene scene)
	{
		if (Holder == null)
		{
			return;
		}
		SceneManager.MoveGameObjectToScene(Holder.Go, SmallSceneMgr.Ins.BattleScene);
		if (Holder.Lod != null)
		{
			Holder.Lod.HideAll();
			Holder.Lod.enabled = false;
		}
		if (Holder.BuildingInfo != null)
		{
			Holder.BuildingInfo.enabled = false;
			MdituObject value;
			if (SmallSceneMgr.Ins.dituScene.mdituObjectIndex.TryGetValue(m_BuildingId, out value))
			{
				value.Lod3Render.enabled = true;
			}
		}
		Holder.Trans.position = SceneObject.HidePos;
		SmallSceneMgr.Ins.ReturnObject(PrefabName, Holder);
		Holder = null;
	}

	public override void Unmarshal(Octets oc, BetterList<string> prefabNames)
	{
		base.Unmarshal(oc, prefabNames);
		int num = oc.pop_int();
		Vector4 item = default(Vector4);
		while (num-- > 0)
		{
			m_LightMapIndexList.Add(oc.pop_byte());
			item.x = oc.pop_float();
			item.y = oc.pop_float();
			item.z = oc.pop_float();
			item.w = oc.pop_float();
			m_LightampScaleOffsetList.Add(item);
		}
		m_BuildingId = oc.pop_int();
	}
}
