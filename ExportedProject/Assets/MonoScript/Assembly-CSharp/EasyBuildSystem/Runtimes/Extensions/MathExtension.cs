using UnityEngine;

namespace EasyBuildSystem.Runtimes.Extensions
{
	public static class MathExtension
	{
		public static Bounds GetChildsBounds(this GameObject target)
		{
			Renderer[] componentsInChildren = target.GetComponentsInChildren<Renderer>();
			Quaternion rotation = target.transform.rotation;
			Vector3 localScale = target.transform.localScale;
			target.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			target.transform.localScale = Vector3.one;
			Bounds result = new Bounds(target.transform.position, Vector3.zero);
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				result.Encapsulate(renderer.bounds);
			}
			Vector3 position = result.center - target.transform.position;
			result.center = PositionToGridPosition(0.1f, 0f, position);
			result.size = PositionToGridPosition(0.1f, 0f, result.size);
			target.transform.rotation = rotation;
			target.transform.localScale = localScale;
			return result;
		}

		public static Bounds GetParentBounds(this GameObject target)
		{
			MeshRenderer[] components = target.GetComponents<MeshRenderer>();
			Quaternion rotation = target.transform.rotation;
			Vector3 localScale = target.transform.localScale;
			target.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			target.transform.localScale = Vector3.one;
			Bounds result = new Bounds(target.transform.position, Vector3.zero);
			MeshRenderer[] array = components;
			foreach (Renderer renderer in array)
			{
				result.Encapsulate(renderer.bounds);
			}
			Vector3 position = result.center - target.transform.position;
			result.center = PositionToGridPosition(0.1f, 0f, position);
			result.size = PositionToGridPosition(0.1f, 0f, result.size);
			target.transform.rotation = rotation;
			target.transform.localScale = localScale;
			return result;
		}

		public static Bounds BoundsToWorld(this Transform transform, Bounds localBounds)
		{
			if (transform != null)
			{
				return new Bounds(transform.TransformPoint(localBounds.center), localBounds.size);
			}
			return new Bounds(localBounds.center, localBounds.size);
		}

		public static float ConvertToGrid(float gridSize, float gridOffset, float axis)
		{
			return Mathf.Round(axis) * gridSize + gridOffset;
		}

		public static Vector3 PositionToGridPosition(float gridSize, float gridOffset, Vector3 position)
		{
			position -= Vector3.one * gridOffset;
			position /= gridSize;
			position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
			position *= gridSize;
			position += Vector3.one * gridOffset;
			return position;
		}
	}
}
