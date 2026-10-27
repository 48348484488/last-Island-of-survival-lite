using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GActivityMonthSignInCell : MonoBehaviour
	{
		public GameObject m_already_got;

		public GameObject m_bind;

		public GameObject m_buQian;

		public GameObject m_icon;

		public GameObject m_sign_in;

		public GameObject txt_day;

		public Text txt_dayText;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			m_already_got = base.transform.Find("m_already_got").gameObject;
			m_bind = base.transform.Find("m_bind").gameObject;
			m_buQian = base.transform.Find("m_buQian").gameObject;
			m_icon = base.transform.Find("m_icon").gameObject;
			m_sign_in = base.transform.Find("m_sign_in").gameObject;
			txt_day = base.transform.Find("txt_day").gameObject;
			txt_dayText = txt_day.GetComponent<Text>();
			txt_num = base.transform.Find("txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
