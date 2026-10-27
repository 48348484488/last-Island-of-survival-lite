using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GDeathPanelKilledPlayer : MonoBehaviour
	{
		public GameObject btn_care;

		public GameObject btn_report;

		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject txt_level;

		public Text txt_levelText;

		public GameObject txt_name;

		public Text txt_nameText;

		public object context;

		private void Awake()
		{
			btn_care = base.transform.Find("btn_care").gameObject;
			btn_report = base.transform.Find("btn_report").gameObject;
			m_frame = base.transform.Find("Image/head (1)/m_frame").gameObject;
			m_icon = base.transform.Find("Image/head (1)/m_icon").gameObject;
			txt_level = base.transform.Find("Image/txt_level").gameObject;
			txt_levelText = txt_level.GetComponent<Text>();
			txt_name = base.transform.Find("Image/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
		}
	}
}
