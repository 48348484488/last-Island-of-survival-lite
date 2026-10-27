using UnityEngine;
using UnityEngine.EventSystems;

public class EnterExitListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	public static PointerEventData pointEventData;

	public VoidDelegate onEnter;

	public VoidDelegate onExit;

	public static EnterExitListener Get(GameObject go)
	{
		EnterExitListener enterExitListener = go.GetComponent<EnterExitListener>();
		if (enterExitListener == null)
		{
			enterExitListener = go.AddComponent<EnterExitListener>();
		}
		return enterExitListener;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onEnter != null)
		{
			onEnter(base.gameObject);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onExit != null)
		{
			onExit(base.gameObject);
		}
	}
}
