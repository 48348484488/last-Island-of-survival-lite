using UnityEngine;
using UnityEngine.EventSystems;

public class DragListener : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	public static PointerEventData pointEventData;

	public VoidDelegate onBeginDrag;

	public VoidDelegate onDrag;

	public VoidDelegate onEndDrag;

	public VoidDelegate onEnter;

	public static DragListener Get(GameObject go)
	{
		DragListener dragListener = go.GetComponent<DragListener>();
		if (dragListener == null)
		{
			dragListener = go.AddComponent<DragListener>();
		}
		return dragListener;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onBeginDrag != null)
		{
			onBeginDrag(base.gameObject);
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onDrag != null)
		{
			onDrag(base.gameObject);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onEndDrag != null)
		{
			onEndDrag(base.gameObject);
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onEnter != null)
		{
			onEnter(base.gameObject);
		}
	}
}
