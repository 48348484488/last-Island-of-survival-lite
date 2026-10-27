using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GElectricElementCell : MonoBehaviour
	{
		public GameObject btn_use;

		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject txt_desc;

		public Text txt_descText;

		public GameObject txt_gonglv;

		public Text txt_gonglvText;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_used;

		public Text txt_usedText;

		public object context;

		private void Awake()
		{
			btn_use = base.transform.Find("btn_use").gameObject;
			m_frame = base.transform.Find("m_frame").gameObject;
			m_icon = base.transform.Find("m_icon").gameObject;
			txt_desc = base.transform.Find("txt_desc").gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
			txt_gonglv = base.transform.Find("Text/txt_gonglv").gameObject;
			txt_gonglvText = txt_gonglv.GetComponent<Text>();
			txt_name = base.transform.Find("txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_used = base.transform.Find("txt_used").gameObject;
			txt_usedText = txt_used.GetComponent<Text>();
		}
	}
}
