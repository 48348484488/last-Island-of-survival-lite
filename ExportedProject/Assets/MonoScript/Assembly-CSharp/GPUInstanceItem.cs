using System;
using UnityEngine;

[Serializable]
public class GPUInstanceItem
{
	public Vector3 position;

	public Vector3 localScale;

	public Vector3 eulerAngles;

	public Matrix4x4 matrisList;

	public float lightmapColor;
}
