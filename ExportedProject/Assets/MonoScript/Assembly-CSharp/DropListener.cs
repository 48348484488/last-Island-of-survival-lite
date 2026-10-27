using UnityEngine;
using UnityEngine.EventSystems;

public class DropListener : MonoBehaviour, IDropHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	public static PointerEventData pointEventData;

	public VoidDelegate onDrop;

	public VoidDelegate onEnter;

	public static DropListener Get(GameObject go)
	{
		DropListener dropListener = go.GetComponent<DropListener>();
		if (dropListener == null)
		{
			dropListener = go.AddComponent<DropListener>();
		}
		return dropListener;
	}

	public void OnDrop(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (onDrop != null)
		{
			onDrop(base.gameObject);
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
