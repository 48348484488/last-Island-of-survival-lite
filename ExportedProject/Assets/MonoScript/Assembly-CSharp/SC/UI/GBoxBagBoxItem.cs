using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GBoxBagBoxItem : MonoBehaviour
	{
		public GameObject m_durability;

		public GameObject m_frame;

		public GameObject m_has;

		public GameObject m_icon;

		public GameObject m_no_has;

		public GameObject m_tag;

		public GameObject txt_new;

		public Text txt_newText;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			m_durability = base.transform.Find("m_has/m_durability").gameObject;
			m_frame = base.transform.Find("m_has/m_frame").gameObject;
			m_has = base.transform.Find("m_has").gameObject;
			m_icon = base.transform.Find("m_has/m_icon").gameObject;
			m_no_has = base.transform.Find("m_no_has").gameObject;
			m_tag = base.transform.Find("m_has/m_tag").gameObject;
			txt_new = base.transform.Find("m_has/m_tag/txt_new").gameObject;
			txt_newText = txt_new.GetComponent<Text>();
			txt_num = base.transform.Find("m_has/txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
