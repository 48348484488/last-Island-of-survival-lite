using UnityEngine;

namespace SC.UI
{
	public class GChatEmojiCell : MonoBehaviour
	{
		public GameObject m_emoji;

		public object context;

		private void Awake()
		{
			m_emoji = base.transform.Find("m_emoji").gameObject;
		}
	}
}
