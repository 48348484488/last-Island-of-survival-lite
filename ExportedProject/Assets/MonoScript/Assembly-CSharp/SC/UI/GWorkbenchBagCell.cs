using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GWorkbenchBagCell : MonoBehaviour
	{
		public GameObject m_durability;

		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject m_new;

		public GameObject m_nothing;

		public GameObject m_select_material;

		public GameObject m_select_pointer;

		public GameObject m_something;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			m_durability = base.transform.Find("m_something/m_durability").gameObject;
			m_frame = base.transform.Find("m_something/m_frame").gameObject;
			m_icon = base.transform.Find("m_something/m_icon").gameObject;
			m_new = base.transform.Find("m_something/m_new").gameObject;
			m_nothing = base.transform.Find("m_nothing").gameObject;
			m_select_material = base.transform.Find("m_something/m_select_material").gameObject;
			m_select_pointer = base.transform.Find("m_something/m_select_pointer").gameObject;
			m_something = base.transform.Find("m_something").gameObject;
			txt_num = base.transform.Find("m_something/txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
