using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;

namespace SC.UI
{
	public class FirePopupPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CFillFormulaCell_003Ec__AnonStorey1
		{
			internal ItemCfg productionItemCfgInfo;

			internal void _003C_003Em__0(GameObject o)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(productionItemCfgInfo);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillFormulaCell_003Ec__AnonStorey0
		{
			internal ItemCfg materialItemCfgInfo;

			internal void _003C_003Em__0(GameObject o)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(materialItemCfgInfo);
			}
		}

		[CompilerGenerated]
		private sealed class _003CUpdateFuel_003Ec__AnonStorey2
		{
			internal ItemCfg fuelItemCfg;

			internal void _003C_003Em__0(GameObject go)
			{
				ViewMgr.Ins.ShowTopView<BagItemInfoPanel>(fuelItemCfg);
			}
		}

		public static string HadShowedParamName = "FirePopup";

		private UIScrollPanel _formulaScrollPanel;

		private List<CookbookCfg> _allCookbookInfos;

		private List<SmelterFuelCfg> _allFuelInfos;

		private GameObject btn_desc;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_formula;

		private GameObject m_formula;

		private GameObject scp_formula;

		private GFirePopupFormulaCell m_cell;

		private GameObject m_desc;

		private List<GFirePopupFuelCell> m_fuelslist = new List<GFirePopupFuelCell>();

		private GameObject[] m_fuels;

		private GameObject m_fuelsObj;

		private GameObject m_bg;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		protected override void onInit()
		{
			base.onInit();
			m_cell.gameObject.SetActive(false);
			_formulaScrollPanel = scp_formula.GetComponent<UIScrollPanel>();
			_allCookbookInfos = CookbookCfg.GetAllList();
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
			_formulaScrollPanel.Reset(_allCookbookInfos.Count, FillFormulaCell);
		}

		private void FillFormulaCell(GameObject go, int index)
		{
			_003CFillFormulaCell_003Ec__AnonStorey1 _003CFillFormulaCell_003Ec__AnonStorey = new _003CFillFormulaCell_003Ec__AnonStorey1();
			GFirePopupFormulaCell component = go.GetComponent<GFirePopupFormulaCell>();
			CookbookCfg cookbookCfg = _allCookbookInfos[index];
			Dictionary<int, int> material = cookbookCfg.material;
			int i = 0;
			foreach (KeyValuePair<int, int> item in material)
			{
				_003CFillFormulaCell_003Ec__AnonStorey0 _003CFillFormulaCell_003Ec__AnonStorey2 = new _003CFillFormulaCell_003Ec__AnonStorey0();
				component.m_materials[i].SetActiveBetter(true);
				GFirePopupMaterialCell gFirePopupMaterialCell = component.m_materialslist[i];
				_003CFillFormulaCell_003Ec__AnonStorey2.materialItemCfgInfo = ItemCfg.Get(item.Key);
				View.SetItemSprite(gFirePopupMaterialCell.m_icon_material, _003CFillFormulaCell_003Ec__AnonStorey2.materialItemCfgInfo.icon);
				View.SetLabelText(gFirePopupMaterialCell.txt_name_numText, Utils.GetString(145, _003CFillFormulaCell_003Ec__AnonStorey2.materialItemCfgInfo.name, item.Value));
				ClickListener.Get(gFirePopupMaterialCell.m_icon_material, string.Empty).onClick = _003CFillFormulaCell_003Ec__AnonStorey2._003C_003Em__0;
				i++;
			}
			for (int num = component.m_materials.Length; i < num; i++)
			{
				component.m_materials[i].SetActiveBetter(false);
			}
			View.SetLabelText(component.txt_timeText, Utils.GetString(116, cookbookCfg.time));
			_003CFillFormulaCell_003Ec__AnonStorey.productionItemCfgInfo = ItemCfg.Get(cookbookCfg.id);
			View.SetItemSprite(component.m_icon_production, _003CFillFormulaCell_003Ec__AnonStorey.productionItemCfgInfo.icon);
			View.SetLabelText(component.txt_name_numText, Utils.GetString(145, _003CFillFormulaCell_003Ec__AnonStorey.productionItemCfgInfo.name, 1));
			ClickListener.Get(component.m_icon_production, string.Empty).onClick = _003CFillFormulaCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private void UpdateFuel()
		{
			int i = 0;
			for (int num = m_fuels.Length; i < num; i++)
			{
				if (_allFuelInfos.Count > i)
				{
					_003CUpdateFuel_003Ec__AnonStorey2 _003CUpdateFuel_003Ec__AnonStorey = new _003CUpdateFuel_003Ec__AnonStorey2();
					m_fuels[i].SetActiveBetter(true);
					GFirePopupFuelCell gFirePopupFuelCell = m_fuelslist[i];
					_003CUpdateFuel_003Ec__AnonStorey.fuelItemCfg = ItemCfg.Get(_allFuelInfos[i].id);
					View.SetItemSprite(gFirePopupFuelCell.m_icon, _003CUpdateFuel_003Ec__AnonStorey.fuelItemCfg.icon);
					View.SetLabelText(gFirePopupFuelCell.txt_nameText, _003CUpdateFuel_003Ec__AnonStorey.fuelItemCfg.name);
					ClickListener.Get(gFirePopupFuelCell.m_icon, string.Empty).onClick = _003CUpdateFuel_003Ec__AnonStorey._003C_003Em__0;
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
			btn_desc = component.GameObjects[0].gameObject;
			txt_on = component.GameObjects[1].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[2].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_formula = component.GameObjects[3].gameObject;
			m_formula = component.GameObjects[4].gameObject;
			scp_formula = component.GameObjects[5].gameObject;
			m_cell = View.AddComponentIfNotExist<GFirePopupFormulaCell>(component.GameObjects[6].gameObject);
			m_desc = component.GameObjects[7].gameObject;
			m_fuels = component.GameObjects[8].gameObject.GetComponent<UIGameObjectList>().objects;
			m_fuelsObj = component.GameObjects[8].gameObject;
			if (m_fuelslist.Count <= 0)
			{
				for (int i = 0; i < m_fuels.Length; i++)
				{
					m_fuelslist.Add(View.AddComponentIfNotExist<GFirePopupFuelCell>(m_fuels[i].gameObject));
				}
			}
			m_bg = component.GameObjects[9].gameObject;
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
