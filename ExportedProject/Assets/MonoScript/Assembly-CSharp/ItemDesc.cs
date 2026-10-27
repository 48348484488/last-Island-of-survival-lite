using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class ItemDesc : View
{
	private float Width;

	private float Height;

	private const int OFF_XY = 20;

	private GameObject txt_des;

	private Text txt_desText;

	private GameObject m_itemHead;

	private GameObject m_itemDesc;

	private GameObject btn_back;

	private GameObject m_haveNum;

	private GameObject txt_haveNum;

	private Text txt_haveNumText;

	protected override void onInit()
	{
		UIEventListener.Get(btn_back, string.Empty).onClick = _003ConInit_003Em__0;
		UIEventListener.Get(btn_back, string.Empty).onUp = _003ConInit_003Em__1;
		Height = (m_itemDesc.transform as RectTransform).rect.height;
		Width = (m_itemDesc.transform as RectTransform).rect.width;
	}

	protected override void onShow(object param = null, string childView = null)
	{
		ItemHead.ItemDescInfo itemDescInfo = param as ItemHead.ItemDescInfo;
		if (itemDescInfo != null)
		{
			string itemDesc = Singleton<DropMgr>.Ins.getItemDesc(itemDescInfo.info.type, itemDescInfo.info.itemId);
			View.SetLabelText(txt_des, itemDesc);
			ItemHead component = m_itemHead.GetComponent<ItemHead>();
			component.Fill(itemDescInfo.info);
			m_haveNum.SetActive(false);
			m_itemDesc.transform.position = itemDescInfo.v3;
			Vector3 localPosition = m_itemDesc.transform.localPosition;
			float y = ((!(localPosition.y + Height < ViewMgr.Ins.CanvasSize.y / 2f)) ? (localPosition.y - Height / 2f - 20f) : (localPosition.y + Height / 2f + 20f));
			m_itemDesc.transform.localPosition = new Vector3(itemDescInfo.v3.x, y, 1f);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
		txt_des = component.GameObjects[0].gameObject;
		txt_desText = txt_des.GetComponent<Text>();
		m_itemHead = component.GameObjects[1].gameObject;
		m_itemDesc = component.GameObjects[2].gameObject;
		btn_back = component.GameObjects[3].gameObject;
		m_haveNum = component.GameObjects[4].gameObject;
		txt_haveNum = component.GameObjects[5].gameObject;
		txt_haveNumText = txt_haveNum.GetComponent<Text>();
		ViewMgr.Ins.addView(this);
	}

	[CompilerGenerated]
	private void _003ConInit_003Em__0(GameObject go)
	{
		Hide();
	}

	[CompilerGenerated]
	private void _003ConInit_003Em__1(GameObject go)
	{
		Hide();
	}
}
