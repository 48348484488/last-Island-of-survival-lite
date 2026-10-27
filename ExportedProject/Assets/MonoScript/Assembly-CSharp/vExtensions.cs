using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class vExtensions
{
	private static ClipPlanePoints clipPlanePoints = default(ClipPlanePoints);

	public static T[] Append<T>(this T[] arrayInitial, T[] arrayToAppend)
	{
		if (arrayToAppend == null)
		{
			throw new ArgumentNullException("The appended object cannot be null");
		}
		if (arrayInitial is string || arrayToAppend is string)
		{
			throw new ArgumentException("The argument must be an enumerable");
		}
		T[] array = new T[arrayInitial.Length + arrayToAppend.Length];
		arrayInitial.CopyTo(array, 0);
		arrayToAppend.CopyTo(array, arrayInitial.Length);
		return array;
	}

	public static Vector3 NormalizeAngle(this Vector3 eulerAngle)
	{
		Vector3 vector = eulerAngle;
		if (vector.x > 180f)
		{
			vector.x -= 360f;
		}
		else if (vector.x < -180f)
		{
			vector.x += 360f;
		}
		if (vector.y > 180f)
		{
			vector.y -= 360f;
		}
		else if (vector.y < -180f)
		{
			vector.y += 360f;
		}
		if (vector.z > 180f)
		{
			vector.z -= 360f;
		}
		else if (vector.z < -180f)
		{
			vector.z += 360f;
		}
		return new Vector3(vector.x, vector.y, vector.z);
	}

	public static Vector3 Difference(this Vector3 vector, Vector3 otherVector)
	{
		return otherVector - vector;
	}

	public static void SetActiveChildren(this GameObject gameObjet, bool value)
	{
		IEnumerator enumerator = gameObjet.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				transform.gameObject.SetActive(value);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static void SetLayerRecursively(this GameObject obj, int layer)
	{
		obj.layer = layer;
		IEnumerator enumerator = obj.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				transform.gameObject.SetLayerRecursively(layer);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		do
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
		}
		while (angle < -360f || angle > 360f);
		return Mathf.Clamp(angle, min, max);
	}

	public static float ClampAngle2(float angle, float min, float max)
	{
		do
		{
			if (angle < -180f)
			{
				angle += 360f;
			}
			if (angle > 180f)
			{
				angle -= 360f;
			}
		}
		while (angle < -180f || angle > 180f);
		return Mathf.Clamp(angle, min, max);
	}

	public static float NormalizeAngle(float angle)
	{
		do
		{
			if (angle < -180f)
			{
				angle += 360f;
			}
			if (angle > 180f)
			{
				angle -= 360f;
			}
		}
		while (angle < -180f || angle > 180f);
		return angle;
	}

	public static void Slerp(this vThirdPersonCameraState to, vThirdPersonCameraState from, float time)
	{
		to.Name = from.Name;
		to.lookAtPosition = Vector3.Lerp(to.lookAtPosition, from.lookAtPosition, time);
		to.defaultDistance = Mathf.Lerp(to.defaultDistance, from.defaultDistance, time);
		to.xMouseSensitivity = Mathf.Lerp(to.xMouseSensitivity, from.xMouseSensitivity, time);
		to.yMouseSensitivity = Mathf.Lerp(to.yMouseSensitivity, from.yMouseSensitivity, time);
		to.yMinLimit = Mathf.Lerp(to.yMinLimit, from.yMinLimit, time);
		to.yMaxLimit = Mathf.Lerp(to.yMaxLimit, from.yMaxLimit, time);
		to.xMinLimit = Mathf.Lerp(to.xMinLimit, from.xMinLimit, time);
		to.xMaxLimit = Mathf.Lerp(to.xMaxLimit, from.xMaxLimit, time);
		to.cullingMinDist = Mathf.Lerp(to.cullingMinDist, from.cullingMinDist, time);
		to.cameraMode = from.cameraMode;
		to.fov = Mathf.Lerp(to.fov, from.fov, time);
		to.dragByAngle = from.dragByAngle;
		to.followDirectionX = from.followDirectionX;
		to.followDirectionY = from.followDirectionY;
		to.followSmooth = from.followSmooth;
		to.target = from.target;
		to.changeSmooth = from.changeSmooth;
		to.NearClippingPlane = from.NearClippingPlane;
		to.FarClippingPlane = from.FarClippingPlane;
	}

	public static void CopyState(this vThirdPersonCameraState to, vThirdPersonCameraState from)
	{
		to.Name = from.Name;
		to.lookAtPosition = from.lookAtPosition;
		to.defaultDistance = from.defaultDistance;
		to.xMouseSensitivity = from.xMouseSensitivity;
		to.yMouseSensitivity = from.yMouseSensitivity;
		to.yMinLimit = from.yMinLimit;
		to.yMaxLimit = from.yMaxLimit;
		to.xMinLimit = from.xMinLimit;
		to.xMaxLimit = from.xMaxLimit;
		to.cullingHeight = from.cullingHeight;
		to.cullingMinDist = from.cullingMinDist;
		to.cameraMode = from.cameraMode;
		to.fov = from.fov;
		to.dragByAngle = from.dragByAngle;
		to.followDirectionX = from.followDirectionX;
		to.followDirectionY = from.followDirectionY;
		to.followSmooth = from.followSmooth;
		to.target = from.target;
		to.changeSmooth = from.changeSmooth;
		to.NearClippingPlane = from.NearClippingPlane;
		to.FarClippingPlane = from.FarClippingPlane;
	}

	public static List<T> vCopy<T>(this List<T> list)
	{
		List<T> list2 = new List<T>();
		if (list == null || list.Count == 0)
		{
			return list;
		}
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(list[i]);
		}
		return list2;
	}

	public static List<T> vToList<T>(this T[] array)
	{
		List<T> list = new List<T>();
		if (array == null || array.Length == 0)
		{
			return list;
		}
		for (int i = 0; i < array.Length; i++)
		{
			list.Add(array[i]);
		}
		return list;
	}

	public static T[] vToArray<T>(this List<T> list)
	{
		T[] array = new T[list.Count];
		if (list == null || list.Count == 0)
		{
			return array;
		}
		for (int i = 0; i < list.Count; i++)
		{
			array[i] = list[i];
		}
		return array;
	}

	public static ClipPlanePoints NearClipPlanePoints(this Camera camera, Transform cameraTransform, Vector3 pos, float clipPlaneMargin)
	{
		float f = camera.fieldOfView / 2f * ((float)Math.PI / 180f);
		float aspect = camera.aspect;
		float nearClipPlane = camera.nearClipPlane;
		float num = nearClipPlane * Mathf.Tan(f);
		float num2 = num * aspect;
		num *= 1f + clipPlaneMargin;
		num2 *= 1f + clipPlaneMargin;
		clipPlanePoints.LowerRight = pos + cameraTransform.right * num2;
		clipPlanePoints.LowerRight -= cameraTransform.up * num;
		clipPlanePoints.LowerRight += cameraTransform.forward * nearClipPlane;
		clipPlanePoints.LowerLeft = pos - cameraTransform.right * num2;
		clipPlanePoints.LowerLeft -= cameraTransform.up * num;
		clipPlanePoints.LowerLeft += cameraTransform.forward * nearClipPlane;
		clipPlanePoints.UpperRight = pos + cameraTransform.right * num2;
		clipPlanePoints.UpperRight += cameraTransform.up * num;
		clipPlanePoints.UpperRight += cameraTransform.forward * nearClipPlane;
		clipPlanePoints.UpperLeft = pos - cameraTransform.right * num2;
		clipPlanePoints.UpperLeft += cameraTransform.up * num;
		clipPlanePoints.UpperLeft += cameraTransform.forward * nearClipPlane;
		return clipPlanePoints;
	}

	public static HitBarPoints GetBoundPoint(this BoxCollider boxCollider, Transform torso, LayerMask mask)
	{
		HitBarPoints hitBarPoints = HitBarPoints.None;
		BoxPoint boxPoint = boxCollider.GetBoxPoint();
		Ray ray = new Ray(boxPoint.top, boxPoint.top - torso.position);
		Ray ray2 = new Ray(torso.position, boxPoint.center - torso.position);
		Ray ray3 = new Ray(torso.position, boxPoint.bottom - torso.position);
		Debug.DrawRay(ray.origin, ray.direction, Color.red, 2f);
		Debug.DrawRay(ray2.origin, ray2.direction, Color.green, 2f);
		Debug.DrawRay(ray3.origin, ray3.direction, Color.blue, 2f);
		float maxDistance = Vector3.Distance(torso.position, boxPoint.top);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, maxDistance, mask))
		{
			hitBarPoints |= HitBarPoints.Top;
			Debug.Log(hitInfo.transform.name);
		}
		maxDistance = Vector3.Distance(torso.position, boxPoint.center);
		if (Physics.Raycast(ray2, out hitInfo, maxDistance, mask))
		{
			hitBarPoints |= HitBarPoints.Center;
			Debug.Log(hitInfo.transform.name);
		}
		maxDistance = Vector3.Distance(torso.position, boxPoint.bottom);
		if (Physics.Raycast(ray3, out hitInfo, maxDistance, mask))
		{
			hitBarPoints |= HitBarPoints.Bottom;
			Debug.Log(hitInfo.transform.name);
		}
		return hitBarPoints;
	}

	public static BoxPoint GetBoxPoint(this BoxCollider boxCollider)
	{
		BoxPoint result = default(BoxPoint);
		result.center = boxCollider.transform.TransformPoint(boxCollider.center);
		float num = boxCollider.transform.lossyScale.y * boxCollider.size.y;
		Ray ray = new Ray(result.center, boxCollider.transform.up);
		result.top = ray.GetPoint(num * 0.5f);
		result.bottom = ray.GetPoint(0f - num * 0.5f);
		return result;
	}

	public static Vector3 BoxSize(this BoxCollider boxCollider)
	{
		float x = boxCollider.transform.lossyScale.x * boxCollider.size.x;
		float z = boxCollider.transform.lossyScale.z * boxCollider.size.z;
		float y = boxCollider.transform.lossyScale.y * boxCollider.size.y;
		return new Vector3(x, y, z);
	}

	public static bool Contains(this Enum keys, Enum flag)
	{
		if (keys.GetType() != flag.GetType())
		{
			throw new ArgumentException("Type Mismatch");
		}
		return (Convert.ToUInt64(keys) & Convert.ToUInt64(flag)) != 0;
	}
}
