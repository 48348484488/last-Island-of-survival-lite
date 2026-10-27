using UnityEngine;
using UnityEngine.EventSystems;

public class YaoganDownUpListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	public static PointerEventData pointEventData;

	public VoidDelegate onDown;

	public VoidDelegate onUp;

	private int pointerId = -1;

	public static YaoganDownUpListener Get(GameObject go)
	{
		YaoganDownUpListener yaoganDownUpListener = go.GetComponent<YaoganDownUpListener>();
		if (yaoganDownUpListener == null)
		{
			yaoganDownUpListener = go.AddComponent<YaoganDownUpListener>();
		}
		return yaoganDownUpListener;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.pointerId == pointerId)
		{
			pointEventData = eventData;
			if (onDown != null)
			{
				onDown(base.gameObject);
			}
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

	public void OnPointerEnter(PointerEventData eventData)
	{
		pointerId = eventData.pointerId;
	}
}
