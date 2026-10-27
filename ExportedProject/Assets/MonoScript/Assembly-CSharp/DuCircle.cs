using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuCircle : MaskableGraphic
{
	private const int MaxSegmentNum = 64;

	private const float Thickness = 3f;

	private static readonly List<Vector2> sVertices;

	private static readonly List<int> sIndicies;

	private static Material sMaterial;

	private readonly List<Vector3> mVertices = new List<Vector3>(128);

	private readonly List<Color> mColors = new List<Color>(128);

	private float mRadius = 200f;

	public float Radius
	{
		get
		{
			return mRadius;
		}
		set
		{
			mRadius = value;
			SetVerticesDirty();
		}
	}

	static DuCircle()
	{
		sVertices = new List<Vector2>(64);
		sIndicies = new List<int>(384);
		float num = (float)Math.PI / 32f;
		float num2 = 0f;
		for (int i = 0; i < 64; i++)
		{
			Vector2 item = Vector3.zero;
			item.x = Mathf.Cos(num2);
			item.y = Mathf.Sin(num2);
			sVertices.Add(item);
			num2 += num;
		}
		for (int j = 0; j < 64; j++)
		{
			int num3 = 2 * j;
			sIndicies.Add(num3);
			sIndicies.Add(num3 + 1);
			sIndicies.Add(num3 + 3);
			sIndicies.Add(num3 + 3);
			sIndicies.Add(num3 + 2);
			sIndicies.Add(num3);
		}
		int num4 = sIndicies.Count - 6;
		sIndicies[num4 + 2] = 1;
		sIndicies[num4 + 3] = 1;
		sIndicies[num4 + 4] = 0;
	}

	protected override void Awake()
	{
		base.Awake();
		for (int i = 0; i < 64; i++)
		{
			mVertices.Add(Vector3.zero);
			mVertices.Add(Vector3.zero);
			mColors.Add(Color.white);
			mColors.Add(Color.white);
		}
		m_OnDirtyVertsCallback = Fill;
		if (sMaterial == null)
		{
			sMaterial = UnityEngine.Object.Instantiate(Graphic.defaultGraphicMaterial);
			sMaterial.name = "DuCircle";
			sMaterial.shader = Shader.Find("Custom/DuCircle");
		}
		m_Material = sMaterial;
	}

	public void SetColor(Color c)
	{
		int i = 0;
		for (int count = mColors.Count; i < count; i++)
		{
			mColors[i] = c;
		}
	}

	private void Fill()
	{
		float num = mRadius - 3f;
		if (num < 0f)
		{
			num = 0f;
		}
		for (int i = 0; i < 64; i++)
		{
			int num2 = 2 * i;
			mVertices[num2] = sVertices[i] * mRadius;
			mVertices[num2 + 1] = sVertices[i] * num;
		}
	}

	protected override void UpdateGeometry()
	{
		Mesh mesh = Graphic.workerMesh;
		mesh.Clear();
		mesh.SetVertices(mVertices);
		mesh.SetColors(mColors);
		mesh.SetTriangles(sIndicies, 0);
		mesh.RecalculateBounds();
		base.canvasRenderer.SetMesh(mesh);
	}
}
