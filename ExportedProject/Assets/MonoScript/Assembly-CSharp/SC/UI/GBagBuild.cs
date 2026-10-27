using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GBagBuild : MonoBehaviour
	{
		public GameObject m_icon;

		public GameObject m_newbee_effect;

		public GameObject txt_name;

		public Text txt_nameText;

		public object context;

		private void Awake()
		{
			m_icon = base.transform.Find("m_icon").gameObject;
			m_newbee_effect = base.transform.Find("m_newbee_effect").gameObject;
			txt_name = base.transform.Find("txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
		}
	}
}
