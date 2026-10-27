using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	public static PointerEventData pointEventData;

	public string SoundName;

	public VoidDelegate onClick;

	public VoidDelegate onDown;

	public VoidDelegate onUp;

	public VoidDelegate onEnter;

	public VoidDelegate onExit;

	public VoidDelegate onBeginDrag;

	public VoidDelegate onDrag;

	public VoidDelegate onEndDrag;

	public object parameter;

	public static UIEventListener Get(GameObject go, string soundName = "")
	{
		UIEventListener uIEventListener = go.GetComponent<UIEventListener>();
		if (uIEventListener == null)
		{
			uIEventListener = go.AddComponent<UIEventListener>();
		}
		uIEventListener.SoundName = soundName;
		return uIEventListener;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onClick != null)
		{
			onClick(base.gameObject);
		}
		eventData.Reset();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onDown != null)
		{
			onDown(base.gameObject);
		}
		eventData.Reset();
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onUp != null)
		{
			onUp(base.gameObject);
		}
		eventData.Reset();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onEnter != null)
		{
			onEnter(base.gameObject);
		}
		eventData.Reset();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onExit != null)
		{
			onExit(base.gameObject);
		}
		eventData.Reset();
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
		eventData.Reset();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onEndDrag != null)
		{
			onEndDrag(base.gameObject);
		}
		eventData.Reset();
	}
}
