using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.smelter.scmsg;

namespace SC.UI
{
	public class TurretSplitPanel : View
	{
		public delegate void TurretSplitPanelCallBack(long instanceId, int itemId, int num, int index);

		private int _instanceId;

		private int _useNum = 1;

		public static long InstanceId;

		public static int ItemId;

		public static int Number;

		public static int Index;

		private static TurretSplitPanelCallBack _callBack;

		private Slider _slider;

		private GameObject btn_close;

		private GameObject txt_chai_fen;

		private Text txt_chai_fenText;

		private GameObject txt_xuan_ze_shu_liang;

		private Text txt_xuan_ze_shu_liangText;

		private GameObject m_icon;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject txt_has_num;

		private Text txt_has_numText;

		private GameObject txt_split_num;

		private Text txt_split_numText;

		private GameObject m_split_sld;

		private GameObject btn_up;

		private GameObject btn_down;

		private GameObject btn_confirm;

		private GameObject btn_rong_lu;

		private GameObject btn_ying_huo;

		protected override void onInit()
		{
			btn_rong_lu.SetActiveBetter(false);
			btn_ying_huo.SetActiveBetter(false);
			_slider = m_split_sld.GetComponent<Slider>();
			_slider.onValueChanged.AddListener(OnSldValueChange);
			_slider.wholeNumbers = true;
			ClickListener.Get(btn_confirm, string.Empty).onClick = OnClickConfirm;
			ClickListener.Get(btn_up, string.Empty).onClick = OnClickUp;
			ClickListener.Get(btn_down, string.Empty).onClick = OnClickDown;
			ClickListener.Get(btn_close, string.Empty).onClick = OnClose;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			ItemCfg itemCfg = ItemCfg.Get(ItemId);
			View.SetLabelText(txt_nameText, itemCfg.name);
			View.SetLabelText(txt_has_numText, Number);
			View.SetItemSprite(m_icon, itemCfg.icon);
			_slider.maxValue = Number;
			int num = (_useNum = Singleton<BagMgr>.Ins.GetItemNum(ItemId));
			View.SetSlider(m_split_sld, _useNum);
			View.SetLabelText(txt_split_numText, _useNum);
		}

		protected override void onHide(string childView = null)
		{
		}

		protected override void onDestroy()
		{
		}

		private void OnClickConfirm(GameObject go)
		{
			if (_callBack != null)
			{
				_callBack(InstanceId, ItemId, _useNum, Index);
			}
			Hide();
		}

		private void OnClickUp(GameObject go)
		{
			if (_useNum < Number)
			{
				_useNum++;
				View.SetSlider(m_split_sld, _useNum);
				View.SetLabelText(txt_split_numText, _useNum);
			}
		}

		private void OnClickDown(GameObject go)
		{
			if (_useNum > 1)
			{
				_useNum--;
				View.SetSlider(m_split_sld, _useNum);
				View.SetLabelText(txt_split_numText, _useNum);
			}
		}

		private void OnSldValueChange(float value)
		{
			_useNum = (int)value;
			View.SetLabelText(txt_split_numText, _useNum);
		}

		private void OnClose(GameObject go)
		{
			Hide();
		}

		public static void ShowSplitPanel(BagItem bagitem, int index, TurretSplitPanelCallBack callBack)
		{
			ShowSplitPanel(bagitem.instanceId, bagitem.itemId, bagitem.number, index, callBack);
		}

		public static void ShowSplitPanel(UseItem useItem, int index, TurretSplitPanelCallBack callBack)
		{
			ShowSplitPanel(useItem.instanceId, useItem.id, useItem.num, index, callBack);
		}

		public static void ShowSplitPanel(long instanceId, int itemId, int num, int index, TurretSplitPanelCallBack callBack)
		{
			InstanceId = instanceId;
			ItemId = itemId;
			Number = num;
			Index = index;
			_callBack = callBack;
			ViewMgr.Ins.ShowView<TurretSplitPanel>(null, false);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			txt_chai_fen = component.GameObjects[1].gameObject;
			txt_chai_fenText = txt_chai_fen.GetComponent<Text>();
			txt_xuan_ze_shu_liang = component.GameObjects[2].gameObject;
			txt_xuan_ze_shu_liangText = txt_xuan_ze_shu_liang.GetComponent<Text>();
			m_icon = component.GameObjects[3].gameObject;
			txt_name = component.GameObjects[4].gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_has_num = component.GameObjects[5].gameObject;
			txt_has_numText = txt_has_num.GetComponent<Text>();
			txt_split_num = component.GameObjects[6].gameObject;
			txt_split_numText = txt_split_num.GetComponent<Text>();
			m_split_sld = component.GameObjects[7].gameObject;
			btn_up = component.GameObjects[8].gameObject;
			btn_down = component.GameObjects[9].gameObject;
			btn_confirm = component.GameObjects[10].gameObject;
			btn_rong_lu = component.GameObjects[11].gameObject;
			btn_ying_huo = component.GameObjects[12].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}
