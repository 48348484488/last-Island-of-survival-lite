using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GBagProduceListCell : MonoBehaviour
	{
		public GameObject m_icon;

		public GameObject m_select;

		public GameObject txt_num;

		public Text txt_numText;

		public GameObject txt_queuing;

		public Text txt_queuingText;

		public object context;

		private void Awake()
		{
			m_icon = base.transform.Find("m_icon").gameObject;
			m_select = base.transform.Find("Background/m_select").gameObject;
			txt_num = base.transform.Find("txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
			txt_queuing = base.transform.Find("txt_queuing").gameObject;
			txt_queuingText = txt_queuing.GetComponent<Text>();
		}
	}
}
