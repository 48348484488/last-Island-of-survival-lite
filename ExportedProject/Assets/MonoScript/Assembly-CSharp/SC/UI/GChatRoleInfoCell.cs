using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GChatRoleInfoCell : MonoBehaviour
	{
		public GameObject m_female;

		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject m_male;

		public GameObject m_noselect_bg;

		public GameObject m_red_dot;

		public GameObject txt_invity_online_stute_battle;

		public Text txt_invity_online_stute_battleText;

		public GameObject txt_invity_online_stute_match;

		public Text txt_invity_online_stute_matchText;

		public GameObject txt_level;

		public Text txt_levelText;

		public GameObject txt_lixian;

		public Text txt_lixianText;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_new_msg_num;

		public Text txt_new_msg_numText;

		public GameObject txt_status;

		public Text txt_statusText;

		public object context;

		private void Awake()
		{
			m_female = base.transform.Find("xingbie/m_female").gameObject;
			m_frame = base.transform.Find("head/m_frame").gameObject;
			m_icon = base.transform.Find("head/m_icon").gameObject;
			m_male = base.transform.Find("xingbie/m_male").gameObject;
			m_noselect_bg = base.transform.Find("m_noselect_bg").gameObject;
			m_red_dot = base.transform.Find("m_red_dot").gameObject;
			txt_invity_online_stute_battle = base.transform.Find("Status/txt_invity_online_stute_battle").gameObject;
			txt_invity_online_stute_battleText = txt_invity_online_stute_battle.GetComponent<Text>();
			txt_invity_online_stute_match = base.transform.Find("Status/txt_invity_online_stute_match").gameObject;
			txt_invity_online_stute_matchText = txt_invity_online_stute_match.GetComponent<Text>();
			txt_level = base.transform.Find("head/Image (3)/txt_level").gameObject;
			txt_levelText = txt_level.GetComponent<Text>();
			txt_lixian = base.transform.Find("Status/txt_lixian").gameObject;
			txt_lixianText = txt_lixian.GetComponent<Text>();
			txt_name = base.transform.Find("txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_new_msg_num = base.transform.Find("m_red_dot/txt_new_msg_num").gameObject;
			txt_new_msg_numText = txt_new_msg_num.GetComponent<Text>();
			txt_status = base.transform.Find("Status/txt_status").gameObject;
			txt_statusText = txt_status.GetComponent<Text>();
		}
	}
}
