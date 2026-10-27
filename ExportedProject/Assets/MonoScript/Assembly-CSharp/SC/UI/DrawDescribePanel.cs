using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class DrawDescribePanel : View
	{
		private int _itemId;

		private GameObject m_recording;

		private GameObject m_rec_bg;

		private GameObject btn_close_rec;

		private GameObject m_icon;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject txt_desc;

		private Text txt_descText;

		private GameObject btn_go;

		private GameObject btn_confirm;

		protected override void onInit()
		{
			ClickListener.Get(btn_confirm, string.Empty).onClick = OnClickConfirm;
			ClickListener.Get(btn_go, string.Empty).onClick = OnClickGo;
			ClickListener.Get(btn_close_rec, string.Empty).onClick = OnClickClose;
			ClickListener.Get(m_rec_bg, string.Empty).onClick = OnClickClose;
		}

		private void OnClickClose(GameObject go)
		{
			Hide();
		}

		private void OnClickGo(GameObject go)
		{
			Hide();
			ViewMgr.Ins.GetView<BagAndBuildPanel>().ToBuild(_itemId);
		}

		private void OnClickConfirm(GameObject go)
		{
			Hide();
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_itemId = (int)param;
			ItemCfg itemCfg = ItemCfg.Get(_itemId);
			View.SetItemSprite(m_icon, itemCfg.icon);
			View.SetLabelText(txt_nameText, itemCfg.name);
			View.SetLabelText(txt_descText, itemCfg.desc);
		}

		protected override void onHide(string childView = null)
		{
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_recording = component.GameObjects[0].gameObject;
			m_rec_bg = component.GameObjects[1].gameObject;
			btn_close_rec = component.GameObjects[2].gameObject;
			m_icon = component.GameObjects[3].gameObject;
			txt_name = component.GameObjects[4].gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_desc = component.GameObjects[5].gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
			btn_go = component.GameObjects[6].gameObject;
			btn_confirm = component.GameObjects[7].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}
