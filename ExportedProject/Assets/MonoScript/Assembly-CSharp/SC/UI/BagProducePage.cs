using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.item.create.scmsg;

namespace SC.UI
{
	public class BagProducePage : MonoBehaviour, IBagAndBuildPage
	{
		private class SelectItemClass
		{
			public const int ItemArea = 1;

			public const int CreateListArea = 2;

			public const int ShortcutArea = 3;

			private readonly int _maxCreateListCount;

			private DrawingCfg _selectItemInfo;

			public int SelectArea;

			public int MaxNumCanCreate;

			public GameObject LastSelect;

			public bool IsMaterialEnough = true;

			public DrawingCfg SelectItemInfo
			{
				get
				{
					return _selectItemInfo;
				}
				set
				{
					_selectItemInfo = value;
					if (_selectItemInfo == null)
					{
						MaxNumCanCreate = 0;
						return;
					}
					IsMaterialEnough = true;
					List<CreateInfo> creatingItemsList = Singleton<ScProduceMgr>.Ins.CreatingItemsList;
					int onceMaxCreate = _selectItemInfo.onceMaxCreate;
					int num = (_maxCreateListCount - creatingItemsList.Count) * onceMaxCreate;
					int id = _selectItemInfo.id;
					int i = 0;
					for (int count = creatingItemsList.Count; i < count; i++)
					{
						CreateInfo createInfo = creatingItemsList[i];
						if (createInfo.createId == id)
						{
							num += onceMaxCreate - createInfo.createNum;
						}
					}
					List<DrawingNeedMaterial> material = _selectItemInfo.material;
					int j = 0;
					for (int count2 = material.Count; j < count2; j++)
					{
						int num2 = Singleton<BagMgr>.Ins.GetItemNum(material[j].itemId) / material[j].num;
						if (num2 < num)
						{
							IsMaterialEnough = false;
						}
						num = ((num2 >= num) ? num : num2);
					}
					MaxNumCanCreate = num;
				}
			}

			public SelectItemClass(int maxCreateListCount)
			{
				_maxCreateListCount = maxCreateListCount;
			}
		}

		[CompilerGenerated]
		private sealed class _003COnInit_003Ec__AnonStorey0
		{
			internal KeyValuePair<int, BagProduceBtn> pair1;

			internal BagProducePage _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this.OnClickBtn(pair1.Key);
			}
		}

		[CompilerGenerated]
		private sealed class _003COnInit_003Ec__AnonStorey1
		{
			internal KeyValuePair<int, BagProduceBtn> pair1;

