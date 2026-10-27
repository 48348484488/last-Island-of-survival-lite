using UnityEngine;

namespace SC.UI
{
	public class GDeathBirthPosItem : MonoBehaviour
	{
		public GameObject m_bed;

		public GameObject m_bed_cding;

		public GameObject m_bed_select;

		public GameObject m_sleep_bed;

		public GameObject m_sleep_cding;

		public GameObject m_sleep_select;

		public object context;

		private void Awake()
		{
			m_bed = base.transform.Find("m_bed").gameObject;
			m_bed_cding = base.transform.Find("m_bed_cding").gameObject;
			m_bed_select = base.transform.Find("m_bed_select").gameObject;
			m_sleep_bed = base.transform.Find("m_sleep_bed").gameObject;
			m_sleep_cding = base.transform.Find("m_sleep_cding").gameObject;
			m_sleep_select = base.transform.Find("m_sleep_select").gameObject;
		}
	}
}
