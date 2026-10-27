using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/TextGradient")]
public class TextGradient : BaseMeshEffect
{
	[SerializeField]
	private Color32 topColor = Color.white;

	[SerializeField]
	private Color32 bottomColor = Color.black;

	public override void ModifyMesh(VertexHelper vh)
	{
		if (IsActive())
		{
			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);
			ModifyVertices(list);
			vh.Clear();
			vh.AddUIVertexTriangleStream(list);
		}
	}

	public void ModifyVertices(List<UIVertex> vertexList)
	{
		if (!IsActive())
		{
			return;
		}
		int count = vertexList.Count;
		if (count <= 0)
		{
			return;
		}
		float num = vertexList[0].position.y;
		float num2 = vertexList[0].position.y;
		for (int i = 1; i < count; i++)
		{
			float y = vertexList[i].position.y;
			if (y > num2)
			{
				num2 = y;
			}
			else if (y < num)
			{
				num = y;
			}
		}
		float num3 = num2 - num;
		for (int j = 0; j < count; j++)
		{
			UIVertex value = vertexList[j];
			value.color = Color32.Lerp(bottomColor, topColor, (value.position.y - num) / num3);
			vertexList[j] = value;
		}
	}
}
