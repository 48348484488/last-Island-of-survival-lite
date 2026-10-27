using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GShortcutBarItem : MonoBehaviour
	{
		public GameObject m_bind;

		public GameObject m_durability;

		public GameObject m_equip_select;

		public GameObject m_gun_part;

		public GameObject[] m_gun_part_num_bgs;

		public GameObject m_gun_part_num_bgsObj;

		public GameObject[] m_gun_part_nums;

		public GameObject m_gun_part_numsObj;

		public GameObject m_gun_part_red;

		public GameObject m_select;

		public GameObject m_shortcut_bar_icon;

		public GameObject txt_bullet_num;

		public Text txt_bullet_numText;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			m_bind = base.transform.Find("m_bind").gameObject;
			m_durability = base.transform.Find("m_durability").gameObject;
			m_equip_select = base.transform.Find("m_shortcut_bar_icon/m_equip_select").gameObject;
			m_gun_part = base.transform.Find("m_gun_part").gameObject;
			m_gun_part_num_bgs = base.transform.Find("m_gun_part/m_gun_part_num_bgs").gameObject.GetComponent<UIGameObjectList>().objects;
			m_gun_part_num_bgsObj = base.transform.Find("m_gun_part/m_gun_part_num_bgs").gameObject;
			m_gun_part_nums = base.transform.Find("m_gun_part/m_gun_part_nums").gameObject.GetComponent<UIGameObjectList>().objects;
			m_gun_part_numsObj = base.transform.Find("m_gun_part/m_gun_part_nums").gameObject;
			m_gun_part_red = base.transform.Find("m_gun_part_red").gameObject;
			m_select = base.transform.Find("Background/m_select").gameObject;
			m_shortcut_bar_icon = base.transform.Find("m_shortcut_bar_icon").gameObject;
			txt_bullet_num = base.transform.Find("txt_bullet_num").gameObject;
			txt_bullet_numText = txt_bullet_num.GetComponent<Text>();
			txt_num = base.transform.Find("txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
