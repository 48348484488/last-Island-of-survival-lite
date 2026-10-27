using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GWorkbenchRepairCell : MonoBehaviour
	{
		public GameObject m_durability;

		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject m_item;

		public GameObject txt_duration;

		public Text txt_durationText;

		public GameObject txt_duration_value;

		public Text txt_duration_valueText;

		public GameObject txt_name;

		public Text txt_nameText;

		public object context;

		private void Awake()
		{
			m_durability = base.transform.Find("m_item/m_durability").gameObject;
			m_frame = base.transform.Find("m_frame").gameObject;
			m_icon = base.transform.Find("m_item/m_icon").gameObject;
			m_item = base.transform.Find("m_item").gameObject;
			txt_duration = base.transform.Find("m_item/txt_duration").gameObject;
			txt_durationText = txt_duration.GetComponent<Text>();
			txt_duration_value = base.transform.Find("m_item/txt_duration/txt_duration_value").gameObject;
			txt_duration_valueText = txt_duration_value.GetComponent<Text>();
			txt_name = base.transform.Find("m_item/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
		}
	}
}
