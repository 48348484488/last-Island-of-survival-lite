using UnityEngine;

namespace SC.UI
{
	public class GZhunxing : MonoBehaviour
	{
		public GameObject m_AimDown;

		public GameObject m_AimLeft;

		public GameObject m_AimRight;

		public GameObject m_AimUp;

		public object context;

		private void Awake()
		{
			m_AimDown = base.transform.Find("m_AimDown").gameObject;
			m_AimLeft = base.transform.Find("m_AimLeft").gameObject;
			m_AimRight = base.transform.Find("m_AimRight").gameObject;
			m_AimUp = base.transform.Find("m_AimUp").gameObject;
		}
	}
}
