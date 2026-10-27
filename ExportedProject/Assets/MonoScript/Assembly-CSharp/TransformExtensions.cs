using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TransformExtensions
{
	[CompilerGenerated]
	private static Func<Transform, IComparable> _003C_003Ef__am_0024cache0;

	public static bool isChild(this Transform me, Transform target)
	{
		if (!target)
		{
			return false;
		}
		Transform transform = me.FindChildByNameRecursive(target.gameObject.name);
		if (transform == null)
		{
			return false;
		}
		return transform.Equals(target);
	}

	public static Transform FindChildByNameRecursive(this Transform me, string name)
	{
		if (me.name == name)
		{
			return me;
		}
		for (int i = 0; i < me.childCount; i++)
		{
			Transform transform = me.GetChild(i).FindChildByNameRecursive(name);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public static void SetX(this Transform transform, float x)
	{
		Vector3 vector2 = (transform.position = new Vector3(x, transform.position.y, transform.position.z));
	}

	public static void SetY(this Transform transform, float y)
	{
		Vector3 vector2 = (transform.position = new Vector3(transform.position.x, y, transform.position.z));
	}

	public static void SetZ(this Transform transform, float z)
	{
		Vector3 vector2 = (transform.position = new Vector3(transform.position.x, transform.position.y, z));
	}

	public static void SetXY(this Transform transform, float x, float y)
	{
		Vector3 vector2 = (transform.position = new Vector3(x, y, transform.position.z));
	}

	public static void SetXZ(this Transform transform, float x, float z)
	{
		Vector3 vector2 = (transform.position = new Vector3(x, transform.position.y, z));
	}

	public static void SetYZ(this Transform transform, float y, float z)
	{
		Vector3 vector2 = (transform.position = new Vector3(transform.position.x, y, z));
	}

	public static void SetXYZ(this Transform transform, float x, float y, float z)
	{
		Vector3 vector2 = (transform.position = new Vector3(x, y, z));
	}

	public static void TranslateX(this Transform transform, float x)
	{
		Vector3 vector = new Vector3(x, 0f, 0f);
		transform.position += vector;
	}

	public static void TranslateY(this Transform transform, float y)
	{
		Vector3 vector = new Vector3(0f, y, 0f);
		transform.position += vector;
	}

	public static void TranslateZ(this Transform transform, float z)
	{
		Vector3 vector = new Vector3(0f, 0f, z);
		transform.position += vector;
	}

	public static void TranslateXY(this Transform transform, float x, float y)
	{
		Vector3 vector = new Vector3(x, y, 0f);
		transform.position += vector;
	}

	public static void TranslateXZ(this Transform transform, float x, float z)
	{
		Vector3 vector = new Vector3(x, 0f, z);
		transform.position += vector;
	}

	public static void TranslateYZ(this Transform transform, float y, float z)
	{
		Vector3 vector = new Vector3(0f, y, z);
		transform.position += vector;
	}

	public static void TranslateXYZ(this Transform transform, float x, float y, float z)
	{
		Vector3 vector = new Vector3(x, y, z);
		transform.position += vector;
	}

	public static void SetLocalX(this Transform transform, float x)
	{
		Vector3 vector2 = (transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z));
	}

	public static void SetLocalY(this Transform transform, float y)
	{
		Vector3 vector2 = (transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z));
	}

	public static void SetLocalZ(this Transform transform, float z)
	{
		Vector3 vector2 = (transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z));
	}

	public static void SetLocalXY(this Transform transform, float x, float y)
	{
		Vector3 vector2 = (transform.localPosition = new Vector3(x, y, transform.localPosition.z));
	}

	public static void SetLocalXZ(this Transform transform, float x, float z)
	{
		Vector3 vector2 = (transform.localPosition = new Vector3(x, transform.localPosition.z, z));
	}

	public static void SetLocalYZ(this Transform transform, float y, float z)
	{
		Vector3 vector2 = (transform.localPosition = new Vector3(transform.localPosition.x, y, z));
	}

	public static void SetLocalXYZ(this Transform transform, float x, float y, float z)
	{
		Vector3 vector2 = (transform.localPosition = new Vector3(x, y, z));
	}

	public static void ResetPosition(this Transform transform)
	{
		transform.position = Vector3.zero;
	}

	public static void ResetLocalPosition(this Transform transform)
	{
		transform.localPosition = Vector3.zero;
	}

	public static void SetScaleX(this Transform transform, float x)
	{
		Vector3 vector2 = (transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z));
	}

	public static void SetScaleY(this Transform transform, float y)
	{
		Vector3 vector2 = (transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z));
	}

	public static void SetScaleZ(this Transform transform, float z)
	{
		Vector3 vector2 = (transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z));
	}

	public static void SetScaleXY(this Transform transform, float x, float y)
	{
		Vector3 vector2 = (transform.localScale = new Vector3(x, y, transform.localScale.z));
	}

	public static void SetScaleXZ(this Transform transform, float x, float z)
	{
		Vector3 vector2 = (transform.localScale = new Vector3(x, transform.localScale.y, z));
	}

	public static void SetScaleYZ(this Transform transform, float y, float z)
	{
		Vector3 vector2 = (transform.localScale = new Vector3(transform.localScale.x, y, z));
	}

	public static void SetScaleXYZ(this Transform transform, float x, float y, float z)
	{
		Vector3 vector2 = (transform.localScale = new Vector3(x, y, z));
	}

	public static void ScaleByX(this Transform transform, float x)
	{
		transform.localScale = new Vector3(transform.localScale.x * x, transform.localScale.y, transform.localScale.z);
	}

	public static void ScaleByY(this Transform transform, float y)
	{
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * y, transform.localScale.z);
	}

	public static void ScaleByZ(this Transform transform, float z)
	{
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z * z);
	}

	public static void ScaleByXY(this Transform transform, float x, float y)
	{
		transform.localScale = new Vector3(transform.localScale.x * x, transform.localScale.y * y, transform.localScale.z);
	}

	public static void ScaleByXZ(this Transform transform, float x, float z)
	{
		transform.localScale = new Vector3(transform.localScale.x * x, transform.localScale.y, transform.localScale.z * z);
	}

	public static void ScaleByYZ(this Transform transform, float y, float z)
	{
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * y, transform.localScale.z * z);
	}

	public static void ScaleByXY(this Transform transform, float r)
	{
		transform.ScaleByXY(r, r);
	}

	public static void ScaleByXZ(this Transform transform, float r)
	{
		transform.ScaleByXZ(r, r);
	}

	public static void ScaleByYZ(this Transform transform, float r)
	{
		transform.ScaleByYZ(r, r);
	}

	public static void ScaleByXYZ(this Transform transform, float x, float y, float z)
	{
		transform.localScale = new Vector3(x, y, z);
	}

	public static void ScaleByXYZ(this Transform transform, float r)
	{
		transform.ScaleByXYZ(r, r, r);
	}

	public static void ResetScale(this Transform transform)
	{
		transform.localScale = Vector3.one;
	}

	public static void FlipX(this Transform transform)
	{
		transform.SetScaleX(0f - transform.localScale.x);
	}

	public static void FlipY(this Transform transform)
	{
		transform.SetScaleY(0f - transform.localScale.y);
	}

	public static void FlipZ(this Transform transform)
	{
		transform.SetScaleZ(0f - transform.localScale.z);
	}

	public static void FlipXY(this Transform transform)
	{
		transform.SetScaleXY(0f - transform.localScale.x, 0f - transform.localScale.y);
	}

	public static void FlipXZ(this Transform transform)
	{
		transform.SetScaleXZ(0f - transform.localScale.x, 0f - transform.localScale.z);
	}

	public static void FlipYZ(this Transform transform)
	{
		transform.SetScaleYZ(0f - transform.localScale.y, 0f - transform.localScale.z);
	}

	public static void FlipXYZ(this Transform transform)
	{
		transform.SetScaleXYZ(0f - transform.localScale.z, 0f - transform.localScale.y, 0f - transform.localScale.z);
	}

	public static void FlipPostive(this Transform transform)
	{
		transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
	}

	public static void RotateAroundX(this Transform transform, float angle)
	{
		Vector3 eulers = new Vector3(angle, 0f, 0f);
		transform.Rotate(eulers);
	}

	public static void RotateAroundY(this Transform transform, float angle)
	{
		Vector3 eulers = new Vector3(0f, angle, 0f);
		transform.Rotate(eulers);
	}

	public static void RotateAroundZ(this Transform transform, float angle)
	{
		Vector3 eulers = new Vector3(0f, 0f, angle);
		transform.Rotate(eulers);
	}

	public static void SetRotationX(this Transform transform, float angle)
	{
		transform.eulerAngles = new Vector3(angle, 0f, 0f);
	}

	public static void SetRotationY(this Transform transform, float angle)
	{
		transform.eulerAngles = new Vector3(0f, angle, 0f);
	}

	public static void SetRotationZ(this Transform transform, float angle)
	{
		transform.eulerAngles = new Vector3(0f, 0f, angle);
	}

	public static void SetLocalRotationX(this Transform transform, float angle)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(angle, 0f, 0f));
	}

	public static void SetLocalRotationY(this Transform transform, float angle)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
	}

	public static void SetLocalRotationZ(this Transform transform, float angle)
	{
		transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
	}

	public static void ResetRotation(this Transform transform)
	{
		transform.rotation = Quaternion.identity;
	}

	public static void ResetLocalRotation(this Transform transform)
	{
		transform.localRotation = Quaternion.identity;
	}

	public static void ResetLocal(this Transform transform)
	{
		transform.ResetLocalRotation();
		transform.ResetLocalPosition();
		transform.ResetScale();
	}

	public static void Reset(this Transform transform)
	{
		transform.ResetRotation();
		transform.ResetPosition();
		transform.ResetScale();
	}

	public static void DestroyChildren(this Transform transform)
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			list.Add(child);
		}
		foreach (Transform item in list)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	public static void DestroyChildrenImmediate(this Transform transform)
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			list.Add(child);
		}
		foreach (Transform item in list)
		{
			UnityEngine.Object.DestroyImmediate(item.gameObject);
		}
	}

	public static List<Transform> GetChildren(this Transform transform)
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			list.Add(child);
		}
		return list;
	}

	public static void Sort(this Transform transform, Func<Transform, IComparable> sortFunction)
	{
		List<Transform> children = transform.GetChildren();
		List<Transform> list = children.OrderBy(sortFunction).ToList();
		for (int i = 0; i < list.Count(); i++)
		{
			list[i].SetSiblingIndex(i);
		}
	}

	public static void SortAlphabetically(this Transform transform)
	{
		if (_003C_003Ef__am_0024cache0 == null)
		{
			_003C_003Ef__am_0024cache0 = _003CSortAlphabetically_003Em__0;
		}
		transform.Sort(_003C_003Ef__am_0024cache0);
	}

	public static IEnumerable<Transform> SelfAndAllChildren(this Transform transform)
	{
		Queue<Transform> openList = new Queue<Transform>();
		openList.Enqueue(transform);
		while (openList.Any())
		{
			yield return openList.Dequeue();
			List<Transform> children = transform.GetChildren();
			foreach (Transform item in children)
			{
				openList.Enqueue(item);
			}
		}
	}

	[CompilerGenerated]
	private static IComparable _003CSortAlphabetically_003Em__0(Transform t)
	{
		return t.name;
	}
}
