using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GBigHornInfo : MonoBehaviour
	{
		public GameObject m_bg_img;

		public GameObject m_big_horn;

		public GameObject m_bigHorn_duanwei;

		public GameObject m_emoji_horn;

		public GameObject m_player_frame;

		public GameObject m_player_icon;

		public GameObject m_player_sex_female;

		public GameObject m_player_sex_male;

		public GameObject m_vip;

		public GameObject txt_horn_msg;

		public Text txt_horn_msgText;

		public GameObject txt_player_name;

		public Text txt_player_nameText;

		public object context;

		private void Awake()
		{
			m_bg_img = base.transform.Find("m_bg_img").gameObject;
			m_big_horn = base.transform.Find("GameObject/m_big_horn").gameObject;
			m_bigHorn_duanwei = base.transform.Find("GameObject/m_bigHorn_duanwei").gameObject;
			m_emoji_horn = base.transform.Find("GameObject/Image/m_emoji_horn").gameObject;
			m_player_frame = base.transform.Find("GameObject/head/m_player_frame").gameObject;
			m_player_icon = base.transform.Find("GameObject/head/m_player_icon").gameObject;
			m_player_sex_female = base.transform.Find("GameObject/m_player_sex_female").gameObject;
			m_player_sex_male = base.transform.Find("GameObject/m_player_sex_male").gameObject;
			m_vip = base.transform.Find("GameObject/head/vip/m_vip").gameObject;
			txt_horn_msg = base.transform.Find("GameObject/Image/txt_horn_msg").gameObject;
			txt_horn_msgText = txt_horn_msg.GetComponent<Text>();
			txt_player_name = base.transform.Find("GameObject/txt_player_name").gameObject;
			txt_player_nameText = txt_player_name.GetComponent<Text>();
		}
	}
}
