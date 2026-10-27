using UnityEngine;
using UnityEngine.EventSystems;

public class ClickListener : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public delegate void VoidDelegate(GameObject go);

	public static PointerEventData pointEventData;

	public VoidDelegate onClick;

	public VoidDelegate onClickLimit05s;

	public VoidDelegate onClickLimit1s;

	public VoidDelegate onClickLimit2s;

	public string SoundName;

	public static ClickListener Get(GameObject go, string soundName = "")
	{
		ClickListener clickListener = go.GetComponent<ClickListener>();
		if (clickListener == null)
		{
			clickListener = go.AddComponent<ClickListener>();
		}
		clickListener.SoundName = soundName;
		return clickListener;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		pointEventData = eventData;
		if (string.IsNullOrEmpty(SoundName))
		{
			SingletonMono<AudioManager>.Ins.Play2D("ui_dianji");
		}
		else if (!(SoundName == "no"))
		{
			SingletonMono<AudioManager>.Ins.Play2D(SoundName);
		}
		if (onClick != null)
		{
			onClick(base.gameObject);
		}
		if (onClickLimit05s != null && SingletonMono<ClickManager>.Ins.LimitTime <= 0f)
		{
			onClickLimit05s(base.gameObject);
			SingletonMono<ClickManager>.Ins.LimitTime = 0.5f;
		}
		if (onClickLimit1s != null && SingletonMono<ClickManager>.Ins.LimitTime <= 0f)
		{
			onClickLimit1s(base.gameObject);
			SingletonMono<ClickManager>.Ins.LimitTime = 1f;
		}
		if (onClickLimit2s != null && SingletonMono<ClickManager>.Ins.LimitTime <= 0f)
		{
			onClickLimit2s(base.gameObject);
			SingletonMono<ClickManager>.Ins.LimitTime = 2f;
		}
	}
}
