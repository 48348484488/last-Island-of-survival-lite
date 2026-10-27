using UnityEngine;

namespace SC.UI
{
	public class GBagProduceShortcutCell : MonoBehaviour
	{
		public GameObject m_shortcut_bar_icon;

		public object context;

		private void Awake()
		{
			m_shortcut_bar_icon = base.transform.Find("m_shortcut_bar_icon").gameObject;
		}
	}
}
