using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GExtraBtns : MonoBehaviour
	{
		public GameObject m_newbee_effect;

		public GameObject txt_btnFonts;

		public Text txt_btnFontsText;

		public object context;

		private void Awake()
		{
			m_newbee_effect = base.transform.Find("m_newbee_effect").gameObject;
			txt_btnFonts = base.transform.Find("txt_btnFonts").gameObject;
			txt_btnFontsText = txt_btnFonts.GetComponent<Text>();
		}
	}
}
