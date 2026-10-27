using UnityEngine;

namespace SC.UI
{
	public class GBulletCloth : MonoBehaviour
	{
		public GameObject m_bulletclothhp;

		public GameObject m_bulletclothlv1;

		public GameObject m_bulletclothlv2;

		public GameObject m_bulletclothlv3;

		public object context;

		private void Awake()
		{
			m_bulletclothhp = base.transform.Find("g/m_bulletclothhp").gameObject;
			m_bulletclothlv1 = base.transform.Find("g/m_bulletclothlv1").gameObject;
			m_bulletclothlv2 = base.transform.Find("g/m_bulletclothlv2").gameObject;
			m_bulletclothlv3 = base.transform.Find("g/m_bulletclothlv3").gameObject;
		}
	}
}
