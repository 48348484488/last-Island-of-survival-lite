using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GTeamInviteCell : MonoBehaviour
	{
		public GameObject btn_invite;

		public GameObject m_female;

		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject m_male;

		public GameObject m_status;

		public GameObject txt_in_team;

		public Text txt_in_teamText;

		public GameObject txt_invite;

		public Text txt_inviteText;

		public GameObject txt_invity_online_stute_battle;

		public Text txt_invity_online_stute_battleText;

		public GameObject txt_invity_online_stute_match;

		public Text txt_invity_online_stute_matchText;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_roleId;

		public Text txt_roleIdText;

		public GameObject txt_status;

		public Text txt_statusText;

		public object context;

		private void Awake()
		{
			btn_invite = base.transform.Find("btn_invite").gameObject;
			m_female = base.transform.Find("GameObject/m_female").gameObject;
			m_frame = base.transform.Find("Image/head/m_frame").gameObject;
			m_icon = base.transform.Find("Image/head/m_icon").gameObject;
			m_male = base.transform.Find("GameObject/m_male").gameObject;
			m_status = base.transform.Find("GameObject (1)/m_status").gameObject;
			txt_in_team = base.transform.Find("GameObject (1)/Image/txt_in_team").gameObject;
			txt_in_teamText = txt_in_team.GetComponent<Text>();
			txt_invite = base.transform.Find("btn_invite/txt_invite").gameObject;
			txt_inviteText = txt_invite.GetComponent<Text>();
			txt_invity_online_stute_battle = base.transform.Find("GameObject (1)/Image/txt_invity_online_stute_battle").gameObject;
			txt_invity_online_stute_battleText = txt_invity_online_stute_battle.GetComponent<Text>();
			txt_invity_online_stute_match = base.transform.Find("GameObject (1)/Image/txt_invity_online_stute_match").gameObject;
			txt_invity_online_stute_matchText = txt_invity_online_stute_match.GetComponent<Text>();
			txt_name = base.transform.Find("txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_roleId = base.transform.Find("ID/txt_roleId").gameObject;
			txt_roleIdText = txt_roleId.GetComponent<Text>();
			txt_status = base.transform.Find("GameObject (1)/Image/txt_status").gameObject;
			txt_statusText = txt_status.GetComponent<Text>();
		}
	}
}
