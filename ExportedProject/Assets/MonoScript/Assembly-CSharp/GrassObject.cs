using Share;
using UnityEngine;

public class GrassObject : SceneObject
{
	private float m_Brightness = 1f;

	private long m_instancingId;

	public override void Recycle()
	{
		Holder = null;
		SmallSceneMgr.Ins.GrassObjectPool.Recycle(this);
	}

	public override void AddToScene(SmallScene scene)
	{
		if (!InstancingMgr.Ins.IsSupport)
		{
			if (Holder == null)
			{
				Holder = SmallSceneMgr.Ins.BorrowObject(PrefabName);
				if (Holder != null)
				{
					Holder.Trans.position = Pos;
					Holder.Trans.eulerAngles = Angles;
					Holder.Trans.localScale = Scale;
					Holder.Renderer.enabled = true;
				}
			}
		}
		else
		{
			if (m_instancingId > 0)
			{
				return;
			}
			if (Holder == null)
			{
				Holder = SmallSceneMgr.Ins.GetProtype(PrefabName);
				if (Holder == null)
				{
					return;
				}
			}
			m_instancingId = InstancingMgr.Ins.AddInstancingObj(Holder.Mesh, Holder.Renderer.sharedMaterial, Matrix4x4.TRS(Pos, Quaternion.Euler(Angles), Scale), false, true);
		}
	}

	public override void RemoveFromScene(SmallScene scene)
	{
		if (InstancingMgr.Ins.IsSupport)
		{
			InstancingMgr.Ins.removeInstancingObj(m_instancingId);
			m_instancingId = 0L;
		}
		else if (Holder != null)
		{
			Holder.Renderer.enabled = false;
			SmallSceneMgr.Ins.ReturnObject(PrefabName, Holder);
			Holder = null;
		}
	}

	public override void Unmarshal(Octets oc, BetterList<string> prefabNames)
	{
		base.Unmarshal(oc, prefabNames);
		m_Brightness = (float)oc.pop_short() * 0.0001f;
	}
}
