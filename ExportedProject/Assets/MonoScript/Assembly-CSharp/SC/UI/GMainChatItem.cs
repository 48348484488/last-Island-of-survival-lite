using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GMainChatItem : MonoBehaviour
	{
		public GameObject m_emoji;

		public GameObject m_InlinText;

		public GameObject m_other_frame;

		public GameObject m_other_icon;

		public GameObject txt_other_name;

		public Text txt_other_nameText;

		public object context;

		private void Awake()
		{
			m_emoji = base.transform.Find("GameObject/m_InlinText/m_emoji").gameObject;
			m_InlinText = base.transform.Find("GameObject/m_InlinText").gameObject;
			m_other_frame = base.transform.Find("GameObject/head/m_other_frame").gameObject;
			m_other_icon = base.transform.Find("GameObject/head/m_other_icon").gameObject;
			txt_other_name = base.transform.Find("GameObject/m_InlinText/txt_other_name").gameObject;
			txt_other_nameText = txt_other_name.GetComponent<Text>();
		}
	}
}
