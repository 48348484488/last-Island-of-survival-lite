using UnityEngine;

namespace SC.UI
{
	public class GBattlePanelMarkDir : MonoBehaviour
	{
		public GameObject m_mark;

		public object context;

		private void Awake()
		{
			m_mark = base.transform.Find("Image/m_mark").gameObject;
		}
	}
}
