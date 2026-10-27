using UnityEngine;

namespace SC.UI
{
	public class GJoystackType2 : MonoBehaviour
	{
		public GameObject m_littleYaogan;

		public GameObject m_littleYaoganParent;

		public GameObject m_YaoganArrow;

		public GameObject m_YaoganBG;

		public GameObject m_YaoganCircle;

		public GameObject m_yaoganTrigger;

		public object context;

		private void Awake()
		{
			m_littleYaogan = base.transform.Find("m_littleYaoganParent/m_littleYaogan").gameObject;
			m_littleYaoganParent = base.transform.Find("m_littleYaoganParent").gameObject;
			m_YaoganArrow = base.transform.Find("m_littleYaoganParent/m_littleYaogan/m_YaoganArrow").gameObject;
			m_YaoganBG = base.transform.Find("m_littleYaoganParent/m_littleYaogan/m_YaoganBG").gameObject;
			m_YaoganCircle = base.transform.Find("m_littleYaoganParent/m_littleYaogan/m_YaoganCircle").gameObject;
			m_yaoganTrigger = base.transform.Find("m_yaoganTrigger").gameObject;
		}
	}
}
