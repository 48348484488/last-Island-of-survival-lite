using UnityEngine;

namespace EasyBuildSystem.Runtimes.Extensions
{
	public static class GameObjectExtension
	{
		public static void AddRigibody(this GameObject target, bool useGravity, bool isKinematic, float maxDepenetrationVelocity = 15f, HideFlags flag = HideFlags.HideAndDontSave)
		{
			if (!(target == null) && !(target.GetComponent<Rigidbody>() != null))
			{
				Rigidbody rigidbody = target.AddComponent<Rigidbody>();
				rigidbody.maxDepenetrationVelocity = maxDepenetrationVelocity;
				rigidbody.useGravity = useGravity;
				rigidbody.isKinematic = isKinematic;
				rigidbody.hideFlags = flag;
			}
		}

		public static void AddSphereCollider(this GameObject target, float radius, bool isTrigger = true, HideFlags flag = HideFlags.HideAndDontSave)
		{
			if (!(target == null) && !(target.GetComponent<Rigidbody>() != null) && !(target.GetComponent<SphereCollider>() != null))
			{
				SphereCollider sphereCollider = target.AddComponent<SphereCollider>();
				sphereCollider.radius = radius;
				sphereCollider.isTrigger = isTrigger;
				sphereCollider.hideFlags = flag;
			}
		}

		public static void AddBoxCollider(this GameObject target, Vector3 size, Vector3 center, bool isTrigger = true, HideFlags flag = HideFlags.HideAndDontSave)
		{
			if (!(target == null) && !(target.GetComponent<Rigidbody>() != null) && !(target.GetComponent<BoxCollider>() != null))
			{
				BoxCollider boxCollider = target.AddComponent<BoxCollider>();
				boxCollider.size = size;
				boxCollider.center = center;
				boxCollider.isTrigger = isTrigger;
				boxCollider.hideFlags = flag;
			}
		}
	}
}
