using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class ElectricAddItemPanel : View
	{
		public class ShowArg
		{
			public long InsId;

			public int ItemId;

			public int MaxCanAddNum;

			public byte Index;
		}

		private ShowArg _arg;

		private int _useNum;

		private InputField _inputField;

		private GameObject btn_close;

		private GameObject m_icon;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject txt_has_num;

		private Text txt_has_numText;

		private GameObject btn_up;

		private GameObject btn_down;

		private GameObject inp_num;

		private GameObject txt_split_num;

		private Text txt_split_numText;

		private GameObject btn_max;

		private GameObject btn_confirm;

		protected override void onInit()
		{
			_inputField = inp_num.GetComponent<InputField>();
			_inputField.onValueChanged.AddListener(OnInputValueChange);
			ClickListener.Get(btn_confirm, string.Empty).onClick = OnClickConfirm;
			ClickListener.Get(btn_up, string.Empty).onClick = OnClickUp;
			ClickListener.Get(btn_down, string.Empty).onClick = OnClickDown;
			ClickListener.Get(btn_close, string.Empty).onClick = OnClose;
			ClickListener.Get(btn_max, string.Empty).onClick = OnClickMax;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_arg = (ShowArg)param;
			int itemNum = Singleton<BagMgr>.Ins.GetItemNum(_arg.ItemId, false);
			ItemCfg itemCfg = ItemCfg.Get(_arg.ItemId);
			View.SetLabelText(txt_nameText, itemCfg.name);
			View.SetLabelText(txt_has_numText, itemNum);
			View.SetItemSprite(m_icon, itemCfg.icon);
			_inputField.text = (_arg.MaxCanAddNum / 2).ToString();
		}

		private void SplitItemSucess()
		{
			Hide();
		}

		protected override void onHide(string childView = null)
		{
		}

		private void OnClickConfirm(GameObject go)
		{
			if (_useNum < 1 || _useNum > _arg.MaxCanAddNum)
			{
				Hide();
				return;
			}
			Singleton<ElectricityMgr>.Ins.SendAddFuelMsg(_arg.InsId, _arg.Index, _arg.ItemId, _useNum);
			Hide();
		}

		private void OnClickUp(GameObject go)
		{
			if (_useNum < _arg.MaxCanAddNum)
			{
				_useNum++;
				_inputField.text = _useNum.ToString();
			}
		}

		private void OnClickDown(GameObject go)
		{
			if (_useNum > 1)
			{
				_useNum--;
				_inputField.text = _useNum.ToString();
			}
		}

		private void OnClickMax(GameObject go)
		{
			_inputField.text = _arg.MaxCanAddNum.ToString();
		}

		private void OnInputValueChange(string value)
		{
			_useNum = int.Parse(value);
			if (_useNum > _arg.MaxCanAddNum)
			{
				_useNum = _arg.MaxCanAddNum;
			}
			View.SetLabelText(txt_split_numText, _useNum);
		}

		private void OnClose(GameObject go)
		{
			Hide();
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			m_icon = component.GameObjects[1].gameObject;
			txt_name = component.GameObjects[2].gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_has_num = component.GameObjects[3].gameObject;
			txt_has_numText = txt_has_num.GetComponent<Text>();
			btn_up = component.GameObjects[4].gameObject;
			btn_down = component.GameObjects[5].gameObject;
			inp_num = component.GameObjects[6].gameObject;
			txt_split_num = component.GameObjects[7].gameObject;
			txt_split_numText = txt_split_num.GetComponent<Text>();
			btn_max = component.GameObjects[8].gameObject;
			btn_confirm = component.GameObjects[9].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}
