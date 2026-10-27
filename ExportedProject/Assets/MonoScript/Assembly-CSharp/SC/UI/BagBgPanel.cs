using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class BagBgPanel : View
	{
		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		private GameObject btn_bag;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject txt_bag_num;

		private Text txt_bag_numText;

		private GameObject btn_Build;

		private GameObject m_bag_page;

		private GameObject scp_bag;

		private GBagItem m_cell;

		private GameObject m_equipment;

		private GameObject btn_equipment_r;

		private GameObject btn_equipment;

		private List<GEquipmentsRItem> m_equipments_rlist = new List<GEquipmentsRItem>();

		private GameObject[] m_equipments_r;

		private GameObject m_equipments_rObj;

		private List<GEquipmentsRItem> m_equipmentslist = new List<GEquipmentsRItem>();

		private GameObject[] m_equipments;

		private GameObject m_equipmentsObj;

		private List<GShortcutBarItem> m_shortcut_barlist = new List<GShortcutBarItem>();

		private GameObject[] m_shortcut_bar;

		private GameObject m_shortcut_barObj;

		private GameObject m_desc;

		private GameObject m_icon_desc;

		private GameObject m_durability_desc;

		private GameObject txt_name_desc;

		private Text txt_name_descText;

		private GameObject txt_num_desc;

		private Text txt_num_descText;

		private GameObject txt_durability_desc;

		private Text txt_durability_descText;

		private GameObject txt_desc_desc;

		private Text txt_desc_descText;

		private GameObject btn_sell;

		private GameObject btn_discard;

		private GameObject btn_split;

		private GameObject btn_combination;

		private GameObject btn_to_shortcut_bar;

		private GameObject btn_out_shortcut_bar;

		private GameObject btn_use;

		private GameObject btn_equip;

		private GameObject m_mask;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		private GameObject txt_on_1;

		private Text txt_on_1Text;

		private GameObject txt_off_1;

		private Text txt_off_1Text;

		private GameObject txt_on_2;

		private Text txt_on_2Text;

		private GameObject txt_off_2;

		private Text txt_off_2Text;

		private GameObject btn_back_desc;

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_back = component.GameObjects[0].gameObject;
			txt_blood = component.GameObjects[1].gameObject;
			txt_bloodText = txt_blood.GetComponent<Text>();
			txt_hunger = component.GameObjects[2].gameObject;
			txt_hungerText = txt_hunger.GetComponent<Text>();
			txt_thirst = component.GameObjects[3].gameObject;
			txt_thirstText = txt_thirst.GetComponent<Text>();
			btn_bag = component.GameObjects[4].gameObject;
			txt_on = component.GameObjects[5].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[6].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			txt_bag_num = component.GameObjects[7].gameObject;
			txt_bag_numText = txt_bag_num.GetComponent<Text>();
			btn_Build = component.GameObjects[8].gameObject;
			m_bag_page = component.GameObjects[9].gameObject;
			scp_bag = component.GameObjects[10].gameObject;
			m_cell = View.AddComponentIfNotExist<GBagItem>(component.GameObjects[11].gameObject);
			m_equipment = component.GameObjects[12].gameObject;
			btn_equipment_r = component.GameObjects[13].gameObject;
			btn_equipment = component.GameObjects[14].gameObject;
			m_equipments_r = component.GameObjects[15].gameObject.GetComponent<UIGameObjectList>().objects;
			m_equipments_rObj = component.GameObjects[15].gameObject;
			for (int i = 0; i < m_equipments_r.Length; i++)
			{
				m_equipments_rlist.Add(View.AddComponentIfNotExist<GEquipmentsRItem>(m_equipments_r[i].gameObject));
			}
			m_equipments = component.GameObjects[16].gameObject.GetComponent<UIGameObjectList>().objects;
			m_equipmentsObj = component.GameObjects[16].gameObject;
			for (int j = 0; j < m_equipments.Length; j++)
			{
				m_equipmentslist.Add(View.AddComponentIfNotExist<GEquipmentsRItem>(m_equipments[j].gameObject));
			}
			m_shortcut_bar = component.GameObjects[17].gameObject.GetComponent<UIGameObjectList>().objects;
			m_shortcut_barObj = component.GameObjects[17].gameObject;
			for (int k = 0; k < m_shortcut_bar.Length; k++)
			{
				m_shortcut_barlist.Add(View.AddComponentIfNotExist<GShortcutBarItem>(m_shortcut_bar[k].gameObject));
			}
			m_desc = component.GameObjects[18].gameObject;
			m_icon_desc = component.GameObjects[19].gameObject;
			m_durability_desc = component.GameObjects[20].gameObject;
			txt_name_desc = component.GameObjects[21].gameObject;
			txt_name_descText = txt_name_desc.GetComponent<Text>();
			txt_num_desc = component.GameObjects[22].gameObject;
			txt_num_descText = txt_num_desc.GetComponent<Text>();
			txt_durability_desc = component.GameObjects[23].gameObject;
			txt_durability_descText = txt_durability_desc.GetComponent<Text>();
			txt_desc_desc = component.GameObjects[24].gameObject;
			txt_desc_descText = txt_desc_desc.GetComponent<Text>();
			btn_sell = component.GameObjects[25].gameObject;
			btn_discard = component.GameObjects[26].gameObject;
			btn_split = component.GameObjects[27].gameObject;
			btn_combination = component.GameObjects[28].gameObject;
			btn_to_shortcut_bar = component.GameObjects[29].gameObject;
			btn_out_shortcut_bar = component.GameObjects[30].gameObject;
			btn_use = component.GameObjects[31].gameObject;
			btn_equip = component.GameObjects[32].gameObject;
			m_mask = component.GameObjects[33].gameObject;
			txt_on_0 = component.GameObjects[34].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[35].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			txt_on_1 = component.GameObjects[36].gameObject;
			txt_on_1Text = txt_on_1.GetComponent<Text>();
			txt_off_1 = component.GameObjects[37].gameObject;
			txt_off_1Text = txt_off_1.GetComponent<Text>();
			txt_on_2 = component.GameObjects[38].gameObject;
			txt_on_2Text = txt_on_2.GetComponent<Text>();
			txt_off_2 = component.GameObjects[39].gameObject;
			txt_off_2Text = txt_off_2.GetComponent<Text>();
			btn_back_desc = component.GameObjects[40].gameObject;
			ViewMgr.Ins.addView(this);
		}
	}
}
