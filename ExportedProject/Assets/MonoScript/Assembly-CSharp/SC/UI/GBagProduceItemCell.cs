using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GBagProduceItemCell : MonoBehaviour
	{
		public GameObject m_frame;

		public GameObject m_gray_ima;

		public GameObject m_icon;

		public GameObject m_locked;

		public GameObject m_novice_effect;

		public GameObject m_queuing;

		public GameObject m_red_dot;

		public GameObject m_select;

		public GameObject m_tag;

		public GameObject txt_new;

		public Text txt_newText;

		public GameObject txt_queuing;

		public Text txt_queuingText;

		public object context;

		private void Awake()
		{
			m_frame = base.transform.Find("m_frame").gameObject;
			m_gray_ima = base.transform.Find("m_gray_ima").gameObject;
			m_icon = base.transform.Find("m_icon").gameObject;
			m_locked = base.transform.Find("m_locked").gameObject;
			m_novice_effect = base.transform.Find("m_novice_effect").gameObject;
			m_queuing = base.transform.Find("m_queuing").gameObject;
			m_red_dot = base.transform.Find("m_red_dot").gameObject;
			m_select = base.transform.Find("m_select").gameObject;
			m_tag = base.transform.Find("m_tag").gameObject;
			txt_new = base.transform.Find("m_tag/txt_new").gameObject;
			txt_newText = txt_new.GetComponent<Text>();
			txt_queuing = base.transform.Find("m_queuing/txt_queuing").gameObject;
			txt_queuingText = txt_queuing.GetComponent<Text>();
		}
	}
}
