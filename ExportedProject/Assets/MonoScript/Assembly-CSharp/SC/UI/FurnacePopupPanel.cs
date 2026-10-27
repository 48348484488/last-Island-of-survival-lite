using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class FurnacePopupPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillFormulaCell_003Ec__AnonStorey0
		{
			internal ItemCfg materialItemCfgInfo;

			internal ItemCfg productionItemCfgInfo;

			internal void _003C_003Em__0(GameObject o)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(materialItemCfgInfo);
			}

			internal void _003C_003Em__1(GameObject o)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(productionItemCfgInfo);
			}
		}

		[CompilerGenerated]
		private sealed class _003CUpdateFuel_003Ec__AnonStorey1
		{
			internal ItemCfg fuelItemCfg;

			internal void _003C_003Em__0(GameObject go)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(fuelItemCfg);
			}
		}

		public static string HadShowedParamName = "FurnacePopup";

		private UIScrollPanel _formulaScrollPanel;

		private List<SmelterCfg> _allSmelterInfos;

		private List<SmelterFuelCfg> _allFuelInfos;

		private GameObject m_bg;

		private GameObject btn_desc;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_formula;

		private GameObject m_formula;

		private GameObject scp_formula;

		private GFurnacePopupFormulaCell m_cell;

		private GameObject m_desc;

		private List<GFurnacePopupFuelCell> m_fuelslist = new List<GFurnacePopupFuelCell>();

		private GameObject[] m_fuels;

		private GameObject m_fuelsObj;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		protected override void onInit()
		{
			base.onInit();
			m_cell.gameObject.SetActive(false);
			_formulaScrollPanel = scp_formula.GetComponent<UIScrollPanel>();
			_allSmelterInfos = SmelterCfg.GetAllList();
			_allFuelInfos = SmelterFuelCfg.GetAllList();
			ClickListener.Get(m_bg, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_desc, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_formula, string.Empty).onClick = _003ConInit_003Em__2;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			m_desc.SetActiveBetter(true);
			m_formula.SetActiveBetter(false);
			RadioButton.ChooseBtn(btn_desc);
			UpdateFormula();
			UpdateFuel();
		}

		private void UpdateFormula()
		{
			_formulaScrollPanel.Reset(_allSmelterInfos.Count, FillFormulaCell);
		}

		private void FillFormulaCell(GameObject go, int index)
		{
			_003CFillFormulaCell_003Ec__AnonStorey0 _003CFillFormulaCell_003Ec__AnonStorey = new _003CFillFormulaCell_003Ec__AnonStorey0();
			GFurnacePopupFormulaCell component = go.GetComponent<GFurnacePopupFormulaCell>();
			SmelterCfg smelterCfg = _allSmelterInfos[index];
			_003CFillFormulaCell_003Ec__AnonStorey.materialItemCfgInfo = ItemCfg.Get(smelterCfg.id);
			_003CFillFormulaCell_003Ec__AnonStorey.productionItemCfgInfo = ItemCfg.Get(smelterCfg.targetItemId);
			View.SetItemSprite(component.m_icon_material, _003CFillFormulaCell_003Ec__AnonStorey.materialItemCfgInfo.icon);
			View.SetLabelText(component.txt_num_materialText, Utils.GetString(145, _003CFillFormulaCell_003Ec__AnonStorey.materialItemCfgInfo.name, 1));
			View.SetItemSprite(component.m_icon_production, _003CFillFormulaCell_003Ec__AnonStorey.productionItemCfgInfo.icon);
			View.SetLabelText(component.txt_num_productionText, Utils.GetString(145, _003CFillFormulaCell_003Ec__AnonStorey.productionItemCfgInfo.name, smelterCfg.targetNum));
			View.SetLabelText(component.txt_timeText, Utils.GetString(116, smelterCfg.needTime));
			ClickListener.Get(component.m_icon_material, string.Empty).onClick = _003CFillFormulaCell_003Ec__AnonStorey._003C_003Em__0;
			ClickListener.Get(component.m_icon_production, string.Empty).onClick = _003CFillFormulaCell_003Ec__AnonStorey._003C_003Em__1;
		}

		private void UpdateFuel()
		{
			int i = 0;
			for (int num = m_fuels.Length; i < num; i++)
			{
				if (_allFuelInfos.Count > i)
				{
					_003CUpdateFuel_003Ec__AnonStorey1 _003CUpdateFuel_003Ec__AnonStorey = new _003CUpdateFuel_003Ec__AnonStorey1();
					m_fuels[i].SetActiveBetter(true);
					GFurnacePopupFuelCell gFurnacePopupFuelCell = m_fuelslist[i];
					_003CUpdateFuel_003Ec__AnonStorey.fuelItemCfg = ItemCfg.Get(_allFuelInfos[i].id);
					View.SetItemSprite(gFurnacePopupFuelCell.m_icon, _003CUpdateFuel_003Ec__AnonStorey.fuelItemCfg.icon);
					View.SetLabelText(gFurnacePopupFuelCell.txt_nameText, _003CUpdateFuel_003Ec__AnonStorey.fuelItemCfg.name);
					ClickListener.Get(gFurnacePopupFuelCell.m_icon, string.Empty).onClick = _003CUpdateFuel_003Ec__AnonStorey._003C_003Em__0;
				}
				else
				{
					m_fuels[i].SetActiveBetter(false);
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_bg = component.GameObjects[0].gameObject;
			btn_desc = component.GameObjects[1].gameObject;
			txt_on = component.GameObjects[2].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[3].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_formula = component.GameObjects[4].gameObject;
			m_formula = component.GameObjects[5].gameObject;
			scp_formula = component.GameObjects[6].gameObject;
			m_cell = View.AddComponentIfNotExist<GFurnacePopupFormulaCell>(component.GameObjects[7].gameObject);
			m_desc = component.GameObjects[8].gameObject;
			m_fuels = component.GameObjects[9].gameObject.GetComponent<UIGameObjectList>().objects;
			m_fuelsObj = component.GameObjects[9].gameObject;
			if (m_fuelslist.Count <= 0)
			{
				for (int i = 0; i < m_fuels.Length; i++)
				{
					m_fuelslist.Add(View.AddComponentIfNotExist<GFurnacePopupFuelCell>(m_fuels[i].gameObject));
				}
			}
			txt_on_0 = component.GameObjects[10].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[11].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
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
			m_desc.SetActiveBetter(true);
			m_formula.SetActiveBetter(false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			m_desc.SetActiveBetter(false);
			m_formula.SetActiveBetter(true);
		}
	}
}
