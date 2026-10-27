using UnityEngine;
using UnityEngine.EventSystems;

public class PressedListener : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	public VoidDelegate onPressed;

	private bool isPointerDown;

	public static PressedListener Get(GameObject go)
	{
		PressedListener pressedListener = go.GetComponent<PressedListener>();
		if (pressedListener == null)
		{
			pressedListener = go.AddComponent<PressedListener>();
		}
		return pressedListener;
	}

	private void Update()
	{
		if (isPointerDown && onPressed != null)
		{
			onPressed(base.gameObject);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		isPointerDown = true;
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
