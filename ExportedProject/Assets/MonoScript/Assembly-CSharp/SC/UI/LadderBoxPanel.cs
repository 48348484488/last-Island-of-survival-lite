using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class LadderBoxPanel : View
	{
		private UIScrollPanel _bagScrollPanel;

		private int _itemId;

		private OpenableBoxCfg _openableBoxCfg;

		private ItemCfg _itemCfg;

		private List<DropMgr.DropDesInfo> _dropDesInfos;

		private GameObject txt_title;

		private Text txt_titleText;

		private GameObject btn_close;

		private GameObject scp_cell;

		private GLadderBoxPanelItem m_cell;

		private GameObject m_bg;

		protected override void onInit()
		{
			_bagScrollPanel = scp_cell.GetComponent<UIScrollPanel>();
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(m_bg, string.Empty).onClick = _003ConInit_003Em__1;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_itemId = (int)param;
			if (_itemId > 0)
			{
				_openableBoxCfg = OpenableBoxCfg.Get(_itemId);
				_itemCfg = ItemCfg.Get(_itemId);
				View.SetLabelText(txt_titleText, _itemCfg.name);
				SetBagData();
			}
		}

		protected override void onHide(string childView = null)
		{
		}

		protected override void onDestroy()
		{
		}

		private void SetBagData()
		{
			int dropId = _itemCfg.dropId;
			_dropDesInfos = Singleton<DropMgr>.Ins.GetDropDetailInfo(dropId);
			_bagScrollPanel.Clear();
			_bagScrollPanel.Reset(_dropDesInfos.Count, FillBagItemData);
		}

		private void FillBagItemData(GameObject go, int index)
		{
			GLadderBoxPanelItem component = go.GetComponent<GLadderBoxPanelItem>();
			DropMgr.DropDesInfo dropDesInfo = _dropDesInfos[index];
			View.SetItemSprite(component.m_icon, dropDesInfo.icon);
			View.SetLabelText(component.txt_numText, dropDesInfo.num);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			txt_title = component.GameObjects[0].gameObject;
			txt_titleText = txt_title.GetComponent<Text>();
			btn_close = component.GameObjects[1].gameObject;
			scp_cell = component.GameObjects[2].gameObject;
			m_cell = View.AddComponentIfNotExist<GLadderBoxPanelItem>(component.GameObjects[3].gameObject);
			m_bg = component.GameObjects[4].gameObject;
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
}
