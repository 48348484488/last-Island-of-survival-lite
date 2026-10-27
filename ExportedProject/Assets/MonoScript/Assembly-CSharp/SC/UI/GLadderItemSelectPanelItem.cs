using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GLadderItemSelectPanelItem : MonoBehaviour
	{
		public GameObject m_durability;

		public GameObject m_gun_part;

		public GameObject[] m_gun_part_num_bgs;

		public GameObject m_gun_part_num_bgsObj;

		public GameObject[] m_gun_part_nums;

		public GameObject m_gun_part_numsObj;

		public GameObject m_select;

		public GameObject m_shortcut_bar_icon;

		public GameObject txt_bullet_num;

		public Text txt_bullet_numText;

		public object context;

		private void Awake()
		{
			m_durability = base.transform.Find("shortcut_bar_item/m_durability").gameObject;
			m_gun_part = base.transform.Find("shortcut_bar_item/m_gun_part").gameObject;
			m_gun_part_num_bgs = base.transform.Find("shortcut_bar_item/m_gun_part/m_gun_part_num_bgs").gameObject.GetComponent<UIGameObjectList>().objects;
			m_gun_part_num_bgsObj = base.transform.Find("shortcut_bar_item/m_gun_part/m_gun_part_num_bgs").gameObject;
			m_gun_part_nums = base.transform.Find("shortcut_bar_item/m_gun_part/m_gun_part_nums").gameObject.GetComponent<UIGameObjectList>().objects;
			m_gun_part_numsObj = base.transform.Find("shortcut_bar_item/m_gun_part/m_gun_part_nums").gameObject;
			m_select = base.transform.Find("shortcut_bar_item/Background/m_select").gameObject;
			m_shortcut_bar_icon = base.transform.Find("shortcut_bar_item/m_shortcut_bar_icon").gameObject;
			txt_bullet_num = base.transform.Find("shortcut_bar_item/txt_bullet_num").gameObject;
			txt_bullet_numText = txt_bullet_num.GetComponent<Text>();
		}
	}
}
