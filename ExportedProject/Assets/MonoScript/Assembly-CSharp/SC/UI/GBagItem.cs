using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GBagItem : MonoBehaviour
	{
		public GameObject m_binding;

		public GameObject m_durability;

		public GameObject m_equip_select;

		public GameObject m_frame;

		public GameObject m_gun_part;

		public GameObject[] m_gun_part_num_bgs;

		public GameObject m_gun_part_num_bgsObj;

		public GameObject[] m_gun_part_nums;

		public GameObject m_gun_part_numsObj;

		public GameObject m_has;

		public GameObject m_icon;

		public GameObject m_new;

		public GameObject m_no_has;

		public GameObject m_novice_effect;

		public GameObject m_select;

		public GameObject txt_bullet_num;

		public Text txt_bullet_numText;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			m_binding = base.transform.Find("m_has/m_binding").gameObject;
			m_durability = base.transform.Find("m_has/m_durability").gameObject;
			m_equip_select = base.transform.Find("m_has/m_equip_select").gameObject;
			m_frame = base.transform.Find("m_has/m_frame").gameObject;
			m_gun_part = base.transform.Find("m_has/m_gun_part").gameObject;
			m_gun_part_num_bgs = base.transform.Find("m_has/m_gun_part/m_gun_part_num_bgs").gameObject.GetComponent<UIGameObjectList>().objects;
			m_gun_part_num_bgsObj = base.transform.Find("m_has/m_gun_part/m_gun_part_num_bgs").gameObject;
			m_gun_part_nums = base.transform.Find("m_has/m_gun_part/m_gun_part_nums").gameObject.GetComponent<UIGameObjectList>().objects;
			m_gun_part_numsObj = base.transform.Find("m_has/m_gun_part/m_gun_part_nums").gameObject;
			m_has = base.transform.Find("m_has").gameObject;
			m_icon = base.transform.Find("m_has/m_icon").gameObject;
			m_new = base.transform.Find("m_has/m_new").gameObject;
			m_no_has = base.transform.Find("m_no_has").gameObject;
			m_novice_effect = base.transform.Find("m_has/m_novice_effect").gameObject;
			m_select = base.transform.Find("m_has/m_select").gameObject;
			txt_bullet_num = base.transform.Find("m_has/txt_bullet_num").gameObject;
			txt_bullet_numText = txt_bullet_num.GetComponent<Text>();
			txt_num = base.transform.Find("m_has/txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
