using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GServerItem : MonoBehaviour
	{
		public GameObject m_no_select;

		public GameObject m_select;

		public GameObject m_state_1;

		public GameObject m_state_2;

		public GameObject m_state_3;

		public GameObject m_state_4;

		public GameObject txt_server_name;

		public Text txt_server_nameText;

		public GameObject txt_server_name_no_select;

		public Text txt_server_name_no_selectText;

		public GameObject txt_state;

		public Text txt_stateText;

		public GameObject txt_state_0;

		public Text txt_state_0Text;

		public GameObject txt_state_1;

		public Text txt_state_1Text;

		public GameObject txt_state_2;

		public Text txt_state_2Text;

		public object context;

		private void Awake()
		{
			m_no_select = base.transform.Find("m_no_select").gameObject;
			m_select = base.transform.Find("m_select").gameObject;
			m_state_1 = base.transform.Find("state/m_state_1").gameObject;
			m_state_2 = base.transform.Find("state/m_state_2").gameObject;
			m_state_3 = base.transform.Find("state/m_state_3").gameObject;
			m_state_4 = base.transform.Find("state/m_state_4").gameObject;
			txt_server_name = base.transform.Find("m_select/txt_server_name").gameObject;
			txt_server_nameText = txt_server_name.GetComponent<Text>();
			txt_server_name_no_select = base.transform.Find("m_no_select/txt_server_name_no_select").gameObject;
			txt_server_name_no_selectText = txt_server_name_no_select.GetComponent<Text>();
			txt_state = base.transform.Find("state/m_state_4/txt_state").gameObject;
			txt_stateText = txt_state.GetComponent<Text>();
			txt_state_0 = base.transform.Find("state/m_state_3/txt_state").gameObject;
			txt_state_0Text = txt_state_0.GetComponent<Text>();
			txt_state_1 = base.transform.Find("state/m_state_1/txt_state").gameObject;
			txt_state_1Text = txt_state_1.GetComponent<Text>();
			txt_state_2 = base.transform.Find("state/m_state_2/txt_state").gameObject;
			txt_state_2Text = txt_state_2.GetComponent<Text>();
		}
	}
}
