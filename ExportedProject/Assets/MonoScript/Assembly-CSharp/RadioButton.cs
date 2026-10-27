using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadioButton : Selectable, IPointerClickHandler, IEventSystemHandler
{
	private static Dictionary<string, List<RadioButton>> checkButtons = new Dictionary<string, List<RadioButton>>();

	public string id;

	public Utils.BoolDelegate OnValueChanged;

	[SerializeField]
	public string group;

	[SerializeField]
	private GameObject backgroundOn;

	[SerializeField]
	private GameObject backgroundOff;

	[SerializeField]
	private bool _isChecked;

	public bool isChecked
	{
		get
		{
			return _isChecked;
		}
		set
		{
			bool flag = _isChecked;
			_isChecked = value;
			backgroundOn.SetActive(_isChecked);
			backgroundOff.SetActive(!_isChecked);
			if (flag != value && OnValueChanged != null)
			{
				OnValueChanged(_isChecked);
			}
		}
	}

	protected override void Awake()
	{
		List<RadioButton> value = null;
		if (!checkButtons.TryGetValue(group, out value))
		{
			value = new List<RadioButton>();
			checkButtons.Add(group, value);
		}
		value.Add(this);
		isChecked = _isChecked;
	}

	protected override void OnDestroy()
	{
		List<RadioButton> value;
		if (checkButtons.TryGetValue(group, out value))
		{
			value.Remove(this);
		}
	}

	private bool MayDrag(PointerEventData eventData)
	{
		return IsActive() && IsInteractable() && eventData.button == PointerEventData.InputButton.Left;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!MayDrag(eventData))
		{
			return;
		}
		EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
		List<RadioButton> value;
		if (checkButtons.TryGetValue(group, out value))
		{
			for (int i = 0; i < value.Count; i++)
			{
				RadioButton radioButton = value[i];
				radioButton.isChecked = radioButton == this;
			}
		}
		base.OnPointerUp(eventData);
		eventData.Use();
	}

	public static void ChooseBtn(GameObject btn)
	{
		RadioButton component = btn.GetComponent<RadioButton>();
		List<RadioButton> value;
		if (checkButtons.TryGetValue(component.group, out value))
		{
			for (int i = 0; i < value.Count; i++)
			{
				RadioButton radioButton = value[i];
				radioButton.isChecked = radioButton == component;
			}
		}
	}

	public static void AllGroupBtnOff(GameObject btn)
	{
		RadioButton component = btn.GetComponent<RadioButton>();
		List<RadioButton> value;
		if (checkButtons.TryGetValue(component.group, out value))
		{
			for (int i = 0; i < value.Count; i++)
			{
				RadioButton radioButton = value[i];
				radioButton.isChecked = false;
			}
		}
	}
}
