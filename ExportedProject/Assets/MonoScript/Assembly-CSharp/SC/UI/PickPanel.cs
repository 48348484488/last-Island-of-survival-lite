using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class PickPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillDropCell_003Ec__AnonStorey0
		{
			internal ItemCfg info;

			internal BoxCfg boxInfo;

			internal int index;

			internal int id;

			internal int num;

			internal long boxInstanceId;

			internal bool isInBox;

			internal void _003C_003Em__0(GameObject o)
			{
				PickItem(info, boxInfo, index, id, num, boxInstanceId, isInBox);
			}
		}

		public const byte Nothing = 0;

		public const byte ResetThingPos = 1;

		private UIScrollPanel _picksScroll;

		private RectTransform _parentRect;

		private float _cellRectX;

		private float _cellRectY;

		private int _num;

		private float _offsetting = 8f;

		private RectTransform _mPanelTrans;

		private RectTransform _mBtnTrans;

		private GameObject _thisGo;

		private Color32 _equipBlue = new Color32(60, 153, 225, byte.MaxValue);

		private Color32 _equipCheng = new Color32(byte.MaxValue, 169, 105, byte.MaxValue);

		private Color32 _equipRed = new Color32(248, 67, 62, byte.MaxValue);

		private float _showBluePercentage = 0.9f;

		private float _showChengPercentage = 0.5f;

		private Vector3 _qiangEuler = new Vector3(0f, 0f, 45f);

		private GameObject btn_pic;

		private GameObject m_panel;

		private GameObject m_bg;

		private GameObject scp_picks;

		private GBattlePickCell m_cell;

		private GameObject btn_close;

		protected override void onInit()
		{
			base.onInit();
			_thisGo = base.gameObject;
			m_cell.gameObject.SetActiveBetter(false);
			_picksScroll = scp_picks.GetComponent<UIScrollPanel>();
			_parentRect = m_bg.GetComponent<RectTransform>();
			RectTransform component = m_cell.GetComponent<RectTransform>();
			_cellRectX = component.sizeDelta.x;
			_cellRectY = component.sizeDelta.y;
			_mPanelTrans = m_panel.GetComponent<RectTransform>();
			_mBtnTrans = btn_pic.GetComponent<RectTransform>();
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_pic, string.Empty).onClick = _003ConInit_003Em__1;
			OnExitCustomSettingPanel();
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			if (!Battle.Ins || !Battle.Ins.SelfPlayer || ((bool)Battle.Ins.SelfPlayer && (Battle.Ins.SelfPlayer.IsDie || Battle.Ins.SelfPlayer.HP <= 0)) || Singleton<BattleDropMgr>.Ins.DropNum == 0)
			{
				OnHidePickPanel();
			}
			if (param != null && (byte)param == 1)
			{
				OnExitCustomSettingPanel();
			}
			UpdateBtnActives();
			if (Singleton<BattleDropMgr>.Ins.IsShowScroll)
			{
				UpdateDropPanel(false);
			}
			BattleDropEvent.DropNumChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(BattleDropEvent.DropNumChangeDelegate, new Utils.VoidDelegate(DropNumChange));
			SettingEvent.ExitCustomSaveSettingPanelDelegate = (Utils.VoidDelegate)Delegate.Combine(SettingEvent.ExitCustomSaveSettingPanelDelegate, new Utils.VoidDelegate(OnExitCustomSettingPanel));
			SettingEvent.ChangeGunModeDelegate = (Utils.IntDelegate)Delegate.Combine(SettingEvent.ChangeGunModeDelegate, new Utils.IntDelegate(OnChangeGunMode));
			BattleDropEvent.HidePickPanel = (Action)Delegate.Combine(BattleDropEvent.HidePickPanel, new Action(OnHidePickPanel));
			BattleDropEvent.ShowPickPanel = (Action<byte>)Delegate.Combine(BattleDropEvent.ShowPickPanel, new Action<byte>(OnShowPickPanel));
		}

		private void OnShowPickPanel(byte obj)
		{
			_thisGo.SetActiveBetter(true);
			if (!Battle.Ins || !Battle.Ins.SelfPlayer || ((bool)Battle.Ins.SelfPlayer && (Battle.Ins.SelfPlayer.IsDie || Battle.Ins.SelfPlayer.HP <= 0)) || Singleton<BattleDropMgr>.Ins.DropNum == 0)
			{
				OnHidePickPanel();
				return;
			}
			if (obj == 1)
			{
				OnExitCustomSettingPanel();
			}
			UpdateBtnActives();
			if (Singleton<BattleDropMgr>.Ins.IsShowScroll)
			{
				UpdateDropPanel(false);
			}
		}

		private void OnHidePickPanel()
		{
			_thisGo.SetActiveBetter(false);
		}

		private void OnChangeGunMode(int arg)
		{
			OnExitCustomSettingPanel();
		}

		private void OnExitCustomSettingPanel()
		{
		}

		private void DropNumChange()
		{
			if (Singleton<BattleDropMgr>.Ins.DropNum == 0)
			{
				OnHidePickPanel();
			}
			else if (Singleton<BattleDropMgr>.Ins.IsShowScroll)
			{
				UpdateDropPanel(true);
			}
		}

		private void UpdateBtnActives()
		{
			btn_pic.SetActiveBetter(!Singleton<BattleDropMgr>.Ins.IsShowScroll);
			m_panel.SetActiveBetter(Singleton<BattleDropMgr>.Ins.IsShowScroll);
		}

		private void UpdateDropPanel(bool isNoPos)
		{
			_num = Singleton<BattleDropMgr>.Ins.GetTotalCellNum();
			Vector2 sizeDelta = default(Vector2);
			if (_num < 6)
			{
				_picksScroll.Reset(_num, FillDropCell);
				sizeDelta.y = _offsetting + _cellRectY * (float)((_num <= 1) ? 2 : _num);
			}
			else
			{
				if (isNoPos)
				{
					_picksScroll.ResetNoPos(_num, FillDropCell);
				}
				else
				{
					_picksScroll.Reset(_num, FillDropCell);
				}
				sizeDelta.y = _offsetting + _cellRectY * 5f;
			}
			sizeDelta.x = _cellRectX;
			_parentRect.sizeDelta = sizeDelta;
		}

		private void FillDropCell(GameObject go, int index)
		{
			_003CFillDropCell_003Ec__AnonStorey0 _003CFillDropCell_003Ec__AnonStorey = new _003CFillDropCell_003Ec__AnonStorey0();
			_003CFillDropCell_003Ec__AnonStorey.index = index;
			GBattlePickCell component = go.GetComponent<GBattlePickCell>();
			_003CFillDropCell_003Ec__AnonStorey.isInBox = Singleton<BattleDropMgr>.Ins.GetIdAndNumber(_003CFillDropCell_003Ec__AnonStorey.index, out _003CFillDropCell_003Ec__AnonStorey.id, out _003CFillDropCell_003Ec__AnonStorey.num, out _003CFillDropCell_003Ec__AnonStorey.boxInstanceId);
			_003CFillDropCell_003Ec__AnonStorey.info = ItemCfg.Get(_003CFillDropCell_003Ec__AnonStorey.id);
			_003CFillDropCell_003Ec__AnonStorey.boxInfo = BoxCfg.Get(-_003CFillDropCell_003Ec__AnonStorey.id);
			component.m_novice_effect.SetActiveBetter(Singleton<GuideMgr>.Ins.IsShowPickEffect(_003CFillDropCell_003Ec__AnonStorey.id));
			component.m_hl_bg.SetActiveBetter(false);
			component.m_armor_value.SetActiveBetter(false);
			Transform component2 = component.m_icon.GetComponent<Transform>();
			if (_003CFillDropCell_003Ec__AnonStorey.info != null)
			{
				if (_003CFillDropCell_003Ec__AnonStorey.info.type != 13 && _003CFillDropCell_003Ec__AnonStorey.info.type != 17 && _003CFillDropCell_003Ec__AnonStorey.info.type != 35)
				{
					component.txt_num.SetActiveBetter(true);
					component2.localEulerAngles = Vector3.zero;
					View.SetItemSprite(component.m_icon, _003CFillDropCell_003Ec__AnonStorey.info.icon, true);
				}
				else
				{
					component.txt_num.SetActiveBetter(false);
					component2.localEulerAngles = _qiangEuler;
					View.SetItemSprite(component.m_icon, _003CFillDropCell_003Ec__AnonStorey.info.extrasstring[2], true);
				}
			}
			else if (_003CFillDropCell_003Ec__AnonStorey.boxInfo != null)
			{
				component.txt_num.SetActiveBetter(true);
				component2.localEulerAngles = Vector3.zero;
				View.SetItemSprite(component.m_icon, _003CFillDropCell_003Ec__AnonStorey.boxInfo.icon, true);
			}
			if (_003CFillDropCell_003Ec__AnonStorey.info != null)
			{
				View.SetLabelText(component.txt_nameText, _003CFillDropCell_003Ec__AnonStorey.info.name);
			}
			else if (_003CFillDropCell_003Ec__AnonStorey.boxInfo != null)
			{
				View.SetLabelText(component.txt_nameText, _003CFillDropCell_003Ec__AnonStorey.boxInfo.name);
			}
			if (_003CFillDropCell_003Ec__AnonStorey.info != null && (_003CFillDropCell_003Ec__AnonStorey.info.type == 33 || _003CFillDropCell_003Ec__AnonStorey.info.type == 22))
			{
				View.SetLabelText(component.txt_numText, Utils.GetString(281, 1));
				float num = (float)_003CFillDropCell_003Ec__AnonStorey.num / (float)ConstsBs.HpEquip;
				if (num > _showBluePercentage)
				{
					component.m_armor_value.SetActiveBetter(false);
				}
				else
				{
					component.m_armor_value.SetActiveBetter(true);
					Image component3 = component.m_armor_value.GetComponent<Image>();
					component3.fillAmount = num;
					component3.color = ((!(num > _showChengPercentage)) ? _equipRed : _equipCheng);
				}
			}
			else
			{
				View.SetLabelText(component.txt_numText, Utils.GetString(281, _003CFillDropCell_003Ec__AnonStorey.num));
				component.m_armor_value.SetActiveBetter(false);
			}
			ClickListener.Get(go, string.Empty).onClick = _003CFillDropCell_003Ec__AnonStorey._003C_003Em__0;
		}

		public static bool PickItem(ItemCfg itemCfgInfo, BoxCfg boxCfgInfo, int index, int id, int num, long boxInstanceId, bool isInBox)
		{
			if ((itemCfgInfo != null && (itemCfgInfo.type == 34 || itemCfgInfo.type == 35)) || boxCfgInfo != null)
			{
				Utils.TriggerEvent(BattleDropEvent.ClickBoxDelegate, index);
				return true;
			}
			int remainCapacity = Singleton<BagMgr>.Ins.GetRemainCapacity(id, num);
			if (remainCapacity > 0)
			{
				long instanceId = Singleton<BattleDropMgr>.Ins.GetInstanceId(index);
				if (!isInBox)
				{
					Singleton<BattleDropMgr>.Ins.SendPickItem(instanceId, remainCapacity);
				}
				else
				{
					Singleton<BattleDropMgr>.Ins.SendPickItemFromBox(boxInstanceId, instanceId, remainCapacity);
				}
				return true;
			}
			AlertBox.Show(26);
			return false;
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			BattleDropEvent.DropNumChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(BattleDropEvent.DropNumChangeDelegate, new Utils.VoidDelegate(DropNumChange));
			SettingEvent.ExitCustomSaveSettingPanelDelegate = (Utils.VoidDelegate)Delegate.Remove(SettingEvent.ExitCustomSaveSettingPanelDelegate, new Utils.VoidDelegate(OnExitCustomSettingPanel));
			SettingEvent.ChangeGunModeDelegate = (Utils.IntDelegate)Delegate.Remove(SettingEvent.ChangeGunModeDelegate, new Utils.IntDelegate(OnChangeGunMode));
			BattleDropEvent.HidePickPanel = (Action)Delegate.Remove(BattleDropEvent.HidePickPanel, new Action(OnHidePickPanel));
			BattleDropEvent.ShowPickPanel = (Action<byte>)Delegate.Remove(BattleDropEvent.ShowPickPanel, new Action<byte>(OnShowPickPanel));
		}

		public override void _SetRenderSort(int order)
		{
			Canvas component = GetComponent<Canvas>();
			if (component == null)
			{
				Debug.LogError(base.name + "canvas is null");
				return;
			}
			component.pixelPerfect = false;
			component.overrideSorting = true;
			component.sortingOrder = 5;
			Transform parent = base.transform.parent;
			if (!parent)
			{
				return;
			}
			Transform transform = parent.Find(typeof(BattlePanel).Name);
			if ((bool)transform)
			{
				Canvas component2 = transform.GetComponent<Canvas>();
				if ((bool)component2)
				{
					component.sortingOrder = component2.sortingOrder + 5;
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_pic = component.GameObjects[0].gameObject;
			m_panel = component.GameObjects[1].gameObject;
			m_bg = component.GameObjects[2].gameObject;
			scp_picks = component.GameObjects[3].gameObject;
			m_cell = View.AddComponentIfNotExist<GBattlePickCell>(component.GameObjects[4].gameObject);
			btn_close = component.GameObjects[5].gameObject;
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Singleton<BattleDropMgr>.Ins.IsShowScroll = false;
			UpdateBtnActives();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			Singleton<BattleDropMgr>.Ins.IsShowScroll = true;
			UpdateBtnActives();
			UpdateDropPanel(false);
		}
	}
}
