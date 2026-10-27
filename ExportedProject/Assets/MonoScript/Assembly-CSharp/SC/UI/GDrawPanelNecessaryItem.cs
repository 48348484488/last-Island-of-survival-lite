using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GDrawPanelNecessaryItem : MonoBehaviour
	{
		public GameObject btn_add;

		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			btn_add = base.transform.Find("btn_add").gameObject;
			m_frame = base.transform.Find("m_frame").gameObject;
			m_icon = base.transform.Find("m_icon").gameObject;
			txt_name = base.transform.Find("txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_num = base.transform.Find("txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
