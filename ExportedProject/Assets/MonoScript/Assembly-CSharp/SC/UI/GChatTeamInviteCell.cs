using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GChatTeamInviteCell : MonoBehaviour
	{
		public GameObject btn_apply_to_troop;

		public GameObject m_apply_to_troop;

		public GameObject m_other;

		public GameObject m_other_female;

		public GameObject m_other_frame;

		public GameObject m_other_grade;

		public GameObject m_other_icon;

		public GameObject m_other_male;

		public GameObject m_vip;

		public GameObject m_vip_head;

		public GameObject txt_apply;

		public Text txt_applyText;

		public GameObject txt_other_msg;

		public Text txt_other_msgText;

		public GameObject txt_other_name;

		public Text txt_other_nameText;

		public object context;

		private void Awake()
		{
			btn_apply_to_troop = base.transform.Find("m_other/Image/GameObject/btn_apply_to_troop").gameObject;
			m_apply_to_troop = base.transform.Find("m_other/Image/GameObject/m_apply_to_troop").gameObject;
			m_other = base.transform.Find("m_other").gameObject;
			m_other_female = base.transform.Find("m_other/m_other_female").gameObject;
			m_other_frame = base.transform.Find("m_other/head/m_other_frame").gameObject;
			m_other_grade = base.transform.Find("m_other/GameObject/m_other_grade").gameObject;
			m_other_icon = base.transform.Find("m_other/head/m_other_icon").gameObject;
			m_other_male = base.transform.Find("m_other/m_other_male").gameObject;
			m_vip = base.transform.Find("m_other/GameObject/m_vip_head/m_vip").gameObject;
			m_vip_head = base.transform.Find("m_other/GameObject/m_vip_head").gameObject;
			txt_apply = base.transform.Find("m_other/Image/GameObject/btn_apply_to_troop/txt_apply").gameObject;
			txt_applyText = txt_apply.GetComponent<Text>();
			txt_other_msg = base.transform.Find("m_other/Image/txt_other_msg").gameObject;
			txt_other_msgText = txt_other_msg.GetComponent<Text>();
			txt_other_name = base.transform.Find("m_other/GameObject/txt_other_name").gameObject;
			txt_other_nameText = txt_other_name.GetComponent<Text>();
		}
	}
}
