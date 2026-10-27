using UnityEngine;

namespace SC.UI
{
	public class TaskPage : MonoBehaviour
	{
		public GameObject m_cell;

		public GameObject scp_;

		public object context;

		private void Awake()
		{
			m_cell = base.transform.Find("GameObject/Image (1)/scp_/content/m_cell").gameObject;
			scp_ = base.transform.Find("GameObject/Image (1)/scp_").gameObject;
		}
	}
}
