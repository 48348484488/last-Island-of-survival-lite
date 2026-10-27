using Share;
using UnityEngine;

public class ThingsObject : SceneObject
{
	public int LightmapIndex = -1;

	public Vector4 LightmapScaleOffset;

	public override void Recycle()
	{
		Holder = null;
		SmallSceneMgr.Ins.ThingsObjectPool.Recycle(this);
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
		if (Holder.Renderer != null)
		{
			if (Holder.Lod == null)
			{
				Holder.Renderer.enabled = true;
			}
			else
			{
				Holder.Lod.enabled = true;
			}
			if (LightmapIndex >= 0 && LightmapIndex < 255)
			{
				int num = scene.m_terrainMeshRender.lightmapIndex - scene.m_sceneLightmapOffet;
				Holder.Renderer.lightmapIndex = LightmapIndex + num;
				Holder.Renderer.lightmapScaleOffset = LightmapScaleOffset;
			}
		}
	}

	public override void RemoveFromScene(SmallScene scene)
	{
		if (Holder != null)
		{
			Holder.Trans.parent = null;
			Holder.Trans.position = SceneObject.HidePos;
			if (Holder.Renderer != null)
			{
				Holder.Renderer.enabled = false;
			}
			if (PrefabName == "project_007_shitou" || PrefabName == "project_002_haibianshitou")
			{
				Holder.Lod.enabled = false;
			}
			SmallSceneMgr.Ins.ReturnObject(PrefabName, Holder);
			Holder = null;
		}
	}

	public override void Unmarshal(Octets oc, BetterList<string> prefabNames)
	{
		base.Unmarshal(oc, prefabNames);
		LightmapIndex = oc.pop_byte();
		LightmapScaleOffset.x = oc.pop_float();
		LightmapScaleOffset.y = oc.pop_float();
		LightmapScaleOffset.z = oc.pop_float();
		LightmapScaleOffset.w = oc.pop_float();
	}

	public void UnmarshalNoLightmapInfo(Octets oc, BetterList<string> prefabNames)
	{
		base.Unmarshal(oc, prefabNames);
		LightmapIndex = -1;
	}
}
