using UnityEngine;
using UnityEngine.UI;

public class RenderQueueFixerInUIForParticle : MonoBehaviour
{
	[SerializeField]
	public int order;

	[SerializeField]
	public bool isUI;

	[HideInInspector]
	public int extraOrder;

	[HideInInspector]
	public bool Once;

	private int sortOrder;

	private void OnEnable()
	{
		if (!Once)
		{
			StartSetting();
		}
	}

	public void StartSetting(bool once = false)
	{
		Once = once;
		Canvas canvas = Utils.FindInParents<Canvas>(base.gameObject);
		if (canvas == null)
		{
			return;
		}
		sortOrder = canvas.sortingOrder + extraOrder;
		if (isUI)
		{
			Canvas canvas2 = GetComponent<Canvas>();
			if (canvas2 == null)
			{
				canvas2 = base.gameObject.AddComponent<Canvas>();
			}
			canvas2.overrideSorting = true;
			canvas2.sortingOrder = order + sortOrder;
			GraphicRaycaster graphicRaycaster = GetComponent<GraphicRaycaster>();
			if (graphicRaycaster == null)
			{
				graphicRaycaster = base.gameObject.AddComponent<GraphicRaycaster>();
			}
			graphicRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
		}
		RendererAll(base.transform);
	}

	private void RendererAll(Transform t)
	{
		Renderer component = t.GetComponent<Renderer>();
		if ((bool)component)
		{
			component.sortingOrder = sortOrder + order;
		}
		int i = 0;
		for (int childCount = t.childCount; i < childCount; i++)
		{
			Transform child = t.GetChild(i);
			RenderQueueFixerInUIForParticle component2 = child.GetComponent<RenderQueueFixerInUIForParticle>();
			if (component2 == null)
			{
				RendererAll(child);
				continue;
			}
			component2.extraOrder = extraOrder;
			component2.StartSetting();
		}
	}
}
