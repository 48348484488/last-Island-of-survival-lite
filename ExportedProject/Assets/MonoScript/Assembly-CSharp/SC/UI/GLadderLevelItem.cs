using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GLadderLevelItem : MonoBehaviour
	{
		public GameObject m_already_get;

		public GameObject m_bind;

		public GameObject m_icon;

		public GameObject m_quality;

		public GameObject m_red;

		public GameObject m_select;

		public GameObject m_suo;

		public GameObject txt_name;

		public Text txt_nameText;

		public object context;

		private void Awake()
		{
			m_already_get = base.transform.Find("m_already_get").gameObject;
			m_bind = base.transform.Find("GameObject/m_bind").gameObject;
			m_icon = base.transform.Find("GameObject/m_icon").gameObject;
			m_quality = base.transform.Find("GameObject/m_quality").gameObject;
			m_red = base.transform.Find("m_red").gameObject;
			m_select = base.transform.Find("GameObject/m_select").gameObject;
			m_suo = base.transform.Find("m_suo").gameObject;
			txt_name = base.transform.Find("GameObject/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
		}
	}
}
