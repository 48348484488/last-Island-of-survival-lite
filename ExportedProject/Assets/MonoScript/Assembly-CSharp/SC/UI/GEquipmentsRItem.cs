using UnityEngine;

namespace SC.UI
{
	public class GEquipmentsRItem : MonoBehaviour
	{
		public GameObject m_durability;

		public GameObject m_equipment_icon;

		public GameObject m_no_equipment;

		public object context;

		private void Awake()
		{
			m_durability = base.transform.Find("m_durability").gameObject;
			m_equipment_icon = base.transform.Find("m_equipment_icon").gameObject;
			m_no_equipment = base.transform.Find("m_no_equipment").gameObject;
		}
	}
}
