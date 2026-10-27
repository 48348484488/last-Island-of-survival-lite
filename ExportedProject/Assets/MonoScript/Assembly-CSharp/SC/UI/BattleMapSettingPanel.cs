using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class BattleMapSettingPanel : View
	{
		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey0
		{
			internal int index;

			internal BattleMapSettingPanel _0024this;

			internal void _003C_003Em__0(bool value)
			{
				if (value)
				{
					Singleton<MapMgr>.Ins.SetMainResource(_0024this._mainIdsInts[index]);
				}
				else
				{
					Singleton<MapMgr>.Ins.RemoveMainResource(_0024this._mainIdsInts[index]);
				}
				_0024this.InitAllResourceSetting();
				_0024this.SetMapTag();
			}
		}

		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey1
		{
			internal int index;

			internal BattleMapSettingPanel _0024this;

			internal void _003C_003Em__0(bool value)
			{
				if (value)
				{
					Singleton<MapMgr>.Ins.SetPlantResource(_0024this._plantIdsInts[index]);
				}
				else
				{
					Singleton<MapMgr>.Ins.RemovePlantResource(_0024this._plantIdsInts[index]);
				}
				_0024this.InitAllResourceSetting();
				_0024this.SetMapTag();
			}
		}

		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey2
		{
			internal int index;

			internal BattleMapSettingPanel _0024this;

			internal void _003C_003Em__0(bool value)
			{
				if (value)
				{
					Singleton<MapMgr>.Ins.SetTreeResource(_0024this._treeIdsInts[index]);
				}
				else
				{
					Singleton<MapMgr>.Ins.RemoveTreeResource(_0024this._treeIdsInts[index]);
				}
				_0024this.InitAllResourceSetting();
				_0024this.SetMapTag();
			}
		}

		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey3
		{
			internal int index;

			internal BattleMapSettingPanel _0024this;

			internal void _003C_003Em__0(bool value)
			{
				if (value)
				{
					Singleton<MapMgr>.Ins.SetLajitongResource(_lajitongIds[index]);
				}
				else
				{
					Singleton<MapMgr>.Ins.RemoveLajitongResource(_lajitongIds[index]);
				}
				_0024this.InitAllResourceSetting();
				_0024this.SetMapTag();
			}
		}

		[CompilerGenerated]
		private sealed class _003ConInit_003Ec__AnonStorey4
		{
			internal int index;

			internal BattleMapSettingPanel _0024this;

			internal void _003C_003Em__0(bool value)
			{
				if (value)
				{
					Singleton<MapMgr>.Ins.SetXiangziResource(_xiagnziIds[index]);
				}
				else
				{
					Singleton<MapMgr>.Ins.RemoveXiangziResource(_xiagnziIds[index]);
				}
				_0024this.InitAllResourceSetting();
				_0024this.SetMapTag();
			}
		}

		private Toggle _ckbToolBoxPosToggle;

		private Toggle _ckbSecondBattlePosToggle;

		private Toggle _ckbTeamPosToggle;

		private Toggle _ckbAirdropPostoToggle;

		private Toggle _ckbDieToggle;

		private Toggle _ckbLajitongToggle;

		private Toggle _ckbXiangziToggle;

		private List<Toggle> _ckbMainToggles = new List<Toggle>();

		private List<Toggle> _ckbPlantToggles = new List<Toggle>();

		private List<Toggle> _ckbTreeToggles = new List<Toggle>();

		private List<Toggle> _ckbLajitongToggles = new List<Toggle>();

		private List<Toggle> _ckbXiangziToggles = new List<Toggle>();

		private Toggle _ckbMainToggle;

		private Toggle _ckbPlantToggle;

		private Toggle _ckbTreeToggle;

		private int[] _mainIdsInts = new int[5] { 10001, 10003, 10004, 10005, 10002 };

		private int[] _plantIdsInts = new int[5] { 3, 4, 1, 2, 5 };

		private int[] _treeIdsInts = new int[2] { 1, 2 };

		public static int[] _lajitongIds = new int[2] { 20001, 20002 };

		public static int[] _xiagnziIds = new int[3] { 20003, 20004, 20005 };

		private GameObject btn_close;

		private GameObject[] m_map_tags;

		private GameObject m_map_tagsObj;

		private GameObject[] m_radioBtns;

		private GameObject m_radioBtnsObj;

		private GameObject btn_tool_box;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject btn_second_battle;

		private GameObject btn_team_mark;

		private GameObject btn_airdrop;

		private GameObject btn_die_pos;

		private GameObject btn_main;

		private GameObject m_main_toggle;

		private GameObject[] m_mains;

		private GameObject m_mainsObj;

		private GameObject btn_main_1;

		private GameObject btn_main_2;

		private GameObject btn_main_3;

		private GameObject btn_main_4;

		private GameObject btn_main_5;

		private GameObject btn_plant;

		private GameObject btn_plant_toggle;

		private GameObject[] m_plants;

		private GameObject m_plantsObj;

		private GameObject btn_plant_2;

		private GameObject btn_plant_3;

		private GameObject btn_plant_4;

		private GameObject btn_plant_5;

		private GameObject btn_tree;

		private GameObject btn_tree_Toggle;

		private GameObject[] m_trees;

		private GameObject m_treesObj;

		private GameObject btn_tree_1;

		private GameObject btn_tree_2;

		private GameObject btn_plant_1;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_on_1;

		private Text txt_on_1Text;

		private GameObject txt_on_2;

		private Text txt_on_2Text;

		private GameObject txt_on_3;

		private Text txt_on_3Text;

		private GameObject txt_on_4;

		private Text txt_on_4Text;

		private GameObject txt_on_5;

		private Text txt_on_5Text;

		private GameObject txt_on_6;

		private Text txt_on_6Text;

		private GameObject txt_on_7;

		private Text txt_on_7Text;

		private GameObject txt_on_8;

		private Text txt_on_8Text;

		private GameObject txt_on_9;

		private Text txt_on_9Text;

		private GameObject txt_on_10;

		private Text txt_on_10Text;

		private GameObject txt_on_11;

		private Text txt_on_11Text;

		private GameObject txt_on_12;

		private Text txt_on_12Text;

		private GameObject txt_on_13;

		private Text txt_on_13Text;

		private GameObject txt_on_14;

		private Text txt_on_14Text;

		private GameObject txt_on_15;

		private Text txt_on_15Text;

		private GameObject txt_on_16;

		private Text txt_on_16Text;

		private GameObject txt_on_17;

		private Text txt_on_17Text;

		private GameObject txt_on_18;

		private Text txt_on_18Text;

		private GameObject btn_lajitong;

		private GameObject txt_on_19;

		private Text txt_on_19Text;

		private GameObject btn_lajitong_toggle;

		private GameObject[] m_lajitong;

		private GameObject m_lajitongObj;

		private GameObject btn_lajitong_1;

		private GameObject txt_on_20;

		private Text txt_on_20Text;

		private GameObject btn_btn_lajitong_2;

		private GameObject txt_on_21;

		private Text txt_on_21Text;

		private GameObject btn_xiangzi;

		private GameObject txt_on_22;

		private Text txt_on_22Text;

		private GameObject btn_xiangzi_toggle;

		private GameObject[] m_xiangzi;

		private GameObject m_xiangziObj;

		private GameObject btn_xiangzi_1;

		private GameObject txt_on_23;

		private Text txt_on_23Text;

		private GameObject btn_xiangzi_2;

		private GameObject txt_on_24;

		private Text txt_on_24Text;

		private GameObject btn_xiangzi_3;

		private GameObject txt_on_25;

		private Text txt_on_25Text;

		public static bool ContainLajitong(int id)
		{
			int[] lajitongIds = _lajitongIds;
			foreach (int num in lajitongIds)
			{
				if (num == id)
				{
					return true;
				}
			}
			return false;
		}

		public static bool ContainXiangzi(int id)
		{
			int[] xiagnziIds = _xiagnziIds;
			foreach (int num in xiagnziIds)
			{
				if (num == id)
				{
					return true;
				}
			}
			return false;
		}

		protected override void onInit()
		{
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			_ckbToolBoxPosToggle = btn_tool_box.GetComponent<Toggle>();
			_ckbSecondBattlePosToggle = btn_second_battle.GetComponent<Toggle>();
			_ckbTeamPosToggle = btn_team_mark.GetComponent<Toggle>();
			_ckbAirdropPostoToggle = btn_airdrop.GetComponent<Toggle>();
			_ckbDieToggle = btn_die_pos.GetComponent<Toggle>();
			_ckbToolBoxPosToggle.onValueChanged.AddListener(OnToolBoxPos);
			_ckbSecondBattlePosToggle.onValueChanged.AddListener(OnSecondBattlePos);
			_ckbTeamPosToggle.onValueChanged.AddListener(OnTeamPos);
			_ckbAirdropPostoToggle.onValueChanged.AddListener(OnAirdropPos);
			_ckbDieToggle.onValueChanged.AddListener(OnDiePos);
			for (int i = 0; i < m_mains.Length; i++)
			{
				_003ConInit_003Ec__AnonStorey0 _003ConInit_003Ec__AnonStorey = new _003ConInit_003Ec__AnonStorey0();
				_003ConInit_003Ec__AnonStorey._0024this = this;
				_003ConInit_003Ec__AnonStorey.index = i;
				Toggle component = m_mains[i].GetComponent<Toggle>();
				component.isOn = Singleton<MapMgr>.Ins.IsContainMainResource(_mainIdsInts[_003ConInit_003Ec__AnonStorey.index]);
				component.onValueChanged.AddListener(_003ConInit_003Ec__AnonStorey._003C_003Em__0);
				_ckbMainToggles.Add(component);
			}
			for (int j = 0; j < m_plants.Length; j++)
			{
				_003ConInit_003Ec__AnonStorey1 _003ConInit_003Ec__AnonStorey2 = new _003ConInit_003Ec__AnonStorey1();
				_003ConInit_003Ec__AnonStorey2._0024this = this;
				_003ConInit_003Ec__AnonStorey2.index = j;
				Toggle component2 = m_plants[j].GetComponent<Toggle>();
				component2.isOn = Singleton<MapMgr>.Ins.IsContainPlantResource(_plantIdsInts[_003ConInit_003Ec__AnonStorey2.index]);
				component2.onValueChanged.AddListener(_003ConInit_003Ec__AnonStorey2._003C_003Em__0);
				_ckbPlantToggles.Add(component2);
			}
			for (int k = 0; k < m_trees.Length; k++)
			{
				_003ConInit_003Ec__AnonStorey2 _003ConInit_003Ec__AnonStorey3 = new _003ConInit_003Ec__AnonStorey2();
				_003ConInit_003Ec__AnonStorey3._0024this = this;
				_003ConInit_003Ec__AnonStorey3.index = k;
				Toggle component3 = m_trees[k].GetComponent<Toggle>();
				component3.isOn = Singleton<MapMgr>.Ins.IsContainMainResource(_treeIdsInts[_003ConInit_003Ec__AnonStorey3.index]);
				component3.onValueChanged.AddListener(_003ConInit_003Ec__AnonStorey3._003C_003Em__0);
				_ckbTreeToggles.Add(component3);
			}
			for (int l = 0; l < m_lajitong.Length; l++)
			{
				_003ConInit_003Ec__AnonStorey3 _003ConInit_003Ec__AnonStorey4 = new _003ConInit_003Ec__AnonStorey3();
				_003ConInit_003Ec__AnonStorey4._0024this = this;
				_003ConInit_003Ec__AnonStorey4.index = l;
				Toggle component4 = m_lajitong[l].GetComponent<Toggle>();
				component4.isOn = Singleton<MapMgr>.Ins.IsContainLajitongResource(_lajitongIds[_003ConInit_003Ec__AnonStorey4.index]);
				component4.onValueChanged.AddListener(_003ConInit_003Ec__AnonStorey4._003C_003Em__0);
				_ckbLajitongToggles.Add(component4);
			}
			for (int m = 0; m < m_xiangzi.Length; m++)
			{
				_003ConInit_003Ec__AnonStorey4 _003ConInit_003Ec__AnonStorey5 = new _003ConInit_003Ec__AnonStorey4();
				_003ConInit_003Ec__AnonStorey5._0024this = this;
				_003ConInit_003Ec__AnonStorey5.index = m;
				Toggle component5 = m_xiangzi[m].GetComponent<Toggle>();
				component5.isOn = Singleton<MapMgr>.Ins.IsContainXiangziResource(_xiagnziIds[_003ConInit_003Ec__AnonStorey5.index]);
				component5.onValueChanged.AddListener(_003ConInit_003Ec__AnonStorey5._003C_003Em__0);
				_ckbXiangziToggles.Add(component5);
			}
			_ckbMainToggle = m_main_toggle.GetComponent<Toggle>();
			_ckbPlantToggle = btn_plant_toggle.GetComponent<Toggle>();
			_ckbTreeToggle = btn_tree_Toggle.GetComponent<Toggle>();
			_ckbLajitongToggle = btn_lajitong_toggle.GetComponent<Toggle>();
			_ckbXiangziToggle = btn_xiangzi_toggle.GetComponent<Toggle>();
			ClickListener.Get(m_main_toggle, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_tree_Toggle, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_plant_toggle, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_lajitong_toggle, string.Empty).onClick = _003ConInit_003Em__4;
			ClickListener.Get(btn_xiangzi_toggle, string.Empty).onClick = _003ConInit_003Em__5;
			InitSetting();
		}

		public void InitResources()
		{
			for (int i = 0; i < m_mains.Length; i++)
			{
				_ckbMainToggles[i].isOn = Singleton<MapMgr>.Ins.IsContainMainResource(_mainIdsInts[i]);
			}
			for (int j = 0; j < m_plants.Length; j++)
			{
				_ckbPlantToggles[j].isOn = Singleton<MapMgr>.Ins.IsContainPlantResource(_plantIdsInts[j]);
			}
			for (int k = 0; k < m_trees.Length; k++)
			{
				_ckbTreeToggles[k].isOn = Singleton<MapMgr>.Ins.IsContainTreeResource(_treeIdsInts[k]);
			}
			for (int l = 0; l < m_lajitong.Length; l++)
			{
				_ckbLajitongToggles[l].isOn = Singleton<MapMgr>.Ins.IsContainLajitongResource(_lajitongIds[l]);
			}
			for (int m = 0; m < m_xiangzi.Length; m++)
			{
				_ckbXiangziToggles[m].isOn = Singleton<MapMgr>.Ins.IsContainXiangziResource(_xiagnziIds[m]);
			}
		}

		private void OnToolBoxPos(bool value)
		{
			MapMgr.ShowToolBoxInLittleMap = value;
			SetMapTag();
		}

		private void OnSecondBattlePos(bool value)
		{
			MapMgr.SecondBattleLittleMap = value;
			SetMapTag();
		}

		private void OnTeamPos(bool value)
		{
			MapMgr.TeamMarkPosLittleMap = value;
			SetMapTag();
		}

		private void OnAirdropPos(bool value)
		{
			MapMgr.AirDropLittleMapPos = value;
			SetMapTag();
		}

		private void OnDiePos(bool value)
		{
			MapMgr.DiePosLittleMap = value;
			SetMapTag();
		}

		private void InitSetting()
		{
			_ckbToolBoxPosToggle.isOn = MapMgr.ShowToolBoxInLittleMap;
			_ckbSecondBattlePosToggle.isOn = MapMgr.SecondBattleLittleMap;
			_ckbTeamPosToggle.isOn = MapMgr.TeamMarkPosLittleMap;
			_ckbAirdropPostoToggle.isOn = MapMgr.AirDropLittleMapPos;
			_ckbDieToggle.isOn = MapMgr.DiePosLittleMap;
			InitResources();
			InitAllResourceSetting();
		}

		private void InitAllResourceSetting()
		{
			_ckbMainToggle.isOn = IsShowAllResourceSetting(_ckbMainToggles);
			_ckbPlantToggle.isOn = IsShowAllResourceSetting(_ckbPlantToggles);
			_ckbTreeToggle.isOn = IsShowAllResourceSetting(_ckbTreeToggles);
			_ckbLajitongToggle.isOn = IsShowAllResourceSetting(_ckbLajitongToggles);
			_ckbXiangziToggle.isOn = IsShowAllResourceSetting(_ckbXiangziToggles);
		}

		private bool IsShowAllResourceSetting(List<Toggle> toggles)
		{
			for (int i = 0; i < toggles.Count; i++)
			{
				if (toggles[i].isOn)
				{
					return true;
				}
			}
			return false;
		}

		public void SetMapTag()
		{
			for (int i = 0; i < m_radioBtns.Length; i++)
			{
				m_map_tags[i].SetActiveBetter(View.IsCheckboxChecked(m_radioBtns[i]));
			}
		}

		protected override void onShow(object param = null, string childView = null)
		{
			SetMapTag();
		}

		protected override void onHide(string childView = null)
		{
		}

		protected override void onDestroy()
		{
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			m_map_tags = component.GameObjects[1].gameObject.GetComponent<UIGameObjectList>().objects;
			m_map_tagsObj = component.GameObjects[1].gameObject;
			m_radioBtns = component.GameObjects[2].gameObject.GetComponent<UIGameObjectList>().objects;
			m_radioBtnsObj = component.GameObjects[2].gameObject;
			btn_tool_box = component.GameObjects[3].gameObject;
			txt_on = component.GameObjects[4].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			btn_second_battle = component.GameObjects[5].gameObject;
			btn_team_mark = component.GameObjects[6].gameObject;
			btn_airdrop = component.GameObjects[7].gameObject;
			btn_die_pos = component.GameObjects[8].gameObject;
			btn_main = component.GameObjects[9].gameObject;
			m_main_toggle = component.GameObjects[10].gameObject;
			m_mains = component.GameObjects[11].gameObject.GetComponent<UIGameObjectList>().objects;
			m_mainsObj = component.GameObjects[11].gameObject;
			btn_main_1 = component.GameObjects[12].gameObject;
			btn_main_2 = component.GameObjects[13].gameObject;
			btn_main_3 = component.GameObjects[14].gameObject;
			btn_main_4 = component.GameObjects[15].gameObject;
			btn_main_5 = component.GameObjects[16].gameObject;
			btn_plant = component.GameObjects[17].gameObject;
			btn_plant_toggle = component.GameObjects[18].gameObject;
			m_plants = component.GameObjects[19].gameObject.GetComponent<UIGameObjectList>().objects;
			m_plantsObj = component.GameObjects[19].gameObject;
			btn_plant_2 = component.GameObjects[20].gameObject;
			btn_plant_3 = component.GameObjects[21].gameObject;
			btn_plant_4 = component.GameObjects[22].gameObject;
			btn_plant_5 = component.GameObjects[23].gameObject;
			btn_tree = component.GameObjects[24].gameObject;
			btn_tree_Toggle = component.GameObjects[25].gameObject;
			m_trees = component.GameObjects[26].gameObject.GetComponent<UIGameObjectList>().objects;
			m_treesObj = component.GameObjects[26].gameObject;
			btn_tree_1 = component.GameObjects[27].gameObject;
			btn_tree_2 = component.GameObjects[28].gameObject;
			btn_plant_1 = component.GameObjects[29].gameObject;
			txt_on_0 = component.GameObjects[30].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_on_1 = component.GameObjects[31].gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_on_2 = component.GameObjects[32].gameObject;
			txt_on_2Text = txt_on_2.GetComponent<Text>();
			txt_on_3 = component.GameObjects[33].gameObject;
			txt_on_3Text = txt_on_3.GetComponent<Text>();
			txt_on_4 = component.GameObjects[34].gameObject;
			txt_on_4Text = txt_on_4.GetComponent<Text>();
			txt_on_5 = component.GameObjects[35].gameObject;
			txt_on_5Text = txt_on_5.GetComponent<Text>();
			txt_on_6 = component.GameObjects[36].gameObject;
			txt_on_6Text = txt_on_6.GetComponent<Text>();
			txt_on_7 = component.GameObjects[37].gameObject;
			txt_on_7Text = txt_on_7.GetComponent<Text>();
			txt_on_8 = component.GameObjects[38].gameObject;
			txt_on_8Text = txt_on_8.GetComponent<Text>();
			txt_on_9 = component.GameObjects[39].gameObject;
			txt_on_9Text = txt_on_9.GetComponent<Text>();
			txt_on_10 = component.GameObjects[40].gameObject;
			txt_on_10Text = txt_on_10.GetComponent<Text>();
			txt_on_11 = component.GameObjects[41].gameObject;
			txt_on_11Text = txt_on_11.GetComponent<Text>();
			txt_on_12 = component.GameObjects[42].gameObject;
			txt_on_12Text = txt_on_12.GetComponent<Text>();
			txt_on_13 = component.GameObjects[43].gameObject;
			txt_on_13Text = txt_on_13.GetComponent<Text>();
			txt_on_14 = component.GameObjects[44].gameObject;
			txt_on_14Text = txt_on_14.GetComponent<Text>();
			txt_on_15 = component.GameObjects[45].gameObject;
			txt_on_15Text = txt_on_15.GetComponent<Text>();
			txt_on_16 = component.GameObjects[46].gameObject;
			txt_on_16Text = txt_on_16.GetComponent<Text>();
			txt_on_17 = component.GameObjects[47].gameObject;
			txt_on_17Text = txt_on_17.GetComponent<Text>();
			txt_on_18 = component.GameObjects[48].gameObject;
			txt_on_18Text = txt_on_18.GetComponent<Text>();
			btn_lajitong = component.GameObjects[49].gameObject;
			txt_on_19 = component.GameObjects[50].gameObject;
			txt_on_19Text = txt_on_19.GetComponent<Text>();
			btn_lajitong_toggle = component.GameObjects[51].gameObject;
			m_lajitong = component.GameObjects[52].gameObject.GetComponent<UIGameObjectList>().objects;
			m_lajitongObj = component.GameObjects[52].gameObject;
			btn_lajitong_1 = component.GameObjects[53].gameObject;
			txt_on_20 = component.GameObjects[54].gameObject;
			txt_on_20Text = txt_on_20.GetComponent<Text>();
			btn_btn_lajitong_2 = component.GameObjects[55].gameObject;
			txt_on_21 = component.GameObjects[56].gameObject;
			txt_on_21Text = txt_on_21.GetComponent<Text>();
			btn_xiangzi = component.GameObjects[57].gameObject;
			txt_on_22 = component.GameObjects[58].gameObject;
			txt_on_22Text = txt_on_22.GetComponent<Text>();
			btn_xiangzi_toggle = component.GameObjects[59].gameObject;
			m_xiangzi = component.GameObjects[60].gameObject.GetComponent<UIGameObjectList>().objects;
			m_xiangziObj = component.GameObjects[60].gameObject;
			btn_xiangzi_1 = component.GameObjects[61].gameObject;
			txt_on_23 = component.GameObjects[62].gameObject;
			txt_on_23Text = txt_on_23.GetComponent<Text>();
			btn_xiangzi_2 = component.GameObjects[63].gameObject;
			txt_on_24 = component.GameObjects[64].gameObject;
			txt_on_24Text = txt_on_24.GetComponent<Text>();
			btn_xiangzi_3 = component.GameObjects[65].gameObject;
			txt_on_25 = component.GameObjects[66].gameObject;
			txt_on_25Text = txt_on_25.GetComponent<Text>();
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
			if (_ckbMainToggle.isOn)
			{
				Singleton<MapMgr>.Ins.SetMainResource(_mainIdsInts);
			}
			else
			{
				Singleton<MapMgr>.Ins.RemoveAllMainResource();
			}
			InitResources();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			if (_ckbTreeToggle.isOn)
			{
				Singleton<MapMgr>.Ins.SetTreeResource(_treeIdsInts);
			}
			else
			{
				Singleton<MapMgr>.Ins.RemoveAllTreeResource();
			}
			InitResources();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			if (_ckbPlantToggle.isOn)
			{
				Singleton<MapMgr>.Ins.SetPlantResource(_plantIdsInts);
			}
			else
			{
				Singleton<MapMgr>.Ins.RemoveAllPlantResource();
			}
			InitResources();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			if (_ckbLajitongToggle.isOn)
			{
				Singleton<MapMgr>.Ins.SetLajitongResource(_lajitongIds);
			}
			else
			{
				Singleton<MapMgr>.Ins.RemoveAllLajitongResource();
			}
			InitResources();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__5(GameObject go)
		{
			if (_ckbXiangziToggle.isOn)
			{
				Singleton<MapMgr>.Ins.SetXiangziResource(_xiagnziIds);
			}
			else
			{
				Singleton<MapMgr>.Ins.RemoveAllXiangziResource();
			}
			InitResources();
		}
	}
}
