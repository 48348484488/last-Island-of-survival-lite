using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class vThirdPersonCameraListData : ScriptableObject
{
	[SerializeField]
	public string Name;

	[SerializeField]
	public List<vThirdPersonCameraState> tpCameraStates;

	public vThirdPersonCameraListData()
	{
		tpCameraStates = new List<vThirdPersonCameraState>();
		tpCameraStates.Add(new vThirdPersonCameraState("Default"));
	}

	public vThirdPersonCameraState FindState(string stateName, out int index)
	{
		int i = 0;
		for (int count = tpCameraStates.Count; i < count; i++)
		{
			vThirdPersonCameraState vThirdPersonCameraState2 = tpCameraStates[i];
			if (vThirdPersonCameraState2.Name == stateName)
			{
				index = i;
				return vThirdPersonCameraState2;
			}
		}
		index = -1;
		return null;
	}

	public vThirdPersonCameraState FindState(string stateName)
	{
		int index;
		return FindState(stateName, out index);
	}
}
