using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Extensions
{
	public static class PhysicExtension
	{
		public static Collider[] Colliders = new Collider[30];

		public static void SetLayerRecursively(this GameObject go, LayerMask layer)
		{
			if (go == null)
			{
				return;
			}
			go.layer = ToLayer(layer.value);
			IEnumerator enumerator = go.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					if (!(transform == null))
					{
						transform.gameObject.SetLayerRecursively(layer);
					}
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

		public static int ToLayer(int bitmask)
		{
			int num = ((bitmask <= 0) ? 31 : 0);
			while (bitmask > 1)
			{
				bitmask >>= 1;
				num++;
			}
			return num;
		}

		public static T[] GetNeighborsTypesBySphere<T>(Vector3 position, float size, int layer, bool allowSame = true, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
		{
			int num = Physics.OverlapSphereNonAlloc(position, size, Colliders, 1 << layer, query);
			List<T> list = new List<T>();
			for (int i = 0; i < num; i++)
			{
				T componentInParent = Colliders[i].GetComponentInParent<T>();
				if (componentInParent != null)
				{
					if (allowSame)
					{
						list.Add(componentInParent);
					}
					else if (!list.Contains(componentInParent))
					{
						list.Add(componentInParent);
					}
				}
			}
			return list.ToArray();
		}

		public static T[] GetNeighborsTypesByBox<T>(Vector3 position, Vector3 size, Quaternion rotation, int layer, QueryTriggerInteraction query = QueryTriggerInteraction.UseGlobal)
		{
			bool queriesHitTriggers = Physics.queriesHitTriggers;
			Physics.queriesHitTriggers = true;
			int num = Physics.OverlapBoxNonAlloc(position, size, Colliders, rotation, layer, query);
			Physics.queriesHitTriggers = queriesHitTriggers;
			List<T> list = new List<T>();
			for (int i = 0; i < num; i++)
			{
				T componentInParent = Colliders[i].GetComponentInParent<T>();
				if (componentInParent != null && componentInParent is T && !list.Contains(componentInParent))
				{
					list.Add(componentInParent);
				}
			}
			return list.ToArray();
		}
	}
}
