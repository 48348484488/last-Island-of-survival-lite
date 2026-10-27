using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class SettingTrusteeshipPage : MonoBehaviour
	{
		[CompilerGenerated]
		private sealed class _003COnInit_003Ec__AnonStorey1
		{
			internal List<KeyValuePair<SettingMgr.TrusteeshipPickType, Toggle>> pickCkbList;

			internal Toggle toggle1;

			internal Toggle toggle2;

			internal UIScrollPanel collectPanel;

			internal SettingTrusteeshipPage _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				int i = 1;
				for (int count = pickCkbList.Count; i < count; i++)
				{
					pickCkbList[i].Value.isOn = pickCkbList[0].Value.isOn;
				}
			}

			internal void _003C_003Em__1(GameObject go)
			{
				toggle1.isOn = true;
				toggle2.isOn = true;
				collectPanel.UpdateAllCell(_0024this.SetAllCkbTrue);
				int i = 0;
				for (int count = pickCkbList.Count; i < count; i++)
				{
					pickCkbList[i].Value.isOn = true;
				}
			}

			internal void _003C_003Em__2(GameObject go)
			{
				toggle1.isOn = !toggle1.isOn;
				toggle2.isOn = !toggle2.isOn;
				collectPanel.UpdateAllCell(_0024this.SetAllCkbInverse);
				int i = 0;
				for (int count = pickCkbList.Count; i < count; i++)
				{
					KeyValuePair<SettingMgr.TrusteeshipPickType, Toggle> keyValuePair = pickCkbList[i];
					keyValuePair.Value.isOn = !keyValuePair.Value.isOn;
				}
			}
		}

		[CompilerGenerated]
		private sealed class _003COnInit_003Ec__AnonStorey0
		{
			internal KeyValuePair<SettingMgr.TrusteeshipPickType, Toggle> pair;

			internal void _003C_003Em__0(bool b)
			{
				SettingMgr.SetTrusteeshipPick(pair.Key, b);
			}
		}

		[CompilerGenerated]
		private sealed class _003CSetSlider_003Ec__AnonStorey2
		{
			internal Slider curSlider;

			internal bool isHp;

			internal GSettingTrusteeshipSliderItem sliderItem;

			internal void _003C_003Em__0(GameObject go)
			{
				curSlider.value -= 1f;
			}

			internal void _003C_003Em__1(GameObject go)
			{
				curSlider.value += 1f;
			}

			internal void _003C_003Em__2(float value)
			{
				if (isHp)
				{
					SettingMgr.SetTrusteeshipHealValue((int)value);
				}
				else
				{
					SettingMgr.SetTrusteeshipEatValue((int)value);
				}
				View.SetLabelText(sliderItem.txt_descText, Utils.GetString((!isHp) ? 367 : 366, (int)(value * 100f / curSlider.maxValue)));
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillCollectCell_003Ec__AnonStorey3
		{
			internal SettingMgr.TrusteeshipCollectType curType;

			internal int i1;

			internal void _003C_003Em__0(bool b)
			{
				SettingMgr.SetTrusteeshipCollectSetByIndex(curType, i1, b);
			}
		}

		private List<TrusteeshipCollectCfg> _allCollectCfg;

		public GameObject btn_all;

		public GameObject btn_help;

		public GameObject btn_inverse;

		public GameObject ckb_auto_food;

		public GameObject ckb_auto_medicine;

		public GSettingTrusteeshipCollectCell m_cell;

		public GameObject[] m_ckbs;

		public GameObject m_ckbsObj;

		public GSettingTrusteeshipSliderItem m_food_slider;

		public GameObject m_help;

		public GSettingTrusteeshipSliderItem m_hp_slider;

		public GameObject scp_collect;

		public object context;

		[CompilerGenerated]
		private static UnityAction<bool> _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static UnityAction<bool> _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

		public void OnInit()
		{
			_003COnInit_003Ec__AnonStorey1 _003COnInit_003Ec__AnonStorey = new _003COnInit_003Ec__AnonStorey1();
			_003COnInit_003Ec__AnonStorey._0024this = this;
			_allCollectCfg = TrusteeshipCollectCfg.GetAllList();
			_003COnInit_003Ec__AnonStorey.pickCkbList = new List<KeyValuePair<SettingMgr.TrusteeshipPickType, Toggle>>();
			string[] names = Enum.GetNames(typeof(SettingMgr.TrusteeshipPickType));
			int i = 0;
			for (int num = names.Length; i < num; i++)
			{
				_003COnInit_003Ec__AnonStorey.pickCkbList.Add(new KeyValuePair<SettingMgr.TrusteeshipPickType, Toggle>((SettingMgr.TrusteeshipPickType)i, m_ckbs[i].GetComponent<Toggle>()));
			}
			_003COnInit_003Ec__AnonStorey.toggle1 = ckb_auto_medicine.GetComponent<Toggle>();
			_003COnInit_003Ec__AnonStorey.toggle1.isOn = SettingMgr.GetTrusteeshipHeal();
			Toggle.ToggleEvent onValueChanged = _003COnInit_003Ec__AnonStorey.toggle1.onValueChanged;
			if (_003C_003Ef__mg_0024cache0 == null)
			{
				_003C_003Ef__mg_0024cache0 = SettingMgr.SetTrusteeshipHeal;
			}
			onValueChanged.AddListener(_003C_003Ef__mg_0024cache0);
			SetSlider(m_hp_slider, true);
			_003COnInit_003Ec__AnonStorey.toggle2 = ckb_auto_food.GetComponent<Toggle>();
			_003COnInit_003Ec__AnonStorey.toggle2.isOn = SettingMgr.GetTrusteeshipEat();
			Toggle.ToggleEvent onValueChanged2 = _003COnInit_003Ec__AnonStorey.toggle2.onValueChanged;
			if (_003C_003Ef__mg_0024cache1 == null)
			{
				_003C_003Ef__mg_0024cache1 = SettingMgr.SetTrusteeshipEat;
			}
			onValueChanged2.AddListener(_003C_003Ef__mg_0024cache1);
			SetSlider(m_food_slider, false);
			_003COnInit_003Ec__AnonStorey.collectPanel = scp_collect.GetComponent<UIScrollPanel>();
			_003COnInit_003Ec__AnonStorey.collectPanel.Reset(_allCollectCfg.Count, FillCollectCell);
			int j = 0;
			for (int count = _003COnInit_003Ec__AnonStorey.pickCkbList.Count; j < count; j++)
			{
				_003COnInit_003Ec__AnonStorey0 _003COnInit_003Ec__AnonStorey2 = new _003COnInit_003Ec__AnonStorey0();
				_003COnInit_003Ec__AnonStorey2.pair = _003COnInit_003Ec__AnonStorey.pickCkbList[j];
				_003COnInit_003Ec__AnonStorey2.pair.Value.isOn = SettingMgr.GetTrusteeshipPick(_003COnInit_003Ec__AnonStorey2.pair.Key);
				_003COnInit_003Ec__AnonStorey2.pair.Value.onValueChanged.AddListener(_003COnInit_003Ec__AnonStorey2._003C_003Em__0);
			}
			ClickListener.Get(m_ckbs[0], string.Empty).onClick = _003COnInit_003Ec__AnonStorey._003C_003Em__0;
			ClickListener clickListener = ClickListener.Get(btn_help, string.Empty);
			ClickListener clickListener2 = ClickListener.Get(m_help, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnInit_003Em__0;
			}
			clickListener.onClick = (clickListener2.onClick = _003C_003Ef__am_0024cache0);
			ClickListener.Get(btn_all, string.Empty).onClick = _003COnInit_003Ec__AnonStorey._003C_003Em__1;
			ClickListener.Get(btn_inverse, string.Empty).onClick = _003COnInit_003Ec__AnonStorey._003C_003Em__2;
		}

		private void SetAllCkbInverse(GameObject go, int index)
		{
			GSettingTrusteeshipCollectCell component = go.GetComponent<GSettingTrusteeshipCollectCell>();
			TrusteeshipCollectCfg trusteeshipCollectCfg = _allCollectCfg[index];
			List<TrusteeshipCollectItem> items = trusteeshipCollectCfg.items;
			int i = 0;
			for (int num = component.m_ckbs.Length; i < num; i++)
			{
				if (items.Count > i)
				{
					GameObject gameObject = component.m_ckbs[i];
					Toggle component2 = gameObject.GetComponent<Toggle>();
					component2.isOn = !component2.isOn;
				}
			}
		}

		private void SetAllCkbTrue(GameObject go, int index)
		{
			GSettingTrusteeshipCollectCell component = go.GetComponent<GSettingTrusteeshipCollectCell>();
			TrusteeshipCollectCfg trusteeshipCollectCfg = _allCollectCfg[index];
			List<TrusteeshipCollectItem> items = trusteeshipCollectCfg.items;
			int i = 0;
			for (int num = component.m_ckbs.Length; i < num; i++)
			{
				if (items.Count > i)
				{
					GameObject gameObject = component.m_ckbs[i];
					Toggle component2 = gameObject.GetComponent<Toggle>();
					component2.isOn = true;
				}
			}
		}

		public void OnShow()
		{
			MultiLanguageEvent.RefreshFixedLableDelegate = (Utils.VoidDelegate)Delegate.Combine(MultiLanguageEvent.RefreshFixedLableDelegate, new Utils.VoidDelegate(OnChangeLanguage));
		}

		public void OnHide()
		{
			MultiLanguageEvent.RefreshFixedLableDelegate = (Utils.VoidDelegate)Delegate.Remove(MultiLanguageEvent.RefreshFixedLableDelegate, new Utils.VoidDelegate(OnChangeLanguage));
		}

		private void OnChangeLanguage()
		{
			UIScrollPanel component = scp_collect.GetComponent<UIScrollPanel>();
			component.Reset(_allCollectCfg.Count, FillCollectCell);
		}

		private void SetSlider(GSettingTrusteeshipSliderItem sliderItem, bool isHp)
		{
			_003CSetSlider_003Ec__AnonStorey2 _003CSetSlider_003Ec__AnonStorey = new _003CSetSlider_003Ec__AnonStorey2();
			_003CSetSlider_003Ec__AnonStorey.isHp = isHp;
			_003CSetSlider_003Ec__AnonStorey.sliderItem = sliderItem;
			_003CSetSlider_003Ec__AnonStorey.curSlider = _003CSetSlider_003Ec__AnonStorey.sliderItem.m_slide.GetComponent<Slider>();
			_003CSetSlider_003Ec__AnonStorey.curSlider.maxValue = ((!_003CSetSlider_003Ec__AnonStorey.isHp) ? ConstsBs.MAX_HUNGER : ConstsBs.HpPlayer);
			_003CSetSlider_003Ec__AnonStorey.curSlider.minValue = 0.1f;
			_003CSetSlider_003Ec__AnonStorey.curSlider.value = ((!_003CSetSlider_003Ec__AnonStorey.isHp) ? SettingMgr.GetTrusteeshipEatValue() : SettingMgr.GetTrusteeshipHealValue());
			View.SetLabelText(_003CSetSlider_003Ec__AnonStorey.sliderItem.txt_descText, Utils.GetString((!_003CSetSlider_003Ec__AnonStorey.isHp) ? 367 : 366, (int)(_003CSetSlider_003Ec__AnonStorey.curSlider.value * 100f / _003CSetSlider_003Ec__AnonStorey.curSlider.maxValue)));
			ClickListener.Get(_003CSetSlider_003Ec__AnonStorey.sliderItem.btn_down, string.Empty).onClick = _003CSetSlider_003Ec__AnonStorey._003C_003Em__0;
			ClickListener.Get(_003CSetSlider_003Ec__AnonStorey.sliderItem.btn_up, string.Empty).onClick = _003CSetSlider_003Ec__AnonStorey._003C_003Em__1;
			_003CSetSlider_003Ec__AnonStorey.curSlider.onValueChanged.AddListener(_003CSetSlider_003Ec__AnonStorey._003C_003Em__2);
		}

		private void FillCollectCell(GameObject go, int index)
		{
			GSettingTrusteeshipCollectCell component = go.GetComponent<GSettingTrusteeshipCollectCell>();
			TrusteeshipCollectCfg trusteeshipCollectCfg = _allCollectCfg[index];
			List<TrusteeshipCollectItem> items = trusteeshipCollectCfg.items;
			int i = 0;
			for (int num = component.m_ckbs.Length; i < num; i++)
			{
				GameObject gameObject = component.m_ckbs[i];
				if (items.Count > i)
				{
					_003CFillCollectCell_003Ec__AnonStorey3 _003CFillCollectCell_003Ec__AnonStorey = new _003CFillCollectCell_003Ec__AnonStorey3();
					gameObject.SetActiveBetter(true);
					TrusteeshipCollectItem trusteeshipCollectItem = items[i];
					GSettingTrusteeshipCollectCkbCell gSettingTrusteeshipCollectCkbCell = component.m_ckbslist[i];
					ItemCfg itemCfg = ItemCfg.Get(trusteeshipCollectItem.itemId);
					if (itemCfg != null)
					{
						View.SetLabelText(gSettingTrusteeshipCollectCkbCell.txt_nameText, itemCfg.name);
					}
					_003CFillCollectCell_003Ec__AnonStorey.curType = GetCollectTypeByCollectCfgId(trusteeshipCollectCfg.id, i);
					Toggle component2 = gameObject.GetComponent<Toggle>();
					component2.isOn = SettingMgr.GetTrusteeshipCollectSetByIndex(_003CFillCollectCell_003Ec__AnonStorey.curType, i);
					_003CFillCollectCell_003Ec__AnonStorey.i1 = i;
					component2.onValueChanged.RemoveAllListeners();
					component2.onValueChanged.AddListener(_003CFillCollectCell_003Ec__AnonStorey._003C_003Em__0);
					View.SetLabelText(gSettingTrusteeshipCollectCkbCell.txt_nameText, trusteeshipCollectItem.name);
				}
				else
				{
					gameObject.SetActiveBetter(false);
				}
			}
		}

		private SettingMgr.TrusteeshipCollectType GetCollectTypeByCollectCfgId(int id, int indexInCfg)
		{
			if (id == 1 && indexInCfg == 0)
			{
				return SettingMgr.TrusteeshipCollectType.Tree;
			}
			return (SettingMgr.TrusteeshipCollectType)id;
		}

		private void Awake()
		{
			btn_all = base.transform.Find("GameObject/btn_all").gameObject;
			btn_help = base.transform.Find("GameObject/btn_help").gameObject;
			btn_inverse = base.transform.Find("GameObject/btn_inverse").gameObject;
			ckb_auto_food = base.transform.Find("eat/ckb_auto_food").gameObject;
			ckb_auto_medicine = base.transform.Find("eat/ckb_auto_medicine").gameObject;
			m_cell = View.AddComponentIfNotExist<GSettingTrusteeshipCollectCell>(base.transform.Find("pick/GameObject/scp_collect/content/m_cell").gameObject);
			m_ckbs = base.transform.Find("pick/m_ckbs").gameObject.GetComponent<UIGameObjectList>().objects;
			m_ckbsObj = base.transform.Find("pick/m_ckbs").gameObject;
			m_food_slider = View.AddComponentIfNotExist<GSettingTrusteeshipSliderItem>(base.transform.Find("eat/m_food_slider").gameObject);
			m_help = base.transform.Find("m_help").gameObject;
			m_hp_slider = View.AddComponentIfNotExist<GSettingTrusteeshipSliderItem>(base.transform.Find("eat/m_hp_slider").gameObject);
			scp_collect = base.transform.Find("pick/GameObject/scp_collect").gameObject;
		}

		[CompilerGenerated]
		private static void _003COnInit_003Em__0(GameObject go)
		{
			ViewMgr.Ins.ShowTopView<SettingPopupPanel>();
		}
	}
}
