using UnityEngine;
using UnityEngine.EventSystems;

public class PressListener : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	[Tooltip("How long must pointer be down on this object to trigger a long press")]
	public float DurationThreshold = 0.6f;

	public static PointerEventData PointEventData;

	private bool _isPointerDown;

	private bool _longPressTriggered;

	private float _timePressStarted;

	public VoidDelegate onPress;

	private void Update()
	{
		if (_isPointerDown && !_longPressTriggered && Time.time - _timePressStarted > DurationThreshold)
		{
			_longPressTriggered = true;
			onPress(base.gameObject);
		}
	}

	public static PressListener Get(GameObject go)
	{
		PressListener pressListener = go.GetComponent<PressListener>();
		if (pressListener == null)
		{
			pressListener = go.AddComponent<PressListener>();
		}
		return pressListener;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_timePressStarted = Time.time;
		_isPointerDown = true;
		_longPressTriggered = false;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_isPointerDown = false;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_isPointerDown = false;
	}
}
