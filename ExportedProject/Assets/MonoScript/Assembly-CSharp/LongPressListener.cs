using UnityEngine;
using UnityEngine.EventSystems;

public class LongPressListener : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	public float durationThreshold = 1f;

	public VoidDelegate onLongPress;

	private bool isPointerDown;

	private bool longPressTriggered;

	private float timePressStarted;

	public static LongPressListener Get(GameObject go)
	{
		LongPressListener longPressListener = go.GetComponent<LongPressListener>();
		if (longPressListener == null)
		{
			longPressListener = go.AddComponent<LongPressListener>();
		}
		return longPressListener;
	}

	private void Update()
	{
		if (isPointerDown && !longPressTriggered && Time.time - timePressStarted > durationThreshold)
		{
			longPressTriggered = true;
			if (onLongPress != null)
			{
				onLongPress(base.gameObject);
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		timePressStarted = Time.time;
		isPointerDown = true;
		longPressTriggered = false;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isPointerDown = false;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isPointerDown = false;
	}
}
