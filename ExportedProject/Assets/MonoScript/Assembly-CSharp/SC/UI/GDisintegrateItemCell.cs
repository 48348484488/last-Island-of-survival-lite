using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GDisintegrateItemCell : MonoBehaviour
	{
		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject m_new;

		public GameObject m_nothing;

		public GameObject m_select;

		public GameObject m_something;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			m_frame = base.transform.Find("m_something/m_frame").gameObject;
			m_icon = base.transform.Find("m_something/m_icon").gameObject;
			m_new = base.transform.Find("m_something/m_new").gameObject;
			m_nothing = base.transform.Find("m_nothing").gameObject;
			m_select = base.transform.Find("m_something/m_select").gameObject;
			m_something = base.transform.Find("m_something").gameObject;
			txt_num = base.transform.Find("m_something/txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
