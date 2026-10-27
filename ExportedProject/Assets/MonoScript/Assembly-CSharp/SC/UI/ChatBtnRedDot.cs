using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class ChatBtnRedDot : MonoBehaviour
	{
		public GameObject m_red_dot;

		public GameObject txt_off;

		public Text txt_offText;

		public GameObject txt_on;

		public Text txt_onText;

		public GameObject txt_red_dot;

		public Text txt_red_dotText;

		public object context;

		private void Awake()
		{
			m_red_dot = base.transform.Find("m_red_dot").gameObject;
			txt_off = base.transform.Find("Off/txt_off").gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			txt_on = base.transform.Find("On/txt_on").gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_red_dot = base.transform.Find("m_red_dot/txt_red_dot").gameObject;
			txt_red_dotText = txt_red_dot.GetComponent<Text>();
		}
	}
}
