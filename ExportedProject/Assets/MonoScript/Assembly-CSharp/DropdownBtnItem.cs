using UnityEngine;
using UnityEngine.EventSystems;

public class DropdownBtnItem : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public GameObject btn;

	public GameObject Mask;

	public float MaskHeight;

	public GameObject Open;

	public GameObject Close;

	public int Index;

	public Utils.IntDelegate OnClickBtn;

	public GameObject DefaultBtn;

	public bool IsChecked;

	public void OnPointerClick(PointerEventData eventData)
	{
		EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
		eventData.Use();
		if (DropdownBtns._currentRadioBtnIndex != Index)
		{
		}
		Utils.TriggerEvent(OnClickBtn, Index);
	}
}
