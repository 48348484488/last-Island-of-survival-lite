using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GInforSelfPanelItem : MonoBehaviour
	{
		public GameObject m_has_bg;

		public GameObject m_lock;

		public GameObject m_lock_icon;

		public GameObject m_new;

		public GameObject m_nohas_bg;

		public GameObject m_unlcok_item;

		public GameObject m_unlock_icon;

		public GameObject txt_lock_name;

		public Text txt_lock_nameText;

		public GameObject txt_unlock_name;

		public Text txt_unlock_nameText;

		public object context;

		private void Awake()
		{
			m_has_bg = base.transform.Find("m_has_bg").gameObject;
			m_lock = base.transform.Find("m_lock").gameObject;
			m_lock_icon = base.transform.Find("m_lock/m_lock_icon").gameObject;
			m_new = base.transform.Find("m_unlcok_item/m_new").gameObject;
			m_nohas_bg = base.transform.Find("m_nohas_bg").gameObject;
			m_unlcok_item = base.transform.Find("m_unlcok_item").gameObject;
			m_unlock_icon = base.transform.Find("m_unlcok_item/m_unlock_icon").gameObject;
			txt_lock_name = base.transform.Find("m_lock/txt_lock_name").gameObject;
			txt_lock_nameText = txt_lock_name.GetComponent<Text>();
			txt_unlock_name = base.transform.Find("m_unlcok_item/txt_unlock_name").gameObject;
			txt_unlock_nameText = txt_unlock_name.GetComponent<Text>();
		}
	}
}
