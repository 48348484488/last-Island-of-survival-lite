using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GInforPanelUnlockItem : MonoBehaviour
	{
		public GameObject m_lock_icon;

		public GameObject m_lock_item;

		public GameObject m_unlock_item;

		public GameObject txt_lock_name;

		public Text txt_lock_nameText;

		public object context;

		private void Awake()
		{
			m_lock_icon = base.transform.Find("m_unlock_item/m_lock_icon").gameObject;
			m_lock_item = base.transform.Find("m_lock_item").gameObject;
			m_unlock_item = base.transform.Find("m_unlock_item").gameObject;
			txt_lock_name = base.transform.Find("m_unlock_item/txt_lock_name").gameObject;
			txt_lock_nameText = txt_lock_name.GetComponent<Text>();
		}
	}
}
