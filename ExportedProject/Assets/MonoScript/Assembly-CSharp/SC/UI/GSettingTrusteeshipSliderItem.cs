using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GSettingTrusteeshipSliderItem : MonoBehaviour
	{
		public GameObject btn_down;

		public GameObject btn_up;

		public GameObject m_slide;

		public GameObject txt_desc;

		public Text txt_descText;

		public object context;

		private void Awake()
		{
			btn_down = base.transform.Find("btn_down").gameObject;
			btn_up = base.transform.Find("btn_up").gameObject;
			m_slide = base.transform.Find("m_slide").gameObject;
			txt_desc = base.transform.Find("GameObject/txt_desc").gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
		}
	}
}
