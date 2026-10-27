using UnityEngine;
using UnityEngine.EventSystems;

public class YaoganDragListener : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	public static PointerEventData pointEventData;

	public VoidDelegate onBeginDrag;

	public VoidDelegate onDrag;

	public VoidDelegate onEndDrag;

	public VoidDelegate onEnter;

	public VoidDelegate onDown;

	public VoidDelegate onUp;

	public static YaoganDragListener Get(GameObject go)
	{
		YaoganDragListener yaoganDragListener = go.GetComponent<YaoganDragListener>();
		if (yaoganDragListener == null)
		{
			yaoganDragListener = go.AddComponent<YaoganDragListener>();
		}
		return yaoganDragListener;
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

	public void OnPointerDown(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onDown != null)
		{
			onDown(base.gameObject);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onUp != null)
		{
			onUp(base.gameObject);
		}
	}
}
