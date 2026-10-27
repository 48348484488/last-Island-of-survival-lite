using System;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.bag.scmsg;
using gs.smelter.scmsg;

namespace SC.UI
{
	public class BagSplitPanel : View
	{
		private int _instanceId;

		private int _useNum = 1;

		private BagItem _bagItem;

		private InputField _inputField;

		private BagItem bagItem = new BagItem();

		private GameObject btn_close;

		private GameObject m_icon;

		private GameObject txt_name;

		private Text txt_nameText;

		private GameObject txt_has_num;

		private Text txt_has_numText;

		private GameObject txt_split_num;

		private Text txt_split_numText;

		private GameObject btn_up;

		private GameObject btn_down;

		private GameObject btn_confirm;

		private GameObject txt_chai_fen;

		private Text txt_chai_fenText;

		private GameObject txt_xuan_ze_shu_liang;

		private Text txt_xuan_ze_shu_liangText;

		private GameObject btn_rong_lu;

		private GameObject btn_ying_huo;

		private GameObject btn_ling_di_gui;

		private GameObject btn_max;

		private GameObject inp_num;

		private GameObject btn_one_to_multi;

		protected override void onInit()
		{
			_inputField = inp_num.GetComponent<InputField>();
			_inputField.onValueChanged.AddListener(OnInputValueChange);
			ClickListener.Get(btn_confirm, string.Empty).onClick = OnClickConfirm;
			ClickListener.Get(btn_up, string.Empty).onClick = OnClickUp;
			ClickListener.Get(btn_down, string.Empty).onClick = OnClickDown;
			ClickListener.Get(btn_close, string.Empty).onClick = OnClose;
			ClickListener.Get(btn_rong_lu, string.Empty).onClick = OnClickRongLu;
			ClickListener.Get(btn_ying_huo, string.Empty).onClick = OnClickYingHuo;
			ClickListener.Get(btn_ling_di_gui, string.Empty).onClick = OnClickLingDiGui;
			ClickListener.Get(btn_max, string.Empty).onClick = OnClickMax;
			ClickListener.Get(btn_one_to_multi, string.Empty).onClick = OnClickOneToMulti;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_instanceId = (int)param;
			btn_confirm.SetActiveBetter(ViewMgr.Ins.IsShow<BagAndBuildPanel>());
			btn_rong_lu.SetActiveBetter(ViewMgr.Ins.IsShow<FurnacePanel>());
			btn_ying_huo.SetActiveBetter(ViewMgr.Ins.IsShow<FirePanel>());
			btn_ling_di_gui.SetActiveBetter(ViewMgr.Ins.IsShow<ToolboxPanel>());
			btn_one_to_multi.SetActiveBetter(ViewMgr.Ins.IsShow<FenjiejiPanel>());
			txt_chai_fen.SetActiveBetter(btn_confirm.activeSelf);
			txt_xuan_ze_shu_liang.SetActiveBetter(btn_rong_lu.activeSelf || btn_ying_huo.activeSelf || btn_ling_di_gui.activeSelf);
			_bagItem = Singleton<BagMgr>.Ins.GetAllItemByInstanceId(_instanceId);
			ItemCfg itemCfg = ItemCfg.Get(_bagItem.itemId);
			View.SetLabelText(txt_nameText, itemCfg.name);
			View.SetLabelText(txt_has_numText, _bagItem.number);
			View.SetItemSprite(m_icon, itemCfg.icon);
			_inputField.text = (_bagItem.number / 2).ToString();
			BagEvent.SplitItemSucess = (Utils.VoidDelegate)Delegate.Combine(BagEvent.SplitItemSucess, new Utils.VoidDelegate(SplitItemSucess));
		}

		private void SplitItemSucess()
		{
			Hide();
		}

		protected override void onHide(string childView = null)
		{
			BagEvent.SplitItemSucess = (Utils.VoidDelegate)Delegate.Remove(BagEvent.SplitItemSucess, new Utils.VoidDelegate(SplitItemSucess));
		}

		protected override void onDestroy()
		{
		}

