using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenDownUpListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	public static PointerEventData pointEventData;

	public VoidDelegate onDown;

	public VoidDelegate onUp;

	public static int ClickNum;

	private void Awake()
	{
		ClickNum = 0;
	}

	public static ScreenDownUpListener Get(GameObject go)
	{
		ScreenDownUpListener screenDownUpListener = go.GetComponent<ScreenDownUpListener>();
		if (screenDownUpListener == null)
		{
			screenDownUpListener = go.AddComponent<ScreenDownUpListener>();
		}
		return screenDownUpListener;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onDown != null)
		{
			onDown(base.gameObject);
			ClickNum++;
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onUp != null)
		{
			onUp(base.gameObject);
			ClickNum--;
		}
	}
}
