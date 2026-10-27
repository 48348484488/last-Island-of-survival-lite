using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GLanguageItem : MonoBehaviour
	{
		public GameObject m_select;

		public GameObject txt_language_name;

		public Text txt_language_nameText;

		public object context;

		private void Awake()
		{
			m_select = base.transform.Find("m_select").gameObject;
			txt_language_name = base.transform.Find("txt_language_name").gameObject;
			txt_language_nameText = txt_language_name.GetComponent<Text>();
		}
	}
}
