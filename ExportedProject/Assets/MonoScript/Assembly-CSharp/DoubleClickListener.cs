using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickListener : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go, Vector2 data);

	public PointerEventData pointEventData;

	public VoidDelegate onSingleClick;

	public VoidDelegate onDoubleClick;

	public string SoundName;

	private Vector2 clickPos = default(Vector2);

	private bool isExecute;

	private int count;

	private float ClickTime;

	public static DoubleClickListener Get(GameObject go)
	{
		DoubleClickListener doubleClickListener = go.GetComponent<DoubleClickListener>();
		if (doubleClickListener == null)
		{
			doubleClickListener = go.AddComponent<DoubleClickListener>();
		}
		return doubleClickListener;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isExecute = !eventData.dragging;
		if (eventData.dragging)
		{
			count = 0;
			ClickTime = 0f;
			clickPos = Vector2.zero;
			return;
		}
		if (count == 0)
		{
			ClickTime = 0.3f;
		}
		if (Mathf.Abs(clickPos.x - eventData.position.x) < 50f && Mathf.Abs(clickPos.y - eventData.position.y) < 50f && ClickTime > 0f)
		{
			count++;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (count == 0)
		{
			clickPos.x = eventData.position.x;
			clickPos.y = eventData.position.y;
		}
	}

	private void Update()
	{
		if (ClickTime <= 0f)
		{
			return;
		}
		ClickTime -= Time.deltaTime;
		if (!(ClickTime <= 0f))
		{
			return;
		}
		if (isExecute)
		{
			if (count == 2)
			{
				if (onDoubleClick != null)
				{
					onDoubleClick(base.gameObject, clickPos);
				}
			}
			else if (count == 1 && onSingleClick != null)
			{
				onSingleClick(base.gameObject, clickPos);
			}
		}
		clickPos = Vector2.zero;
		ClickTime = 0f;
		count = 0;
	}
}
