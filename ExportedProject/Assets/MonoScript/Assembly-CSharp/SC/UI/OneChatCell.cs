using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class OneChatCell : MonoBehaviour
	{
		public GameObject m_big_horn_other;

		public GameObject m_big_horn_self;

		public GChatInLineTextGroup m_dlb_other;

		public GChatInLineTextGroup m_dlb_self;

		public GameObject m_other;

		public GameObject m_other_female;

		public GameObject m_other_frame;

		public GameObject m_other_grade;

		public GameObject m_other_icon;

		public GameObject m_other_male;

		public GChatInLineTextGroup m_other_mgr;

		public GameObject m_self;

		public GameObject m_self_female;

		public GameObject m_self_frame;

		public GameObject m_self_grade;

		public GameObject m_self_icon;

		public GameObject m_self_male;

		public GChatInLineTextGroup m_self_mgr;

		public GameObject m_small_horn_other;

		public GameObject m_small_horn_self;

		public GChatInLineTextGroup m_xlb_other;

		public GChatInLineTextGroup m_xlb_self;

		public GameObject txt_other_name;

		public Text txt_other_nameText;

		public GameObject txt_self_name;

		public Text txt_self_nameText;

		public object context;

		private void Awake()
		{
			m_big_horn_other = base.transform.Find("m_other/m_big_horn_other").gameObject;
			m_big_horn_self = base.transform.Find("m_self/m_big_horn_self").gameObject;
			m_dlb_other = View.AddComponentIfNotExist<GChatInLineTextGroup>(base.transform.Find("m_other/m_dlb_other").gameObject);
			m_dlb_self = View.AddComponentIfNotExist<GChatInLineTextGroup>(base.transform.Find("m_self/m_dlb_self").gameObject);
			m_other = base.transform.Find("m_other").gameObject;
			m_other_female = base.transform.Find("m_other/m_other_female").gameObject;
			m_other_frame = base.transform.Find("m_other/head/m_other_frame").gameObject;
			m_other_grade = base.transform.Find("m_other/GameObject/m_other_grade").gameObject;
			m_other_icon = base.transform.Find("m_other/head/m_other_icon").gameObject;
			m_other_male = base.transform.Find("m_other/m_other_male").gameObject;
			m_other_mgr = View.AddComponentIfNotExist<GChatInLineTextGroup>(base.transform.Find("m_other/m_other_mgr").gameObject);
			m_self = base.transform.Find("m_self").gameObject;
			m_self_female = base.transform.Find("m_self/m_self_female").gameObject;
			m_self_frame = base.transform.Find("m_self/head/m_self_frame").gameObject;
			m_self_grade = base.transform.Find("m_self/GameObject/m_self_grade").gameObject;
			m_self_icon = base.transform.Find("m_self/head/m_self_icon").gameObject;
			m_self_male = base.transform.Find("m_self/m_self_male").gameObject;
			m_self_mgr = View.AddComponentIfNotExist<GChatInLineTextGroup>(base.transform.Find("m_self/m_self_mgr").gameObject);
			m_small_horn_other = base.transform.Find("m_other/m_small_horn_other").gameObject;
			m_small_horn_self = base.transform.Find("m_self/m_small_horn_self").gameObject;
			m_xlb_other = View.AddComponentIfNotExist<GChatInLineTextGroup>(base.transform.Find("m_other/m_xlb_other").gameObject);
			m_xlb_self = View.AddComponentIfNotExist<GChatInLineTextGroup>(base.transform.Find("m_self/m_xlb_self").gameObject);
			txt_other_name = base.transform.Find("m_other/GameObject/txt_other_name").gameObject;
			txt_other_nameText = txt_other_name.GetComponent<Text>();
			txt_self_name = base.transform.Find("m_self/GameObject/txt_self_name").gameObject;
			txt_self_nameText = txt_self_name.GetComponent<Text>();
		}
	}
}
