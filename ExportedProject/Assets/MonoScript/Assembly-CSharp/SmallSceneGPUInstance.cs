using System;
using System.Collections.Generic;
using UnityEngine;

public class SmallSceneGPUInstance : MonoBehaviour
{
	public List<GPUInstanceInfo> grass_tree_gpu;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public GPUInstanceInfo GetGPUInstanceInfo(string meshpath)
	{
		foreach (GPUInstanceInfo item in grass_tree_gpu)
		{
			if (item.meshPath.Equals(meshpath, StringComparison.OrdinalIgnoreCase))
			{
				return item;
			}
		}
		return null;
	}

	public void GetGPUInstanceInfo(List<GPUInstanceInfo> gpuInfoList)
	{
		foreach (GPUInstanceInfo item2 in grass_tree_gpu)
		{
			bool flag = false;
			foreach (GPUInstanceInfo gpuInfo in gpuInfoList)
			{
				if (item2.meshPath.Equals(gpuInfo.meshPath, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					gpuInfo.items.AddRange(item2.items);
					break;
				}
			}
			if (!flag)
			{
				GPUInstanceInfo item = new GPUInstanceInfo(item2);
				gpuInfoList.Add(item);
			}
		}
	}
}
