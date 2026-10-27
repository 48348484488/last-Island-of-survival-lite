using System;
using UnityEngine;

[Serializable]
public class vThirdPersonCameraState
{
	public string Name;

	public Vector3 lookAtPosition;

	public float defaultDistance;

	public float xMouseSensitivity;

	public float yMouseSensitivity;

	public float yMinLimit;

	public float yMaxLimit;

	public float xMinLimit;

	public float xMaxLimit;

	public float cullingHeight;

	public float cullingMinDist;

	public float fov;

	public Vector2 fixedAngle;

	public TPCameraMode cameraMode;

	public bool dragByAngle;

	public bool followDirectionX;

	public bool followDirectionY;

	public float followSmooth;

	public float changeSmooth;

	public float NearClippingPlane;

	public float FarClippingPlane;

	[NonSerialized]
	public Transform target;

	public vThirdPersonCameraState(string name)
	{
		Name = name;
		lookAtPosition = Vector3.zero;
		defaultDistance = 1.5f;
		xMouseSensitivity = 3f;
		yMouseSensitivity = 3f;
		yMinLimit = -40f;
		yMaxLimit = 80f;
		xMinLimit = -360f;
		xMaxLimit = 360f;
		cullingHeight = 0.2f;
		cullingMinDist = 0.1f;
		fixedAngle = Vector2.zero;
		cameraMode = TPCameraMode.FreeDirectional;
		dragByAngle = false;
		followDirectionX = false;
		followDirectionY = false;
		followSmooth = 3f;
		changeSmooth = 8f;
		NearClippingPlane = 0.1f;
		FarClippingPlane = 800f;
	}
}
