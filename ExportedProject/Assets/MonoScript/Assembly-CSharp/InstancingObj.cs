using System;
using UnityEngine;

public sealed class InstancingObj : IComparable
{
	public long id;

	public int meshInsId;

	public int materialInsId;

	public Matrix4x4 matrix;

	public bool castShadows = true;

	public bool receiveShadows = true;

	public int CompareTo(object obj)
	{
		InstancingObj instancingObj = (InstancingObj)obj;
		if (meshInsId > instancingObj.meshInsId)
		{
			return 1;
		}
		if (meshInsId < instancingObj.meshInsId)
		{
			return -1;
		}
		return materialInsId - instancingObj.materialInsId;
	}

	public void Reset()
	{
	}
}
