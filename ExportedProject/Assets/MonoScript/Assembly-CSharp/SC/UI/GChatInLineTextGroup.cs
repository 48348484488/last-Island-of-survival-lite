using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GChatInLineTextGroup : MonoBehaviour
	{
		public GameObject m_emoji;

		public GameObject txt_msg;

		public Text txt_msgText;

		public object context;

		private void Awake()
		{
			m_emoji = base.transform.Find("m_emoji").gameObject;
			txt_msg = base.transform.Find("txt_msg").gameObject;
			txt_msgText = txt_msg.GetComponent<Text>();
		}
	}
}
