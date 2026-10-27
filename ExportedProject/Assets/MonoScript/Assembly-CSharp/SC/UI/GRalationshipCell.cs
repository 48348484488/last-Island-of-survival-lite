using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GRalationshipCell : MonoBehaviour
	{
		public GameObject btn_look_detailInfo;

		public GameObject btn_team;

		public GameObject m_care_eachother;

		public GameObject m_care_parent;

		public GameObject m_icon;

		public GameObject m_qinMiDu;

		public GameObject m_status;

		public GameObject txt_in_team;

		public Text txt_in_teamText;

		public GameObject txt_invity_online_stute_battle;

		public Text txt_invity_online_stute_battleText;

		public GameObject txt_invity_online_stute_match;

		public Text txt_invity_online_stute_matchText;

		public GameObject txt_qinMiDu_lv;

		public Text txt_qinMiDu_lvText;

		public GameObject txt_qinMiDu_value;

		public Text txt_qinMiDu_valueText;

		public GameObject txt_roleId;

		public Text txt_roleIdText;

		public GameObject txt_status;

		public Text txt_statusText;

		public GameObject txt_team;

		public Text txt_teamText;

		public object context;

		private void Awake()
		{
			btn_look_detailInfo = base.transform.Find("btn_look_detailInfo").gameObject;
			btn_team = base.transform.Find("btn_team").gameObject;
			m_care_eachother = base.transform.Find("m_care_parent/m_care_eachother").gameObject;
			m_care_parent = base.transform.Find("m_care_parent").gameObject;
			m_icon = base.transform.Find("Image (1)/head/2/m_icon").gameObject;
			m_qinMiDu = base.transform.Find("m_qinMiDu").gameObject;
			m_status = base.transform.Find("GameObject (1)/m_status").gameObject;
			txt_in_team = base.transform.Find("GameObject (1)/Image/txt_in_team").gameObject;
			txt_in_teamText = txt_in_team.GetComponent<Text>();
			txt_invity_online_stute_battle = base.transform.Find("GameObject (1)/Image/txt_invity_online_stute_battle").gameObject;
			txt_invity_online_stute_battleText = txt_invity_online_stute_battle.GetComponent<Text>();
			txt_invity_online_stute_match = base.transform.Find("GameObject (1)/Image/txt_invity_online_stute_match").gameObject;
			txt_invity_online_stute_matchText = txt_invity_online_stute_match.GetComponent<Text>();
			txt_qinMiDu_lv = base.transform.Find("m_qinMiDu/txt_qinMiDu_lv").gameObject;
			txt_qinMiDu_lvText = txt_qinMiDu_lv.GetComponent<Text>();
			txt_qinMiDu_value = base.transform.Find("m_qinMiDu/txt_qinMiDu_value").gameObject;
			txt_qinMiDu_valueText = txt_qinMiDu_value.GetComponent<Text>();
			txt_roleId = base.transform.Find("ID/txt_roleId").gameObject;
			txt_roleIdText = txt_roleId.GetComponent<Text>();
			txt_status = base.transform.Find("GameObject (1)/Image/txt_status").gameObject;
			txt_statusText = txt_status.GetComponent<Text>();
			txt_team = base.transform.Find("btn_team/txt_team").gameObject;
			txt_teamText = txt_team.GetComponent<Text>();
		}
	}
}
