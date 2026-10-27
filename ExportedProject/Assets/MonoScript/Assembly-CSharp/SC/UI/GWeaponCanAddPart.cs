using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GWeaponCanAddPart : MonoBehaviour
	{
		public GameObject m_duration;

		public GameObject m_icon;

		public GameObject m_part;

		public GameObject m_select;

		public GameObject m_unload;

		public GameObject txt_name;

		public Text txt_nameText;

		public object context;

		private void Awake()
		{
			m_duration = base.transform.Find("m_part/m_duration").gameObject;
			m_icon = base.transform.Find("m_part/m_icon").gameObject;
			m_part = base.transform.Find("m_part").gameObject;
			m_select = base.transform.Find("m_part/m_select").gameObject;
			m_unload = base.transform.Find("m_unload").gameObject;
			txt_name = base.transform.Find("m_part/Image/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
		}
	}
}
