using Share;
using UnityEngine;

public class MdituObject : SceneObject
{
	private Renderer m_Lod3Renderer;

	public int nBuilderId;

	public Renderer Lod3Render
	{
		get
		{
			if (m_Lod3Renderer == null)
			{
				m_Lod3Renderer = Holder.Go.GetComponentInChildren<MeshRenderer>();
			}
			return m_Lod3Renderer;
		}
	}

	public override void Recycle()
	{
		Holder = null;
	}

	public override void AddToScene(SmallScene scene)
	{
		if (Holder == null)
		{
			Holder = SmallSceneMgr.Ins.dituScene.BorrowObject(PrefabName);
			if (Holder != null)
			{
				Holder.Trans.position = Pos;
				Holder.Trans.eulerAngles = Angles;
				Holder.Trans.localScale = Scale;
			}
		}
	}

	public void AddToScene(MdituScene scene)
	{
		if (Holder == null)
		{
			Holder = SmallSceneMgr.Ins.dituScene.BorrowObject(PrefabName);
			if (Holder != null)
			{
				Holder.Trans.position = Pos;
				Holder.Trans.eulerAngles = Angles;
				Holder.Trans.localScale = Scale;
			}
		}
	}

	public override void RemoveFromScene(SmallScene scene)
	{
	}

	public override void Unmarshal(Octets oc, BetterList<string> prefabNames)
	{
		base.Unmarshal(oc, prefabNames);
	}

	public void UnmarshalNoLightmapInfo(Octets oc, BetterList<string> prefabNames)
	{
		base.Unmarshal(oc, prefabNames);
		nBuilderId = oc.pop_int();
	}
}
