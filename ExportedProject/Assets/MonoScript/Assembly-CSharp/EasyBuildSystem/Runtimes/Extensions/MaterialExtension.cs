using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Runtimes.Extensions
{
	public static class MaterialExtension
	{
		public static void ChangeAllMaterialsColorInChildren(this GameObject go, Renderer[] renderers, Color color, float lerpTime = 15f, bool lerp = false)
		{
			Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (!(componentsInChildren[i] != null))
				{
					continue;
				}
				for (int j = 0; j < componentsInChildren[i].materials.Length; j++)
				{
					if (lerp)
					{
						componentsInChildren[i].materials[j].SetColor("_OutlineColor", color);
					}
					else
					{
						componentsInChildren[i].materials[j].SetColor("_OutlineColor", color);
					}
				}
			}
		}

		public static void ChangeAllMaterialsInChildren(this GameObject go, Renderer[] renderers, Material material)
		{
			for (int i = 0; i < renderers.Length; i++)
			{
				if (renderers[i] != null)
				{
					Material[] array = new Material[renderers[i].sharedMaterials.Length];
					for (int j = 0; j < renderers[i].sharedMaterials.Length; j++)
					{
						array[j] = material;
					}
					renderers[i].sharedMaterials = array;
				}
			}
		}

		public static void ChangeAllMaterialsOutLineInChildren(this GameObject go, Renderer renderer, Color color, bool isSelect)
		{
			if (!(renderer == null))
			{
				if (isSelect)
				{
					OutlineFilter.Select(renderer.gameObject, color);
				}
				else
				{
					OutlineFilter.CancelSelect();
				}
			}
		}

		public static void ChangeAllMaterialsInChildren(this GameObject go, Renderer[] renderers, Dictionary<Renderer, Material[]> materials)
		{
			for (int i = 0; i < renderers.Length; i++)
			{
				Material[] sharedMaterials = renderers[i].sharedMaterials;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					if (materials.ContainsKey(renderers[i]))
					{
						sharedMaterials[j] = materials[renderers[i]][j];
					}
				}
				renderers[i].materials = sharedMaterials;
			}
		}
	}
}
