using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class MailListCell : MonoBehaviour
	{
		public GameObject m_isOpen;

		public GameObject m_isSelect;

		public GameObject m_notOpen;

		public GameObject m_red;

		public GameObject m_weidu;

		public GameObject m_yidu;

		public GameObject txt_from;

		public Text txt_fromText;

		public GameObject txt_mailName;

		public Text txt_mailNameText;

		public GameObject txt_timeHour;

		public Text txt_timeHourText;

		public object context;

		private void Awake()
		{
			m_isOpen = base.transform.Find("m_yidu/m_isOpen").gameObject;
			m_isSelect = base.transform.Find("m_isSelect").gameObject;
			m_notOpen = base.transform.Find("m_weidu/m_notOpen").gameObject;
			m_red = base.transform.Find("m_weidu/m_red").gameObject;
			m_weidu = base.transform.Find("m_weidu").gameObject;
			m_yidu = base.transform.Find("m_yidu").gameObject;
			txt_from = base.transform.Find("txt_from").gameObject;
			txt_fromText = txt_from.GetComponent<Text>();
			txt_mailName = base.transform.Find("txt_mailName").gameObject;
			txt_mailNameText = txt_mailName.GetComponent<Text>();
			txt_timeHour = base.transform.Find("txt_timeHour").gameObject;
			txt_timeHourText = txt_timeHour.GetComponent<Text>();
		}
	}
}
