using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GPUInstanceInfo
{
	public static int nMaxPerCount = 500;

	public static float nearestGrassDistance;

	public string meshPath;

	public List<GPUInstanceItem> items;

	public List<GPUInstanceLods> lodList;

	private static Quaternion lod2Quation = Quaternion.identity;

	private static Matrix4x4 lod2Matrix = Matrix4x4.identity;

	public GPUInstanceInfo()
	{
		items = new List<GPUInstanceItem>();
		lodList = new List<GPUInstanceLods>();
	}

	public GPUInstanceInfo(GPUInstanceInfo item)
	{
		items = new List<GPUInstanceItem>(item.items);
		meshPath = item.meshPath;
		lodList = new List<GPUInstanceLods>(item.lodList);
	}

	public void Reset()
	{
		items.Clear();
		foreach (GPUInstanceLods lod in lodList)
		{
			lod.items.Clear();
		}
	}

	public void CaluShowItems(ICameraFrustumCull cameroFrustum, Vector3 camPos, Quaternion rotation)
	{
		Vector2 zero = Vector2.zero;
		float distance = 0f;
		foreach (GPUInstanceLods lod in lodList)
		{
			cameroFrustum.UpdateVisibleDistance(lod.showDistance_0);
			lod.items.Clear();
			for (int i = 0; i < items.Count; i++)
			{
				zero.x = items[i].position.x;
				zero.y = items[i].position.z;
				if (cameroFrustum.IsPointInCircularSector3(zero, ref distance) && !MatrisInLowerLod(items[i], lod.lodLevel, 0) && !MatrisInLowerLod(items[i], lod.lodLevel, 1))
				{
					if (lod.haveLightBright && distance < nearestGrassDistance)
					{
						nearestGrassDistance = distance;
					}
					if (lod.lodLevel == 2)
					{
						lod2Quation.SetLookRotation(camPos - items[i].position);
						lod2Matrix.SetTRS(items[i].position, lod2Quation, items[i].localScale);
						items[i].matrisList = lod2Matrix;
					}
					else if (items[i].matrisList == Matrix4x4.zero)
					{
						items[i].matrisList.SetTRS(items[i].position, Quaternion.Euler(items[i].eulerAngles), items[i].localScale);
					}
					else if (lodList.Count > 2)
					{
						items[i].matrisList.SetTRS(items[i].position, Quaternion.Euler(items[i].eulerAngles), items[i].localScale);
					}
					lod.items.Add(items[i]);
				}
			}
		}
	}

	private GPUInstanceLods GetGpuLod(int lod)
	{
		foreach (GPUInstanceLods lod2 in lodList)
		{
			if (lod == lod2.lodLevel)
			{
				return lod2;
			}
		}
		return null;
	}

	private bool MatrisInLowerLod(GPUInstanceItem matr, int selflod, int checklod)
	{
		if (selflod <= checklod)
		{
			return false;
		}
		GPUInstanceLods gpuLod = GetGpuLod(checklod);
		if (gpuLod.items.Contains(matr))
		{
			return true;
		}
		return false;
	}
}
