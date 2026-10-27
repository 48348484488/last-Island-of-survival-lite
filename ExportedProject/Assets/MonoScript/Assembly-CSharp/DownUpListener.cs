using UnityEngine;
using UnityEngine.EventSystems;

public class DownUpListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	public static PointerEventData pointEventData;

	public VoidDelegate onDown;

	public VoidDelegate onUp;

	public static DownUpListener Get(GameObject go)
	{
		DownUpListener downUpListener = go.GetComponent<DownUpListener>();
		if (downUpListener == null)
		{
			downUpListener = go.AddComponent<DownUpListener>();
		}
		return downUpListener;
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
