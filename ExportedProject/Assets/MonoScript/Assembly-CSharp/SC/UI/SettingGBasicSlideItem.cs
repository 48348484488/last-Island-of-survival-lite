using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class SettingGBasicSlideItem : MonoBehaviour
	{
		public GameObject btn_down;

		public GameObject btn_up;

		public GameObject m_slide;

		public GameObject txt_value;

		public Text txt_valueText;

		public object context;

		private void Awake()
		{
			btn_down = base.transform.Find("btn_down").gameObject;
			btn_up = base.transform.Find("btn_up").gameObject;
			m_slide = base.transform.Find("m_slide").gameObject;
			txt_value = base.transform.Find("value/txt_value").gameObject;
			txt_valueText = txt_value.GetComponent<Text>();
		}
	}
}
