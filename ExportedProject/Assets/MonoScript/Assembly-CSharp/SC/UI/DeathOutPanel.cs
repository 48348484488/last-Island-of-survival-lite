using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;

namespace SC.UI
{
	public class DeathOutPanel : View
	{
		private UIScrollPanel _lossItemsScrollPanel;

		private Dictionary<int, int> _dropedItemsOffline;

		private List<int> _dropedItemsOfflineKeys = new List<int>();

		private GameObject m_bg;

		private GameObject m_panel_root;

		private GameObject scp_loss;

		private GDeathOutPanelItem m_cell_1;

		private GameObject btn_ok;

		protected override void onInit()
		{
			m_cell_1.gameObject.SetActiveBetter(false);
			ClickListener.Get(btn_ok, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(m_bg, string.Empty).onClick = _003ConInit_003Em__1;
			_lossItemsScrollPanel = scp_loss.GetComponent<UIScrollPanel>();
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_dropedItemsOffline = Singleton<RebirthMgr>.Ins.DropedItemsOffline;
			_dropedItemsOfflineKeys = new List<int>(_dropedItemsOffline.Keys);
			SetLossItems();
		}

		protected override void onHide(string childView = null)
		{
			Singleton<RebirthMgr>.Ins.ClearDropedItemsOffline();
		}

		protected override void onDestroy()
		{
		}

		private void SetLossItems()
		{
			_lossItemsScrollPanel.Clear();
			_lossItemsScrollPanel.ResetNoPos(_dropedItemsOfflineKeys.Count, LossItemData);
		}

		private void LossItemData(GameObject go, int index)
		{
			GDeathOutPanelItem component = go.GetComponent<GDeathOutPanelItem>();
			int key = _dropedItemsOfflineKeys[index];
			int num = _dropedItemsOffline[key];
			ItemCfg itemCfg = ItemCfg.Get(key);
			View.SetItemSprite(component.m_icon, itemCfg.icon);
			View.SetLabelText(component.txt_numText, num);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_bg = component.GameObjects[0].gameObject;
			m_panel_root = component.GameObjects[1].gameObject;
			scp_loss = component.GameObjects[2].gameObject;
			m_cell_1 = View.AddComponentIfNotExist<GDeathOutPanelItem>(component.GameObjects[3].gameObject);
			btn_ok = component.GameObjects[4].gameObject;
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
