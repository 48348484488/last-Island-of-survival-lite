using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapObject : MonoBehaviour
{
	public long InsId;

	public int CfgId;

	public string MapObjectName;

	public float Hp;

	public int MaxHp;

	public List<Renderer> Renderers = new List<Renderer>();

	protected virtual void Awake()
	{
		Renderers = GetComponentsInChildren<Renderer>(true).ToList();
		for (int i = 0; i < Renderers.Count; i++)
		{
			if (!(Renderers[i] is MeshRenderer) && !(Renderers[i] is SkinnedMeshRenderer))
			{
				Renderers.Remove(Renderers[i]);
			}
		}
	}
}