		private void OnClickRongLu(GameObject go)
		{
			if (Singleton<FurnaceMgr>.Ins.AiJiaShaJiaSha != null && Singleton<FurnaceMgr>.Ins.JiaShaA != null)
			{
				bagItem.itemId = Singleton<FurnaceMgr>.Ins.JiaShaA.itemId;
				bagItem.instanceId = Singleton<FurnaceMgr>.Ins.JiaShaA.instanceId;
				bagItem.duration = Singleton<FurnaceMgr>.Ins.JiaShaA.duration;
				bagItem.extraInfo = Singleton<FurnaceMgr>.Ins.JiaShaA.extraInfo;
				bagItem.timeout = Singleton<FurnaceMgr>.Ins.JiaShaA.timeout;
				bagItem.number = _useNum;
				Singleton<FurnaceMgr>.Ins.JiaShaA = bagItem;
				Singleton<FurnaceMgr>.Ins.AiJiaShaJiaSha();
				Hide();
			}
		}

		private void OnClickYingHuo(GameObject go)
		{
			if (_useNum > Singleton<CookMgr>.Ins.CanAddFuelNum)
			{
				AlertBox.Show(238);
				return;
			}
			UseItem useItem = new UseItem();
			useItem.id = _bagItem.itemId;
			useItem.num = _useNum;
			useItem.instanceId = _bagItem.instanceId;
			Singleton<CookMgr>.Ins.SendAddFuelMsg(useItem);
			Hide();
		}

		private void OnClickLingDiGui(GameObject go)
		{
			if (BagEvent.OnClickLingDiGuiBtn != null)
			{
				BagEvent.OnClickLingDiGuiBtn(_bagItem, _useNum);
			}
			Hide();
		}

		private void OnClickOneToMulti(GameObject go)
		{
			if (Singleton<FacilityOneToMultiMgr>.Ins.AiJiaShaJiaSha != null && Singleton<FacilityOneToMultiMgr>.Ins.JiaShaA != null)
			{
				bagItem.itemId = Singleton<FacilityOneToMultiMgr>.Ins.JiaShaA.itemId;
				bagItem.instanceId = Singleton<FacilityOneToMultiMgr>.Ins.JiaShaA.instanceId;
				bagItem.duration = Singleton<FacilityOneToMultiMgr>.Ins.JiaShaA.duration;
				bagItem.extraInfo = Singleton<FacilityOneToMultiMgr>.Ins.JiaShaA.extraInfo;
				bagItem.timeout = Singleton<FacilityOneToMultiMgr>.Ins.JiaShaA.timeout;
				bagItem.number = _useNum;
				Singleton<FacilityOneToMultiMgr>.Ins.JiaShaA = bagItem;
				Singleton<FacilityOneToMultiMgr>.Ins.AiJiaShaJiaSha();
				Hide();
			}
		}

		private void OnClickConfirm(GameObject go)
		{
			if (_useNum < 1 || _useNum >= _bagItem.number)
			{
				AlertBox.Show(346);
			}
			else
			{
				Singleton<BagMgr>.Ins.BreakItem(_instanceId, _useNum);
			}
		}

		private void OnClickUp(GameObject go)
		{
			if (_useNum < _bagItem.number)
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
			_inputField.text = _bagItem.number.ToString();
		}

		private void OnInputValueChange(string value)
		{
			_useNum = int.Parse(value);
			if (_useNum > _bagItem.number)
			{
				_useNum = _bagItem.number;
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
			txt_split_num = component.GameObjects[4].gameObject;
			txt_split_numText = txt_split_num.GetComponent<Text>();
			btn_up = component.GameObjects[5].gameObject;
			btn_down = component.GameObjects[6].gameObject;
			btn_confirm = component.GameObjects[7].gameObject;
			txt_chai_fen = component.GameObjects[8].gameObject;
			txt_chai_fenText = txt_chai_fen.GetComponent<Text>();
			txt_xuan_ze_shu_liang = component.GameObjects[9].gameObject;
			txt_xuan_ze_shu_liangText = txt_xuan_ze_shu_liang.GetComponent<Text>();
			btn_rong_lu = component.GameObjects[10].gameObject;
			btn_ying_huo = component.GameObjects[11].gameObject;
			btn_ling_di_gui = component.GameObjects[12].gameObject;
			btn_max = component.GameObjects[13].gameObject;
			inp_num = component.GameObjects[14].gameObject;
			btn_one_to_multi = component.GameObjects[15].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}
