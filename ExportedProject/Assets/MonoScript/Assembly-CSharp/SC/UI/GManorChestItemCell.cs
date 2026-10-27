using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GManorChestItemCell : MonoBehaviour
	{
		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject m_something;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			m_frame = base.transform.Find("m_something/m_frame").gameObject;
			m_icon = base.transform.Find("m_something/m_icon").gameObject;
			m_something = base.transform.Find("m_something").gameObject;
			txt_num = base.transform.Find("m_something/txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