			internal BagProducePage _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this.OnClickBtn1(pair1.Key);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillDrawingItemCell_003Ec__AnonStorey2
		{
			internal bool isShowEffect;

			internal GBagProduceItemCell cell;

			internal DrawingCfg drawingInfo;

			internal BagProducePage _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				if (isShowEffect)
				{
					cell.m_novice_effect.SetActiveBetter(false);
					Utils.TriggerEvent(GuideEvent.StepCompleteAction);
				}
				if ((bool)_0024this._selectItem.LastSelect)
				{
					_0024this._selectItem.LastSelect.SetActiveBetter(false);
				}
				_0024this._selectItem.LastSelect = cell.m_select;
				_0024this._selectItem.SelectItemInfo = drawingInfo;
				_0024this._selectItem.SelectArea = 1;
				_0024this.SetSelectItemDesc();
				_0024this.UpdateSelectCell();
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillShortcutListCells_003Ec__AnonStorey3
		{
			internal int i1;

			internal BagProducePage _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this._selectItem.SelectItemInfo = UIContext.Get<DrawingCfg>(_0024this.m_shortcut[i1]);
				_0024this._selectItem.SelectArea = 3;
				_0024this.SetSelectItemDesc();
				if ((bool)_0024this._selectItem.LastSelect)
				{
					_0024this._selectItem.LastSelect.SetActiveBetter(false);
					_0024this._selectItem.LastSelect = null;
				}
				_0024this.PretreatmentSendCreateMsg(1);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillCreateListCells_003Ec__AnonStorey4
		{
			internal GBagProduceListCell cell;

			internal BagProducePage _0024this;
		}

		[CompilerGenerated]
		private sealed class _003CFillCreateListCells_003Ec__AnonStorey5
		{
			internal int i1;

			internal RectTransform rectTrans;

			internal _003CFillCreateListCells_003Ec__AnonStorey4 _003C_003Ef__ref_00244;

			internal void _003C_003Em__0(GameObject go)
			{
				if ((bool)_003C_003Ef__ref_00244._0024this._selectItem.LastSelect)
				{
					_003C_003Ef__ref_00244._0024this._selectItem.LastSelect.SetActiveBetter(false);
				}
				_003C_003Ef__ref_00244._0024this._selectItem.LastSelect = _003C_003Ef__ref_00244.cell.m_select;
				_003C_003Ef__ref_00244.cell.m_select.SetActiveBetter(true);
				_003C_003Ef__ref_00244._0024this._selectItem.SelectItemInfo = UIContext.Get<DrawingCfg>(_003C_003Ef__ref_00244._0024this.m_produce_list[i1]);
				_003C_003Ef__ref_00244._0024this._selectItem.SelectArea = 2;
				_003C_003Ef__ref_00244._0024this._selectCreateListIndex = i1;
				_003C_003Ef__ref_00244._0024this.SetSelectItemDesc();
			}

			internal void _003C_003Em__1(GameObject go)
			{
				_003C_003Ef__ref_00244._0024this._from = _003C_003Ef__ref_00244._0024this.m_produce_list[i1];
				_003C_003Ef__ref_00244._0024this._createRenderOderList[i1].order = 1;
				_003C_003Ef__ref_00244._0024this._createRenderOderList[i1].enabled = true;
				_003C_003Ef__ref_00244._0024this._createRenderOderList[i1].enabled = false;
			}

			internal void _003C_003Em__2(GameObject go)
			{
				rectTrans.anchoredPosition = Vector2.zero;
				int fromIndex = UIContext.Get<int>(_003C_003Ef__ref_00244._0024this._from, "index");
				if (_003C_003Ef__ref_00244._0024this._target == null)
				{
					_003C_003Ef__ref_00244._0024this.ClearClickCache(fromIndex);
					return;
				}
				if (_003C_003Ef__ref_00244._0024this._from != _003C_003Ef__ref_00244._0024this._target)
				{
					int targetIndex = UIContext.Get<int>(_003C_003Ef__ref_00244._0024this._target, "index");
					Singleton<ScProduceMgr>.Ins.SendChangeIndexMsg(fromIndex, targetIndex);
				}
				_003C_003Ef__ref_00244._0024this.ClearClickCache(fromIndex);
			}

			internal void _003C_003Em__3(GameObject go)
			{
				_003C_003Ef__ref_00244._0024this._target = _003C_003Ef__ref_00244._0024this.m_produce_list[i1];
			}

			internal void _003C_003Em__4(GameObject go)
			{
				_003C_003Ef__ref_00244._0024this._target = null;
			}

			internal void _003C_003Em__5(GameObject go)
			{
				rectTrans.position = (Vector2)ViewMgr.Ins.UICamera.ScreenToWorldPoint(Input.mousePosition);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillRequireMaterials_003Ec__AnonStorey6
		{
			internal DrawingNeedMaterial material;

			internal void _003C_003Em__0(GameObject go)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(material.itemId));
			}
		}

		private int _roleLevel;

		private UIScrollPanel _itemsScrollPanel;

		private SelectItemClass _selectItem;

		private int _selectCreateListIndex;

		private Toggle _toggleShortcut;

		private InputField _inpCreateNum;

		private Text _countdownText;

		private Text _countdownNum;

		private readonly Dictionary<int, BagProduceBtn> _dicTabType2Btn2 = new Dictionary<int, BagProduceBtn>();

		private readonly Dictionary<int, BagProduceBtn> _dicTabType2Btn1 = new Dictionary<int, BagProduceBtn>();

		private readonly List<DrawingCfg> _curTabItemsList = new List<DrawingCfg>();

		private int _curTabType;

		private readonly Dictionary<int, List<int>> _dicTab1ToTab2S = new Dictionary<int, List<int>>();

		private bool _isReRequestInfo;

		private float _intervalTime = 0.1f;

		private float _passTime;

		private GameObject _from;

		private GameObject _target;

		private List<RenderQueueFixerInUIForParticle> _createRenderOderList = new List<RenderQueueFixerInUIForParticle>();

		public GameObject btn_add;

		public BagProduceBtn btn_battle;

		public BagProduceBtn btn_bullet;

		public GameObject btn_cancel;

		public BagProduceBtn btn_cook;

		public GameObject btn_create;

		public BagProduceBtn btn_dianqi_s;

		public BagProduceBtn btn_drug;

		public BagProduceBtn btn_equip;

		public GameObject btn_get;

		public GameObject btn_get_finish;

		public BagProduceBtn btn_item;

		public GameObject btn_learn;

		public BagProduceBtn btn_material;

		public GameObject btn_max;

		public BagProduceBtn btn_moto;

		public BagProduceBtn btn_normal;

		public BagProduceBtn btn_normal_s;

		public GameObject btn_reduce;

		public BagProduceBtn btn_structure;

		public BagProduceBtn btn_structure_s;

		public BagProduceBtn btn_tool_s;

		public BagProduceBtn btn_trap;

		public BagProduceBtn btn_vehicle;

		public BagProduceBtn btn_weapon;

		public GameObject ckb_add_shortcut;

		public GameObject inp_num;

		public GBagProduceItemCell m_cell;

		public GameObject m_gray_ima;

		public GameObject m_icon_select;

		public GameObject m_jichu;

		public List<GBagProduceListCell> m_produce_listlist = new List<GBagProduceListCell>();

		public GameObject[] m_produce_list;

		public GameObject m_produce_listObj;

		public List<GBagProduceRequirementCell> m_requirementlist = new List<GBagProduceRequirementCell>();

		public GameObject[] m_requirement;

		public GameObject m_requirementObj;

		public GameObject m_right;

		public GameObject m_sheshi;

		public List<GBagProduceShortcutCell> m_shortcutlist = new List<GBagProduceShortcutCell>();

		public GameObject[] m_shortcut;

		public GameObject m_shortcutObj;

		public GameObject m_weapon_property_parent;

		public GameObject m_wupin;

		public GameObject m_zaiju;

		public GameObject m_zhandou;

		public GameObject scp_items;

		public GameObject txt_bag_full;

		public Text txt_bag_fullText;

		public GameObject txt_danLiang_weapon;

		public Text txt_danLiang_weaponText;

		public GameObject txt_desc_select;

		public Text txt_desc_selectText;

		public GameObject txt_locked;

		public Text txt_lockedText;

		public GameObject txt_name_select;

		public Text txt_name_selectText;

		public GameObject txt_shangHai_weapon;

		public Text txt_shangHai_weaponText;

		public GameObject txt_sheCheng_weapon;

		public Text txt_sheCheng_weaponText;

		public GameObject txt_sheSu_weapon;

		public Text txt_sheSu_weaponText;

		public GameObject txt_time;

		public Text txt_timeText;

		public GameObject txt_unlock_condition;

		public Text txt_unlock_conditionText;

		public GameObject txt_wenDingXing_weapon;

		public Text txt_wenDingXing_weaponText;

		public GameObject txt_zhuangDanSuDu_weapon;

		public Text txt_zhuangDanSuDu_weaponText;

		public object context;

		public bool IsShow
		{
			get
			{
				return base.gameObject.activeSelf;
			}
		}

		public void OnInit()
		{
			Singleton<ScProduceMgr>.Ins.SendRequireCreateInfoMsg();
			Singleton<ScProduceMgr>.Ins.InitItemLists();
			m_cell.gameObject.SetActiveBetter(false);
			_selectItem = new SelectItemClass(m_produce_list.Length);
			_itemsScrollPanel = scp_items.GetComponent<UIScrollPanel>();
			_toggleShortcut = ckb_add_shortcut.GetComponent<Toggle>();
			_inpCreateNum = inp_num.GetComponent<InputField>();
			_dicTabType2Btn2.Add(1, btn_normal_s);
			_dicTabType2Btn2.Add(2, btn_tool_s);
			_dicTabType2Btn2.Add(3, btn_material);
			_dicTabType2Btn2.Add(4, btn_structure_s);
			_dicTabType2Btn2.Add(5, btn_cook);
			_dicTabType2Btn2.Add(6, btn_drug);
			_dicTabType2Btn2.Add(7, btn_equip);
			_dicTabType2Btn2.Add(8, btn_weapon);
			_dicTabType2Btn2.Add(9, btn_bullet);
			_dicTabType2Btn2.Add(10, btn_trap);
			_dicTabType2Btn2.Add(15, btn_moto);
			_dicTabType2Btn2.Add(16, btn_dianqi_s);
			foreach (KeyValuePair<int, BagProduceBtn> item in _dicTabType2Btn2)
			{
				_003COnInit_003Ec__AnonStorey0 _003COnInit_003Ec__AnonStorey = new _003COnInit_003Ec__AnonStorey0();
				_003COnInit_003Ec__AnonStorey._0024this = this;
				_003COnInit_003Ec__AnonStorey.pair1 = item;
				ClickListener.Get(item.Value.gameObject, string.Empty).onClick = _003COnInit_003Ec__AnonStorey._003C_003Em__0;
			}
			List<int> list = new List<int>();
			list.Add(1);
			_dicTab1ToTab2S[1] = list;
			list = new List<int>();
			list.Add(2);
			list.Add(5);
			list.Add(6);
			list.Add(3);
			_dicTab1ToTab2S[12] = list;
			list = new List<int>();
			list.Add(4);
			_dicTab1ToTab2S[4] = list;
			list = new List<int>();
			list.Add(7);
			list.Add(8);
			list.Add(9);
			list.Add(10);
			_dicTab1ToTab2S[13] = list;
			list = new List<int>();
			list.Add(15);
			_dicTab1ToTab2S[14] = list;
			_dicTabType2Btn1.Add(1, btn_normal);
			_dicTabType2Btn1.Add(12, btn_item);
			_dicTabType2Btn1.Add(4, btn_structure);
			_dicTabType2Btn1.Add(13, btn_battle);
			_dicTabType2Btn1.Add(14, btn_vehicle);
			foreach (KeyValuePair<int, BagProduceBtn> item2 in _dicTabType2Btn1)
			{
				_003COnInit_003Ec__AnonStorey1 _003COnInit_003Ec__AnonStorey2 = new _003COnInit_003Ec__AnonStorey1();
				_003COnInit_003Ec__AnonStorey2._0024this = this;
				_003COnInit_003Ec__AnonStorey2.pair1 = item2;
				ClickListener.Get(item2.Value.gameObject, string.Empty).onClick = _003COnInit_003Ec__AnonStorey2._003C_003Em__0;
			}
			ClickListener.Get(btn_add, string.Empty).onClick = _003COnInit_003Em__0;
			ClickListener.Get(btn_reduce, string.Empty).onClick = _003COnInit_003Em__1;
			_inpCreateNum.onValueChanged.AddListener(_003COnInit_003Em__2);
			ClickListener.Get(btn_max, string.Empty).onClick = _003COnInit_003Em__3;
			ClickListener.Get(btn_cancel, string.Empty).onClick = _003COnInit_003Em__4;
			ClickListener.Get(btn_learn, string.Empty).onClick = _003COnInit_003Em__5;
			ClickListener.Get(btn_get, string.Empty).onClick = _003COnInit_003Em__6;
			ClickListener.Get(btn_create, string.Empty).onClick = _003COnInit_003Em__7;
			ClickListener.Get(ckb_add_shortcut, string.Empty).onClick = _003COnInit_003Em__8;
			ClickListener.Get(m_icon_select, string.Empty).onClick = (ClickListener.Get(m_gray_ima, string.Empty).onClick = _003COnInit_003Em__9);
			ClickListener.Get(btn_get_finish, string.Empty).onClick = _003COnInit_003Em__A;
		}

		private void PretreatmentSendCreateMsg(int toCreateNum)
		{
			if (_selectItem.SelectItemInfo.levelLimit > _roleLevel)
			{
				AlertBox.Show(22);
				return;
			}
			int num = toCreateNum;
			int onceMaxCreate = _selectItem.SelectItemInfo.onceMaxCreate;
			int i = 0;
			for (int num2 = m_produce_list.Length; i < num2; i++)
			{
				List<CreateInfo> creatingItemsList = Singleton<ScProduceMgr>.Ins.CreatingItemsList;
				if (creatingItemsList.Count > i)
				{
					if (creatingItemsList[i].createId == _selectItem.SelectItemInfo.id)
					{
						if (creatingItemsList[i].createNum >= onceMaxCreate)
						{
							continue;
						}
						int num3 = ((onceMaxCreate - creatingItemsList[i].createNum >= num) ? num : (onceMaxCreate - creatingItemsList[i].createNum));
						if (num3 > 0)
						{
							Singleton<ScProduceMgr>.Ins.SendChangeCreateNumMsg(i, num3);
							num -= num3;
						}
					}
				}
				else
				{
					int num4 = ((num >= onceMaxCreate) ? onceMaxCreate : num);
					Singleton<ScProduceMgr>.Ins.SendCreateItemMsg(_selectItem.SelectItemInfo.id, num4);
					num -= num4;
				}
				if (num <= 0)
				{
					return;
				}
			}
			if (num > 0)
			{
				AlertBox.Show(11);
			}
		}

		private void ResetBtn1RedDotAndNewTag(int tabType)
		{
			bool flag = false;
			bool flag2 = false;
			List<int> value;
			if (_dicTab1ToTab2S.TryGetValue(tabType, out value))
			{
				int i = 0;
				for (int count = value.Count; i < count; i++)
				{
					bool isCanCreate;
					bool isNewUnlock;
					ResetBtnRedDotAndNewTag(value[i], out isCanCreate, out isNewUnlock);
					if (!flag)
					{
						flag = isCanCreate;
					}
					if (!flag2)
					{
						flag2 = isNewUnlock;
					}
					if (flag && flag2)
					{
						break;
					}
				}
			}
			BagProduceBtn bagProduceBtn = _dicTabType2Btn1[tabType];
			bagProduceBtn.m_red_dot.SetActiveBetter(flag);
			bagProduceBtn.m_new.SetActiveBetter(flag2);
		}

		private void ResetBtnRedDotAndNewTag(int tabType, out bool isCanCreate, out bool isNewUnlock)
		{
			isCanCreate = false;
			isNewUnlock = false;
			BagProduceBtn value;
			if (!_dicTabType2Btn2.TryGetValue(tabType, out value))
			{
				return;
			}
			List<DrawingCfg> value2;
			if (Singleton<ScProduceMgr>.Ins.DicType2Cfgs.TryGetValue(tabType, out value2))
			{
				HashSet<int> canProduce = Singleton<ScProduceMgr>.Ins.CanProduce;
				HashSet<int> newUnlock = Singleton<ScProduceMgr>.Ins.NewUnlock;
				int i = 0;
				for (int count = value2.Count; i < count; i++)
				{
					int id = value2[i].id;
					if (canProduce.Contains(id))
					{
						isCanCreate = true;
					}
					if (newUnlock.Contains(id))
					{
						isNewUnlock = true;
					}
					if (isCanCreate && isNewUnlock)
					{
						break;
					}
				}
			}
			value.m_red_dot.SetActiveBetter(isCanCreate);
			value.m_new.SetActiveBetter(isNewUnlock);
		}

		private int GetSuitableNum(int curNum, int max, int min = 0)
		{
			if (curNum > max)
			{
				return max;
			}
			if (curNum < min)
			{
				return min;
			}
			return curNum;
		}

		private void OnClickBtn1(int tab1, bool isResetSelect = true, bool isReset = false)
		{
			switch (tab1)
			{
			case 1:
				RadioButton.AllGroupBtnOff(btn_normal_s.gameObject);
				break;
			case 12:
				RadioButton.AllGroupBtnOff(btn_tool_s.gameObject);
				break;
			case 4:
				RadioButton.AllGroupBtnOff(btn_structure_s.gameObject);
				break;
			case 13:
				RadioButton.AllGroupBtnOff(btn_equip.gameObject);
				break;
			case 14:
				RadioButton.AllGroupBtnOff(btn_moto.gameObject);
				break;
			}
			OnClickBtn(tab1, isResetSelect, isReset);
			m_jichu.SetActiveBetter(tab1 == 1);
			m_wupin.SetActiveBetter(tab1 == 12);
			m_sheshi.SetActiveBetter(tab1 == 4);
			m_zhandou.SetActiveBetter(tab1 == 13);
			m_zaiju.SetActiveBetter(tab1 == 14);
			UpdateAllBtnRedDot();
		}

		private void OnClickBtn(int tab, bool isResetSelect = true, bool isReset = false)
		{
			if (_curTabType == tab && !isReset)
			{
				return;
			}
			_curTabType = tab;
			_curTabItemsList.Clear();
			List<DrawingCfg> value;
			Singleton<ScProduceMgr>.Ins.DicType2Cfgs.TryGetValue(tab, out value);
			SetDrawingList(value);
			_curTabItemsList.Sort(SortDrawingList);
			if (isResetSelect)
			{
				_selectItem.SelectItemInfo = null;
			}
			UpdateItemScroll(false);
			SetSelectItemDesc();
			bool isCanCreate;
			bool isNewUnlock;
			ResetBtnRedDotAndNewTag(tab, out isCanCreate, out isNewUnlock);
			HashSet<int> newUnlock = Singleton<ScProduceMgr>.Ins.NewUnlock;
			if (newUnlock.Count > 0)
			{
				for (int i = 0; i < _curTabItemsList.Count; i++)
				{
					newUnlock.Remove(_curTabItemsList[i].id);
				}
				Singleton<ScProduceMgr>.Ins.SaveForAllSetChange(newUnlock, false);
			}
			UpdateAllBtnRedDot();
		}

		private void SetDrawingList(List<DrawingCfg> tabAllList)
		{
			if (tabAllList == null)
			{
				return;
			}
			int i = 0;
			for (int count = tabAllList.Count; i < count; i++)
			{
				DrawingCfg drawingCfg = tabAllList[i];
				if (drawingCfg.isShowLocked || !Singleton<ScProduceMgr>.Ins.IsLocked(drawingCfg, _roleLevel))
				{
					_curTabItemsList.Add(drawingCfg);
				}
			}
		}

		private int SortDrawingList(DrawingCfg info1, DrawingCfg info2)
		{
			bool flag = Singleton<ScProduceMgr>.Ins.IsLocked(info1, _roleLevel);
			bool flag2 = Singleton<ScProduceMgr>.Ins.IsLocked(info2, _roleLevel);
			if (flag == flag2)
			{
				return info1.order - info2.order;
			}
			return flag ? 1 : (-1);
		}

		private void UpdateItemScrollNoPos()
		{
			UpdateItemScroll(true);
			bool isCanCreate;
			bool isNewUnlock;
			ResetBtnRedDotAndNewTag(_curTabType, out isCanCreate, out isNewUnlock);
		}

		private void UpdateItemScroll(bool isNoPos)
		{
			if (isNoPos)
			{
				_itemsScrollPanel.ResetNoPosClear(_curTabItemsList.Count, FillDrawingItemCell);
			}
			else
			{
				_itemsScrollPanel.Reset(_curTabItemsList.Count, FillDrawingItemCell);
			}
		}

		private void FillDrawingItemCell(GameObject go, int index)
		{
			_003CFillDrawingItemCell_003Ec__AnonStorey2 _003CFillDrawingItemCell_003Ec__AnonStorey = new _003CFillDrawingItemCell_003Ec__AnonStorey2();
			_003CFillDrawingItemCell_003Ec__AnonStorey._0024this = this;
			_003CFillDrawingItemCell_003Ec__AnonStorey.cell = go.GetComponent<GBagProduceItemCell>();
			_003CFillDrawingItemCell_003Ec__AnonStorey.drawingInfo = _curTabItemsList[index];
			bool flag = Singleton<ScProduceMgr>.Ins.IsLocked(_003CFillDrawingItemCell_003Ec__AnonStorey.drawingInfo, _roleLevel);
			bool flag2 = Singleton<ScProduceMgr>.Ins.MeetWorkbenchCondition(_003CFillDrawingItemCell_003Ec__AnonStorey.drawingInfo);
			_003CFillDrawingItemCell_003Ec__AnonStorey.cell.m_red_dot.SetActiveBetter(!flag && Singleton<ScProduceMgr>.Ins.CanProduce.Contains(_003CFillDrawingItemCell_003Ec__AnonStorey.drawingInfo.id) && flag2);
			_003CFillDrawingItemCell_003Ec__AnonStorey.cell.m_tag.SetActiveBetter(Singleton<ScProduceMgr>.Ins.NewUnlock.Contains(_003CFillDrawingItemCell_003Ec__AnonStorey.drawingInfo.id));
			_003CFillDrawingItemCell_003Ec__AnonStorey.cell.m_locked.SetActiveBetter(flag);
			_003CFillDrawingItemCell_003Ec__AnonStorey.cell.m_gray_ima.SetActiveBetter(!flag && !_003CFillDrawingItemCell_003Ec__AnonStorey.cell.m_red_dot.activeSelf);
			_003CFillDrawingItemCell_003Ec__AnonStorey.isShowEffect = Singleton<GuideMgr>.Ins.IsProduceEffectShow(_003CFillDrawingItemCell_003Ec__AnonStorey.drawingInfo.id);
			_003CFillDrawingItemCell_003Ec__AnonStorey.cell.m_novice_effect.SetActiveBetter(_003CFillDrawingItemCell_003Ec__AnonStorey.isShowEffect);
			View.SetItemSprite(_003CFillDrawingItemCell_003Ec__AnonStorey.cell.m_icon, ItemCfg.Get(_003CFillDrawingItemCell_003Ec__AnonStorey.drawingInfo.targetItemId).icon);
			_003CFillDrawingItemCell_003Ec__AnonStorey.cell.m_select.SetActiveBetter(_selectItem.SelectItemInfo != null && _selectItem.SelectItemInfo.id == _003CFillDrawingItemCell_003Ec__AnonStorey.drawingInfo.id && _selectItem.SelectArea == 1);
			if (_selectItem.SelectItemInfo != null && _selectItem.SelectItemInfo.id == _003CFillDrawingItemCell_003Ec__AnonStorey.drawingInfo.id && !_selectItem.LastSelect)
			{
				_selectItem.LastSelect = _003CFillDrawingItemCell_003Ec__AnonStorey.cell.m_select;
				_003CFillDrawingItemCell_003Ec__AnonStorey.cell.m_select.SetActiveBetter(true);
			}
			bool flag3 = false;
			List<CreateInfo> creatingItemsList = Singleton<ScProduceMgr>.Ins.CreatingItemsList;
			for (int i = 0; i < creatingItemsList.Count; i++)
			{
				if (creatingItemsList[i].createId == _003CFillDrawingItemCell_003Ec__AnonStorey.drawingInfo.id)
				{
					flag3 = true;
					break;
				}
			}
			_003CFillDrawingItemCell_003Ec__AnonStorey.cell.m_queuing.SetActiveBetter(flag3);
			if (flag3)
			{
				View.SetLabelText(_003CFillDrawingItemCell_003Ec__AnonStorey.cell.txt_queuingText, Utils.GetString((creatingItemsList[0].createId != _003CFillDrawingItemCell_003Ec__AnonStorey.drawingInfo.id) ? 103 : 102));
			}
			ClickListener.Get(go, string.Empty).onClick = _003CFillDrawingItemCell_003Ec__AnonStorey._003C_003Em__0;
		}

		public void OnShow(object param = null)
		{
			base.gameObject.SetActiveBetter(true);
			_roleLevel = Singleton<RoleMgr>.Ins.info.level;
			Singleton<ScProduceMgr>.Ins.RefreshCanProduceSet();
			if (param != null)
			{
				int key = (int)param;
				DrawingCfg drawingCfg = DrawingCfg.Get(key);
				if (drawingCfg != null)
				{
					ParamNotNull(drawingCfg);
				}
				else
				{
					RadioButton.ChooseBtn(btn_normal.gameObject);
					OnClickBtn1(1, true, true);
				}
			}
			else
			{
				RadioButton.ChooseBtn(btn_normal.gameObject);
				OnClickBtn1(1, true, true);
			}
			FillShortcutListCells();
			FillCreateListCells();
			SetSelectItemDesc();
			UpdateAllBtnRedDot();
			if (Singleton<ScProduceMgr>.Ins.IsLockCreate && Singleton<ScProduceMgr>.Ins.CreatingItemsList.Count > 0)
			{
				View.SetLabelText(_countdownText, Utils.GetCountDownTime(Singleton<ScProduceMgr>.Ins.CountDown));
				List<CreateInfo> creatingItemsList = Singleton<ScProduceMgr>.Ins.CreatingItemsList;
				View.SetLabelText(_countdownNum, creatingItemsList[0].createNum);
				txt_bag_full.SetActiveBetter(true);
				Color color = txt_bag_fullText.color;
				color.a = ((!Singleton<BagMgr>.Ins.IsHasCapacity(DrawingCfg.Get(creatingItemsList[0].createId).targetItemId, 1)) ? 1 : 0);
				txt_bag_fullText.color = color;
			}
			else
			{
				Singleton<ScProduceMgr>.Ins.IsLockCreate = false;
				txt_bag_full.SetActiveBetter(false);
			}
			RoleEvent.LevelChangeDelegate = (Utils.Int2Delegate)Delegate.Combine(RoleEvent.LevelChangeDelegate, new Utils.Int2Delegate(OnLevelChange));
			ScProduceEvent.ItemListChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(ScProduceEvent.ItemListChangeDelegate, new Utils.VoidDelegate(UpdateItemScrollNoPos));
			ScProduceEvent.CreateListChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(ScProduceEvent.CreateListChangeDelegate, new Utils.VoidDelegate(FillCreateListCells));
			ScProduceEvent.ChangeShortcutKeyDelegate = (Utils.VoidDelegate)Delegate.Combine(ScProduceEvent.ChangeShortcutKeyDelegate, new Utils.VoidDelegate(FillShortcutListCells));
			ScProduceEvent.ItemCanProduceChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(ScProduceEvent.ItemCanProduceChangeDelegate, new Utils.VoidDelegate(UpdateCanProduce));
			ScProduceEvent.ItemNewUnlockChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(ScProduceEvent.ItemNewUnlockChangeDelegate, new Utils.VoidDelegate(UpdateNewUnlock));
			ScProduceEvent.UpdateAllBtnRedDotDelegate = (Utils.VoidDelegate)Delegate.Combine(ScProduceEvent.UpdateAllBtnRedDotDelegate, new Utils.VoidDelegate(UpdateAllBtnRedDot));
			SCancelCreate.handler = (SCancelCreate.Handler)Delegate.Combine(SCancelCreate.handler, new SCancelCreate.Handler(OnSCancelCreate));
			SGetFinshCreate.handler = (SGetFinshCreate.Handler)Delegate.Combine(SGetFinshCreate.handler, new SGetFinshCreate.Handler(OnSGetFinshCreate));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Combine(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			ScProduceEvent.ItemInfoWayJumpDelegate = (Utils.IntDelegate)Delegate.Combine(ScProduceEvent.ItemInfoWayJumpDelegate, new Utils.IntDelegate(JumpToSelectByProduceId));
			SCreateInfo.handler = (SCreateInfo.Handler)Delegate.Combine(SCreateInfo.handler, new SCreateInfo.Handler(OnSCreateInfo));
		}

		private void OnSCreateInfo(SCreateInfo msg)
		{
			_isReRequestInfo = false;
			OnClickBtn(_curTabType, false, true);
		}

		private void JumpToSelectByProduceId(int itemId)
		{
			int value;
			Singleton<ScProduceMgr>.Ins.DicProductionId2DrawId.TryGetValue(itemId, out value);
			DrawingCfg drawingCfg = DrawingCfg.Get(value);
			if (drawingCfg != null)
			{
				ParamNotNull(drawingCfg);
				SetSelectItemDesc();
			}
			else
			{
				RadioButton.ChooseBtn(btn_normal.gameObject);
				OnClickBtn1(1, true, true);
				SetSelectItemDesc();
			}
		}

		private void ParamNotNull(DrawingCfg drawingInfo)
		{
			if ((bool)_selectItem.LastSelect)
			{
				_selectItem.LastSelect.SetActiveBetter(false);
				_selectItem.LastSelect = null;
			}
			_selectItem.SelectItemInfo = drawingInfo;
			_selectItem.SelectArea = 1;
			RadioButton.ChooseBtn(GetBtn1ByDrawingType(drawingInfo.bigType));
			OnClickBtn1(drawingInfo.bigType, false, true);
			RadioButton.ChooseBtn(GetBtn2ByDrawingType(drawingInfo.smallType));
			OnClickBtn(drawingInfo.smallType, false, true);
		}

		private GameObject GetBtn1ByDrawingType(int type)
		{
			return _dicTabType2Btn1[type].gameObject;
		}

		private GameObject GetBtn2ByDrawingType(int type)
		{
			return _dicTabType2Btn2[type].gameObject;
		}

		private void OnSCancelCreate(SCancelCreate msg)
		{
			btn_cancel.SetActiveBetter(false);
			btn_create.SetActiveBetter(true);
		}

		private void OnLevelChange(int oldLevel, int level)
		{
			_roleLevel = level;
			UpdateItemScrollNoPos();
		}

		public void OnHide()
		{
			base.gameObject.SetActiveBetter(false);
			RoleEvent.LevelChangeDelegate = (Utils.Int2Delegate)Delegate.Remove(RoleEvent.LevelChangeDelegate, new Utils.Int2Delegate(OnLevelChange));
			ScProduceEvent.ItemListChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(ScProduceEvent.ItemListChangeDelegate, new Utils.VoidDelegate(UpdateItemScrollNoPos));
			ScProduceEvent.CreateListChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(ScProduceEvent.CreateListChangeDelegate, new Utils.VoidDelegate(FillCreateListCells));
			ScProduceEvent.ChangeShortcutKeyDelegate = (Utils.VoidDelegate)Delegate.Remove(ScProduceEvent.ChangeShortcutKeyDelegate, new Utils.VoidDelegate(FillShortcutListCells));
			ScProduceEvent.ItemCanProduceChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(ScProduceEvent.ItemCanProduceChangeDelegate, new Utils.VoidDelegate(UpdateCanProduce));
			ScProduceEvent.ItemNewUnlockChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(ScProduceEvent.ItemNewUnlockChangeDelegate, new Utils.VoidDelegate(UpdateNewUnlock));
			ScProduceEvent.UpdateAllBtnRedDotDelegate = (Utils.VoidDelegate)Delegate.Remove(ScProduceEvent.UpdateAllBtnRedDotDelegate, new Utils.VoidDelegate(UpdateAllBtnRedDot));
			SCancelCreate.handler = (SCancelCreate.Handler)Delegate.Remove(SCancelCreate.handler, new SCancelCreate.Handler(OnSCancelCreate));
			SGetFinshCreate.handler = (SGetFinshCreate.Handler)Delegate.Remove(SGetFinshCreate.handler, new SGetFinshCreate.Handler(OnSGetFinshCreate));
			BagEvent.ItemChange = (Utils.Int2Delegate)Delegate.Remove(BagEvent.ItemChange, new Utils.Int2Delegate(OnItemChange));
			ScProduceEvent.ItemInfoWayJumpDelegate = (Utils.IntDelegate)Delegate.Remove(ScProduceEvent.ItemInfoWayJumpDelegate, new Utils.IntDelegate(JumpToSelectByProduceId));
			SCreateInfo.handler = (SCreateInfo.Handler)Delegate.Remove(SCreateInfo.handler, new SCreateInfo.Handler(OnSCreateInfo));
		}

		private void OnItemChange(int itemId, int num)
		{
			Singleton<ScProduceMgr>.Ins.RefreshCanProduceSet();
			SetSelectItemDesc();
		}

		private void OnSGetFinshCreate(SGetFinshCreate msg)
		{
			txt_bag_full.SetActiveBetter(false);
		}

		private void UpdateAllBtnRedDot()
		{
			foreach (int key in _dicTab1ToTab2S.Keys)
			{
				ResetBtn1RedDotAndNewTag(key);
			}
			foreach (int key2 in _dicTabType2Btn2.Keys)
			{
				bool isCanCreate;
				bool isNewUnlock;
				ResetBtnRedDotAndNewTag(key2, out isCanCreate, out isNewUnlock);
			}
		}

		private void Update()
		{
			_passTime += Time.deltaTime;
			if (!(_passTime > _intervalTime) || Singleton<ScProduceMgr>.Ins.IsLockCreate)
			{
				return;
			}
			_passTime = 0f;
			List<CreateInfo> creatingItemsList = Singleton<ScProduceMgr>.Ins.CreatingItemsList;
			if (!_countdownText || creatingItemsList.Count <= 0)
			{
				return;
			}
			int num = (int)(Singleton<ScProduceMgr>.Ins.CurItemFinishTime - Time.realtimeSinceStartup);
			num++;
			if (num < 0 && !_isReRequestInfo)
			{
				_isReRequestInfo = true;
				Singleton<ScProduceMgr>.Ins.SendRequireCreateInfoMsg(true);
				return;
			}
			View.SetLabelText(_countdownText, Utils.GetCountDownTime(num));
			DrawingCfg drawingCfg = DrawingCfg.Get(creatingItemsList[0].createId);
			int num2 = 0;
			if (drawingCfg.needTime != 0)
			{
				num2 = num / drawingCfg.needTime + 1;
			}
			if (num == 0)
			{
				num2 = 0;
			}
			if (creatingItemsList[0].createNum - num2 == 1 && !Singleton<BagMgr>.Ins.IsHasCapacity(drawingCfg.targetItemId, 1))
			{
				Singleton<ScProduceMgr>.Ins.IsLockCreate = true;
				Singleton<ScProduceMgr>.Ins.CountDown = num;
				Color color = txt_bag_fullText.color;
				color.a = 1f;
				txt_bag_fullText.color = color;
				txt_bag_full.SetActiveBetter(true);
			}
			else
			{
				if (num2 < 1)
				{
					num2 = 1;
				}
				creatingItemsList[0].createNum = ((num2 >= creatingItemsList[0].createNum) ? creatingItemsList[0].createNum : num2);
				View.SetLabelText(_countdownNum, creatingItemsList[0].createNum);
			}
		}

		private void UpdateCanProduce()
		{
			_itemsScrollPanel.UpdateAllCell(RefreshCanProduce);
			bool isCanCreate;
			bool isNewUnlock;
			ResetBtnRedDotAndNewTag(_curTabType, out isCanCreate, out isNewUnlock);
		}

		private void RefreshCanProduce(GameObject go, int index)
		{
			GBagProduceItemCell component = go.GetComponent<GBagProduceItemCell>();
			DrawingCfg drawingCfg = _curTabItemsList[index];
			bool flag = Singleton<ScProduceMgr>.Ins.IsLocked(drawingCfg, _roleLevel);
			bool flag2 = Singleton<ScProduceMgr>.Ins.MeetWorkbenchCondition(drawingCfg);
			component.m_red_dot.SetActiveBetter(!flag && flag2 && Singleton<ScProduceMgr>.Ins.CanProduce.Contains(drawingCfg.id));
			component.m_locked.SetActiveBetter(flag);
			component.m_gray_ima.SetActiveBetter(!flag && !component.m_red_dot.activeSelf);
		}

		private void UpdateNewUnlock()
		{
			_itemsScrollPanel.UpdateAllCell(RefreshNewUnlock);
			UpdateAllBtnRedDot();
		}

		private void RefreshNewUnlock(GameObject go, int index)
		{
			GBagProduceItemCell component = go.GetComponent<GBagProduceItemCell>();
			DrawingCfg drawingCfg = _curTabItemsList[index];
			bool flag = Singleton<ScProduceMgr>.Ins.IsLocked(drawingCfg, _roleLevel);
			bool flag2 = Singleton<ScProduceMgr>.Ins.MeetWorkbenchCondition(drawingCfg);
			component.m_red_dot.SetActiveBetter(!flag && flag2 && Singleton<ScProduceMgr>.Ins.CanProduce.Contains(drawingCfg.id));
			component.m_locked.SetActiveBetter(flag);
			component.m_gray_ima.SetActiveBetter(!flag && !component.m_red_dot.activeSelf);
		}

		private void UpdateSelectCell()
		{
			_itemsScrollPanel.UpdateAllCell(RefreshSelect);
		}

		private void RefreshSelect(GameObject go, int index)
		{
			GBagProduceItemCell component = go.GetComponent<GBagProduceItemCell>();
			DrawingCfg drawingCfg = _curTabItemsList[index];
			component.m_select.SetActiveBetter(_selectItem.SelectItemInfo != null && _selectItem.SelectItemInfo.id == drawingCfg.id);
		}

		private void FillShortcutListCells()
		{
			List<int> shortcutItemList = Singleton<ScProduceMgr>.Ins.ShortcutItemList;
			int i = 0;
			for (int num = m_shortcut.Length; i < num; i++)
			{
				GBagProduceShortcutCell gBagProduceShortcutCell = m_shortcutlist[i];
				if (shortcutItemList.Count > i)
				{
					_003CFillShortcutListCells_003Ec__AnonStorey3 _003CFillShortcutListCells_003Ec__AnonStorey = new _003CFillShortcutListCells_003Ec__AnonStorey3();
					_003CFillShortcutListCells_003Ec__AnonStorey._0024this = this;
					gBagProduceShortcutCell.m_shortcut_bar_icon.SetActiveBetter(true);
					DrawingCfg drawingCfg = DrawingCfg.Get(shortcutItemList[i]);
					ItemCfg itemCfg = ItemCfg.Get(drawingCfg.targetItemId);
					View.SetItemSprite(gBagProduceShortcutCell.m_shortcut_bar_icon, itemCfg.icon);
					UIContext.Attach(m_shortcut[i], drawingCfg);
					_003CFillShortcutListCells_003Ec__AnonStorey.i1 = i;
					ClickListener.Get(m_shortcut[i], string.Empty).onClick = _003CFillShortcutListCells_003Ec__AnonStorey._003C_003Em__0;
				}
				else
				{
					gBagProduceShortcutCell.m_shortcut_bar_icon.SetActiveBetter(false);
					ClickListener.Get(m_shortcut[i], string.Empty).onClick = null;
				}
			}
		}

		private void FillCreateListCells()
		{
			List<CreateInfo> creatingItemsList = Singleton<ScProduceMgr>.Ins.CreatingItemsList;
			int count = creatingItemsList.Count;
			int i = 0;
			for (int num = m_produce_list.Length; i < num; i++)
			{
				_003CFillCreateListCells_003Ec__AnonStorey4 _003CFillCreateListCells_003Ec__AnonStorey = new _003CFillCreateListCells_003Ec__AnonStorey4();
				_003CFillCreateListCells_003Ec__AnonStorey._0024this = this;
				_003CFillCreateListCells_003Ec__AnonStorey.cell = m_produce_listlist[i];
				if (_createRenderOderList.Count < num)
				{
					_createRenderOderList.Add(m_produce_listlist[i].m_icon.GetComponent<RenderQueueFixerInUIForParticle>());
				}
				if (count > i)
				{
					_003CFillCreateListCells_003Ec__AnonStorey5 _003CFillCreateListCells_003Ec__AnonStorey2 = new _003CFillCreateListCells_003Ec__AnonStorey5();
					_003CFillCreateListCells_003Ec__AnonStorey2._003C_003Ef__ref_00244 = _003CFillCreateListCells_003Ec__AnonStorey;
					_003CFillCreateListCells_003Ec__AnonStorey.cell.m_icon.SetActiveBetter(true);
					_003CFillCreateListCells_003Ec__AnonStorey.cell.txt_num.SetActiveBetter(true);
					_003CFillCreateListCells_003Ec__AnonStorey.cell.txt_queuing.SetActiveBetter(true);
					CreateInfo createInfo = creatingItemsList[i];
					DrawingCfg drawingCfg = DrawingCfg.Get(createInfo.createId);
					_003CFillCreateListCells_003Ec__AnonStorey.cell.m_select.SetActiveBetter(_selectCreateListIndex == i && _selectItem.SelectItemInfo != null && _selectItem.SelectItemInfo.id == createInfo.createId && _selectItem.SelectArea == 2);
					View.SetItemSprite(_003CFillCreateListCells_003Ec__AnonStorey.cell.m_icon, ItemCfg.Get(drawingCfg.targetItemId).icon);
					View.SetLabelText(_003CFillCreateListCells_003Ec__AnonStorey.cell.txt_numText, createInfo.createNum);
					if (i == 0)
					{
						_countdownText = _003CFillCreateListCells_003Ec__AnonStorey.cell.txt_queuingText;
						_countdownNum = _003CFillCreateListCells_003Ec__AnonStorey.cell.txt_numText;
					}
					UIContext.Attach(m_produce_list[i], drawingCfg);
					UIContext.Attach(m_produce_list[i], "index", i);
					_003CFillCreateListCells_003Ec__AnonStorey2.i1 = i;
					ClickListener.Get(m_produce_list[i], string.Empty).onClick = _003CFillCreateListCells_003Ec__AnonStorey2._003C_003Em__0;
					_003CFillCreateListCells_003Ec__AnonStorey2.rectTrans = _003CFillCreateListCells_003Ec__AnonStorey.cell.m_icon.GetComponent<RectTransform>();
					DownUpListener.Get(m_produce_list[i]).onDown = _003CFillCreateListCells_003Ec__AnonStorey2._003C_003Em__1;
					DownUpListener.Get(m_produce_list[i]).onUp = _003CFillCreateListCells_003Ec__AnonStorey2._003C_003Em__2;
					DragListener.Get(m_produce_list[i]).onEnter = _003CFillCreateListCells_003Ec__AnonStorey2._003C_003Em__3;
					UIEventListener.Get(m_produce_list[i], string.Empty).onExit = _003CFillCreateListCells_003Ec__AnonStorey2._003C_003Em__4;
					DragListener.Get(m_produce_list[i]).onDrag = _003CFillCreateListCells_003Ec__AnonStorey2._003C_003Em__5;
				}
				else
				{
					_003CFillCreateListCells_003Ec__AnonStorey.cell.m_icon.SetActiveBetter(false);
					_003CFillCreateListCells_003Ec__AnonStorey.cell.txt_num.SetActiveBetter(false);
					_003CFillCreateListCells_003Ec__AnonStorey.cell.txt_queuing.SetActiveBetter(false);
					_003CFillCreateListCells_003Ec__AnonStorey.cell.m_select.SetActiveBetter(false);
					ClickListener.Get(m_produce_list[i], string.Empty).onClick = null;
					DownUpListener.Get(m_produce_list[i]).onDown = null;
					DownUpListener.Get(m_produce_list[i]).onUp = null;
					DragListener.Get(m_produce_list[i]).onEnter = null;
					DragListener.Get(m_produce_list[i]).onDrag = null;
					UIContext.Dettach(m_produce_list[i]);
				}
			}
		}

		private void ClearClickCache(int fromIndex)
		{
			_from = null;
			_target = null;
			_createRenderOderList[fromIndex].order = 0;
			_createRenderOderList[fromIndex].enabled = true;
			_createRenderOderList[fromIndex].enabled = false;
		}

		private void SetSelectItemDesc()
		{
			if (_selectItem == null)
			{
				return;
			}
			if (_selectItem.SelectItemInfo == null)
			{
				m_right.SetActiveBetter(false);
				return;
			}
			m_right.SetActiveBetter(true);
			ItemCfg itemCfg = ItemCfg.Get(_selectItem.SelectItemInfo.targetItemId);
			if (itemCfg != null)
			{
				View.SetItemSprite(m_icon_select, itemCfg.icon);
				View.SetLabelText(txt_name_selectText, itemCfg.name);
				bool flag = Singleton<ScProduceMgr>.Ins.IsLocked(_selectItem.SelectItemInfo, _roleLevel);
				bool flag2 = Singleton<ScProduceMgr>.Ins.MeetWorkbenchCondition(_selectItem.SelectItemInfo);
				m_gray_ima.SetActiveBetter(flag || !flag2);
				txt_locked.SetActiveBetter(flag);
				View.SetLabelText(txt_desc_selectText, itemCfg.desc);
				FillRequireMaterials(_selectItem.SelectItemInfo);
				SetWeaponProperty(GunCfg.Get(itemCfg.id));
				txt_unlock_condition.SetActiveBetter(flag || !flag2);
				if (flag || !flag2)
				{
					View.SetLabelText(txt_unlock_conditionText, GetLimitCondition(_selectItem.SelectItemInfo));
				}
				_inpCreateNum.text = ((_selectItem.MaxNumCanCreate != 0) ? "1" : "0");
				View.SetLabelText(txt_timeText, Utils.GetCountDownTime(_selectItem.SelectItemInfo.needTime));
				bool isOn = Singleton<ScProduceMgr>.Ins.IsShortcut(_selectItem.SelectItemInfo.id);
				_toggleShortcut.isOn = isOn;
				bool flag3 = Singleton<ScProduceMgr>.Ins.IsCreating(_selectItem.SelectItemInfo.id) && _selectItem.SelectArea == 2;
				bool flag4 = Singleton<ScProduceMgr>.Ins.IsLearned(_selectItem.SelectItemInfo.id) || _selectItem.SelectItemInfo.isDefault;
				bool flag5 = Singleton<BagMgr>.Ins.GetItemNum(_selectItem.SelectItemInfo.id) > 0;
				btn_get.SetActiveBetter(!flag5 && !flag4);
				btn_learn.SetActiveBetter(flag5 && !flag4);
				btn_cancel.SetActiveBetter(flag3 && flag4);
				btn_create.SetActiveBetter(!flag3 && flag4);
			}
		}

		private void SetWeaponProperty(GunCfg gunInfo)
		{
			if (gunInfo != null)
			{
				m_weapon_property_parent.SetActiveBetter(true);
				View.SetLabelText(txt_shangHai_weaponText, gunInfo.factors[47]);
				View.SetLabelText(txt_sheSu_weaponText, gunInfo.factors[50]);
				View.SetLabelText(txt_wenDingXing_weaponText, gunInfo.factors[49]);
				View.SetLabelText(txt_sheCheng_weaponText, gunInfo.factors[48]);
				View.SetLabelText(txt_danLiang_weaponText, gunInfo.factors[34]);
				View.SetLabelText(txt_zhuangDanSuDu_weaponText, gunInfo.factors[51]);
			}
			else
			{
				m_weapon_property_parent.SetActiveBetter(false);
			}
		}

		private string GetLimitCondition(DrawingCfg drawingInfo)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (drawingInfo.levelLimit > _roleLevel)
			{
				stringBuilder.Append(Utils.GetString(15, _selectItem.SelectItemInfo.levelLimit));
				stringBuilder.Append(';');
			}
			if (drawingInfo.needWorkbench != 0)
			{
				stringBuilder.Append(GetWorkbenchLimit(drawingInfo));
				stringBuilder.Append(';');
			}
			if (!drawingInfo.isDefault && !Singleton<ScProduceMgr>.Ins.IsLearned(drawingInfo.id))
			{
				stringBuilder.Append(Utils.GetString(34, ItemCfg.Get(drawingInfo.id).name));
				stringBuilder.Append(';');
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return Utils.GetString(35, stringBuilder);
		}

		private string GetWorkbenchLimit(DrawingCfg drawingInfo)
		{
			if (drawingInfo.needWorkbenchLevel > 0)
			{
				return Utils.GetString(16, Utils.GetString(17, drawingInfo.needWorkbenchLevel) + GetWorkBenchName(drawingInfo.needWorkbench));
			}
			return Utils.GetString(16, GetWorkBenchName(drawingInfo.needWorkbench));
		}

		private string GetWorkBenchName(int workbenchType)
		{
			switch (workbenchType)
			{
			case 1:
				return Utils.GetString(32);
			case 4:
				return Utils.GetString(33);
			default:
				return string.Empty;
			}
		}

		private void FillRequireMaterials(DrawingCfg drawingInfo)
		{
			int count = drawingInfo.material.Count;
			int num = ((m_requirementlist.Count <= drawingInfo.material.Count) ? count : m_requirementlist.Count);
			for (int i = 0; i < num; i++)
			{
				if (count > i)
				{
					_003CFillRequireMaterials_003Ec__AnonStorey6 _003CFillRequireMaterials_003Ec__AnonStorey = new _003CFillRequireMaterials_003Ec__AnonStorey6();
					_003CFillRequireMaterials_003Ec__AnonStorey.material = drawingInfo.material[i];
					int itemId = _003CFillRequireMaterials_003Ec__AnonStorey.material.itemId;
					int num2 = _003CFillRequireMaterials_003Ec__AnonStorey.material.num;
					int itemNum = Singleton<BagMgr>.Ins.GetItemNum(itemId);
					bool flag = num2 > itemNum;
					m_requirement[i].SetActiveBetter(true);
					View.SetItemSprite(m_requirementlist[i].m_icon_desc, ItemCfg.Get(itemId).icon);
					View.SetLabelText(m_requirementlist[i].txt_num_descText, Utils.GetString(9, itemNum, num2));
					m_requirementlist[i].btn_add.SetActiveBetter(flag);
					Image component = m_requirementlist[i].m_icon_desc.GetComponent<Image>();
					if (!flag)
					{
						component.color = Color.white;
						m_requirementlist[i].txt_num_descText.color = Color.white;
					}
					else
					{
						component.color = Color.gray;
						Color color;
						ColorUtility.TryParseHtmlString("#E93F3F", out color);
						m_requirementlist[i].txt_num_descText.color = color;
					}
					ClickListener.Get(m_requirementlist[i].btn_add, string.Empty).onClick = (ClickListener.Get(m_requirement[i], string.Empty).onClick = _003CFillRequireMaterials_003Ec__AnonStorey._003C_003Em__0);
				}
				else
				{
					m_requirement[i].SetActiveBetter(false);
				}
			}
		}

		private void ResetRequireMaterialsNum(DrawingCfg drawingInfo, int createNum)
		{
			int i = 0;
			for (int count = drawingInfo.material.Count; i < count; i++)
			{
				DrawingNeedMaterial drawingNeedMaterial = drawingInfo.material[i];
				int num = drawingNeedMaterial.num * createNum;
				int itemNum = Singleton<BagMgr>.Ins.GetItemNum(drawingNeedMaterial.itemId);
				View.SetLabelText(m_requirementlist[i].txt_num_descText, Utils.GetString(9, itemNum, num));
				m_requirementlist[i].btn_add.SetActiveBetter(num > itemNum);
			}
		}

		private void Awake()
		{
			btn_add = base.transform.Find("m_right/GameObject (2)/GameObject (1)/Image/btn_add").gameObject;
			btn_battle = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image (1)/btn_battle").gameObject);
			btn_bullet = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_zhandou/btn_bullet").gameObject);
			btn_cancel = base.transform.Find("m_right/GameObject (2)/GameObject/btn_cancel").gameObject;
			btn_cook = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_wupin/btn_cook").gameObject);
			btn_create = base.transform.Find("m_right/GameObject (2)/GameObject/btn_create").gameObject;
			btn_dianqi_s = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_sheshi/btn_dianqi_s").gameObject);
			btn_drug = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_wupin/btn_drug").gameObject);
			btn_equip = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_zhandou/btn_equip").gameObject);
			btn_get = base.transform.Find("m_right/GameObject (2)/GameObject/btn_get").gameObject;
			btn_get_finish = base.transform.Find("zuo (1)/Image/GameObject/txt_bag_full/btn_get_finish").gameObject;
			btn_item = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image (1)/btn_item").gameObject);
			btn_learn = base.transform.Find("m_right/GameObject (2)/GameObject/btn_learn").gameObject;
			btn_material = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_wupin/btn_material").gameObject);
			btn_max = base.transform.Find("m_right/GameObject (2)/GameObject (1)/Image/btn_max").gameObject;
			btn_moto = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_zaiju/btn_moto").gameObject);
			btn_normal = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image (1)/btn_normal").gameObject);
			btn_normal_s = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_jichu/btn_normal_s").gameObject);
			btn_reduce = base.transform.Find("m_right/GameObject (2)/GameObject (1)/Image/btn_reduce").gameObject;
			btn_structure = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image (1)/btn_structure").gameObject);
			btn_structure_s = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_sheshi/btn_structure_s").gameObject);
			btn_tool_s = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_wupin/btn_tool_s").gameObject);
			btn_trap = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_zhandou/btn_trap").gameObject);
			btn_vehicle = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image (1)/btn_vehicle").gameObject);
			btn_weapon = View.AddComponentIfNotExist<BagProduceBtn>(base.transform.Find("zuo (1)/Image/m_zhandou/btn_weapon").gameObject);
			ckb_add_shortcut = base.transform.Find("m_right/GameObject (2)/ckb_add_shortcut").gameObject;
			inp_num = base.transform.Find("m_right/GameObject (2)/GameObject (1)/Image/inp_num").gameObject;
			m_cell = View.AddComponentIfNotExist<GBagProduceItemCell>(base.transform.Find("zuo (1)/Image/scp_items/content/m_cell").gameObject);
			m_gray_ima = base.transform.Find("m_right/m_gray_ima").gameObject;
			m_icon_select = base.transform.Find("m_right/RawImage/bg/m_icon_select").gameObject;
			m_jichu = base.transform.Find("zuo (1)/Image/m_jichu").gameObject;
			m_produce_list = base.transform.Find("zuo (1)/Image/GameObject/m_produce_list").gameObject.GetComponent<UIGameObjectList>().objects;
			m_produce_listObj = base.transform.Find("zuo (1)/Image/GameObject/m_produce_list").gameObject;
			if (m_produce_listlist.Count <= 0)
			{
				for (int i = 0; i < m_produce_list.Length; i++)
				{
					m_produce_listlist.Add(View.AddComponentIfNotExist<GBagProduceListCell>(m_produce_list[i].gameObject));
				}
			}
			m_requirement = base.transform.Find("m_right/RawImage/m_requirement").gameObject.GetComponent<UIGameObjectList>().objects;
			m_requirementObj = base.transform.Find("m_right/RawImage/m_requirement").gameObject;
			if (m_requirementlist.Count <= 0)
			{
				for (int j = 0; j < m_requirement.Length; j++)
				{
					m_requirementlist.Add(View.AddComponentIfNotExist<GBagProduceRequirementCell>(m_requirement[j].gameObject));
				}
			}
			m_right = base.transform.Find("m_right").gameObject;
			m_sheshi = base.transform.Find("zuo (1)/Image/m_sheshi").gameObject;
			m_shortcut = base.transform.Find("GameObject/m_shortcut").gameObject.GetComponent<UIGameObjectList>().objects;
			m_shortcutObj = base.transform.Find("GameObject/m_shortcut").gameObject;
			if (m_shortcutlist.Count <= 0)
			{
				for (int k = 0; k < m_shortcut.Length; k++)
				{
					m_shortcutlist.Add(View.AddComponentIfNotExist<GBagProduceShortcutCell>(m_shortcut[k].gameObject));
				}
			}
			m_weapon_property_parent = base.transform.Find("m_right/m_weapon_property_parent").gameObject;
			m_wupin = base.transform.Find("zuo (1)/Image/m_wupin").gameObject;
			m_zaiju = base.transform.Find("zuo (1)/Image/m_zaiju").gameObject;
			m_zhandou = base.transform.Find("zuo (1)/Image/m_zhandou").gameObject;
			scp_items = base.transform.Find("zuo (1)/Image/scp_items").gameObject;
			txt_bag_full = base.transform.Find("zuo (1)/Image/GameObject/txt_bag_full").gameObject;
			txt_bag_fullText = txt_bag_full.GetComponent<Text>();
			txt_danLiang_weapon = base.transform.Find("m_right/m_weapon_property_parent/weapon_property/Image/txt_danLiang_weapon").gameObject;
			txt_danLiang_weaponText = txt_danLiang_weapon.GetComponent<Text>();
			txt_desc_select = base.transform.Find("m_right/RawImage/Image/txt_desc_select").gameObject;
			txt_desc_selectText = txt_desc_select.GetComponent<Text>();
			txt_locked = base.transform.Find("m_right/m_gray_ima/txt_locked").gameObject;
			txt_lockedText = txt_locked.GetComponent<Text>();
			txt_name_select = base.transform.Find("m_right/RawImage/bg/txt_name_select").gameObject;
			txt_name_selectText = txt_name_select.GetComponent<Text>();
			txt_shangHai_weapon = base.transform.Find("m_right/m_weapon_property_parent/weapon_property/Image/txt_shangHai_weapon").gameObject;
			txt_shangHai_weaponText = txt_shangHai_weapon.GetComponent<Text>();
			txt_sheCheng_weapon = base.transform.Find("m_right/m_weapon_property_parent/weapon_property/Image/txt_sheCheng_weapon").gameObject;
			txt_sheCheng_weaponText = txt_sheCheng_weapon.GetComponent<Text>();
			txt_sheSu_weapon = base.transform.Find("m_right/m_weapon_property_parent/weapon_property/Image/txt_sheSu_weapon").gameObject;
			txt_sheSu_weaponText = txt_sheSu_weapon.GetComponent<Text>();
			txt_time = base.transform.Find("m_right/GameObject (2)/GameObject (1)/Image (1)/txt_time").gameObject;
			txt_timeText = txt_time.GetComponent<Text>();
			txt_unlock_condition = base.transform.Find("m_right/txt_unlock_condition").gameObject;
			txt_unlock_conditionText = txt_unlock_condition.GetComponent<Text>();
			txt_wenDingXing_weapon = base.transform.Find("m_right/m_weapon_property_parent/weapon_property/Image/txt_wenDingXing_weapon").gameObject;
			txt_wenDingXing_weaponText = txt_wenDingXing_weapon.GetComponent<Text>();
			txt_zhuangDanSuDu_weapon = base.transform.Find("m_right/m_weapon_property_parent/weapon_property/Image/txt_zhuangDanSuDu_weapon").gameObject;
			txt_zhuangDanSuDu_weaponText = txt_zhuangDanSuDu_weapon.GetComponent<Text>();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__0(GameObject go)
		{
			int num = Convert.ToInt32(_inpCreateNum.text);
			_inpCreateNum.text = (num + 1).ToString();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__1(GameObject go)
		{
			int num = Convert.ToInt32(_inpCreateNum.text);
			_inpCreateNum.text = (num - 1).ToString();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__2(string str)
		{
			int num = Convert.ToInt32(str);
			if (num > _selectItem.MaxNumCanCreate && !_selectItem.IsMaterialEnough)
			{
				AlertBox.Show(119);
			}
			num = GetSuitableNum(num, _selectItem.MaxNumCanCreate);
			View.SetLabelText(txt_timeText, Utils.GetCountDownTime(_selectItem.SelectItemInfo.needTime * num));
			if (num != Convert.ToInt32(str))
			{
				_inpCreateNum.text = num.ToString();
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__3(GameObject go)
		{
			_inpCreateNum.text = _selectItem.MaxNumCanCreate.ToString();
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__4(GameObject go)
		{
			if (_selectCreateListIndex >= 0)
			{
				Singleton<ScProduceMgr>.Ins.SendCancelCreateMsg(_selectCreateListIndex);
				_selectCreateListIndex = -1;
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__5(GameObject go)
		{
			if (_roleLevel < ItemCfg.Get(_selectItem.SelectItemInfo.id).level)
			{
				AlertBox.Show(22);
			}
			else if (Singleton<BagMgr>.Ins.GetItemNum(_selectItem.SelectItemInfo.id) <= 0)
			{
				AlertBox.Show(_selectItem.SelectItemInfo.name + Utils.GetString(21));
			}
			else
			{
				Singleton<ScProduceMgr>.Ins.SendLearnDrawingMsg(_selectItem.SelectItemInfo.id);
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__6(GameObject go)
		{
			ItemCfg itemCfg = ItemCfg.Get(_selectItem.SelectItemInfo.id);
			if (itemCfg != null)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(itemCfg);
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__7(GameObject go)
		{
			if (Singleton<ScProduceMgr>.Ins.IsLockCreate)
			{
				AlertBox.Show(101);
				return;
			}
			if (Singleton<ScProduceMgr>.Ins.IsLocked(_selectItem.SelectItemInfo, _roleLevel))
			{
				AlertBox.Show(301);
				return;
			}
			int num = Convert.ToInt32(_inpCreateNum.text);
			if (num <= 0 && _selectItem != null && _selectItem.MaxNumCanCreate == 0)
			{
				List<DrawingNeedMaterial> material = _selectItem.SelectItemInfo.material;
				List<int> list = new List<int>();
				int i = 0;
				for (int count = material.Count; i < count; i++)
				{
					int num2 = material[i].num;
					if (Singleton<BagMgr>.Ins.GetItemNum(material[i].itemId) < num2)
					{
						list.Add(material[i].itemId);
					}
				}
				if (list.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int j = 0; j < list.Count; j++)
					{
						stringBuilder.Append(Utils.GetString(ItemCfg.Get(list[j]).name));
						stringBuilder.Append(',');
					}
					string text = stringBuilder.ToString();
					AlertBox.Show(10, text.TrimEnd(','));
				}
			}
			else
			{
				PretreatmentSendCreateMsg(num);
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__8(GameObject go)
		{
			if (_toggleShortcut.isOn)
			{
				if (Singleton<ScProduceMgr>.Ins.ShortcutItemList.Count >= cfg.Consts.PRODUCE_SHORT_CUT_NUM)
				{
					AlertBox.Show(249);
					_toggleShortcut.isOn = false;
				}
				else
				{
					Singleton<ScProduceMgr>.Ins.SendEditShortcutKeyMsg(_selectItem.SelectItemInfo.id, 1);
				}
			}
			else
			{
				Singleton<ScProduceMgr>.Ins.SendEditShortcutKeyMsg(_selectItem.SelectItemInfo.id, 2);
			}
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__9(GameObject go)
		{
			ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(ItemCfg.Get(_selectItem.SelectItemInfo.targetItemId));
		}

		[CompilerGenerated]
		private void _003COnInit_003Em__A(GameObject go)
		{
			List<CreateInfo> creatingItemsList = Singleton<ScProduceMgr>.Ins.CreatingItemsList;
			if (creatingItemsList.Count <= 0)
			{
				txt_bag_full.SetActiveBetter(false);
			}
			else if (Singleton<ScProduceMgr>.Ins.IsLockCreate && creatingItemsList != null && creatingItemsList.Count > 0)
			{
				if (!Singleton<BagMgr>.Ins.IsHasCapacity(DrawingCfg.Get(creatingItemsList[0].createId).targetItemId, 1))
				{
					AlertBox.Show(101);
				}
				else
				{
					Singleton<ScProduceMgr>.Ins.SendCGetFinishCreateMsg();
				}
			}
		}
	}
}
