using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GPUInstanceLods
{
	public int lodLevel;

	public Mesh mesh;

	public Material matrerial;

	public float showDistance_0 = 100f;

	public bool haveLightBright;

	public List<GPUInstanceItem> items { get; set; }

	public GPUInstanceLods()
	{
		lodLevel = 0;
		haveLightBright = false;
		items = new List<GPUInstanceItem>();
	}
}
