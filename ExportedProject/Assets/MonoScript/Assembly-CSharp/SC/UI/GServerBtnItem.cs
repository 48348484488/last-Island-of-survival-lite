using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GServerBtnItem : MonoBehaviour
	{
		public GameObject m_noselect;

		public GameObject m_select;

		public GameObject txt_name_off;

		public Text txt_name_offText;

		public GameObject txt_name_on;

		public Text txt_name_onText;

		public object context;

		private void Awake()
		{
			m_noselect = base.transform.Find("m_noselect").gameObject;
			m_select = base.transform.Find("m_select").gameObject;
			txt_name_off = base.transform.Find("m_noselect/txt_name_off").gameObject;
			txt_name_offText = txt_name_off.GetComponent<Text>();
			txt_name_on = base.transform.Find("m_select/txt_name_on").gameObject;
			txt_name_onText = txt_name_on.GetComponent<Text>();
		}
	}
}
