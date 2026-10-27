using UnityEngine;

namespace SC.UI
{
	public class GHelmetInfo : MonoBehaviour
	{
		public GameObject m_helmetInfoHp;

		public GameObject m_helmetlv1;

		public GameObject m_helmetlv2;

		public GameObject m_helmetlv3;

		public object context;

		private void Awake()
		{
			m_helmetInfoHp = base.transform.Find("g/m_helmetInfoHp").gameObject;
			m_helmetlv1 = base.transform.Find("g/m_helmetlv1").gameObject;
			m_helmetlv2 = base.transform.Find("g/m_helmetlv2").gameObject;
			m_helmetlv3 = base.transform.Find("g/m_helmetlv3").gameObject;
		}
	}
}
