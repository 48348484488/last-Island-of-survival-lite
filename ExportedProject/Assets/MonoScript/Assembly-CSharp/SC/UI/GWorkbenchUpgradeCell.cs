using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GWorkbenchUpgradeCell : MonoBehaviour
	{
		public GameObject btn_add;

		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject txt_num_enough;

		public Text txt_num_enoughText;

		public GameObject txt_num_not_enough;

		public Text txt_num_not_enoughText;

		public object context;

		private void Awake()
		{
			btn_add = base.transform.Find("btn_add").gameObject;
			m_frame = base.transform.Find("m_frame").gameObject;
			m_icon = base.transform.Find("m_icon").gameObject;
			txt_num_enough = base.transform.Find("txt_num_enough").gameObject;
			txt_num_enoughText = txt_num_enough.GetComponent<Text>();
			txt_num_not_enough = base.transform.Find("txt_num_not_enough").gameObject;
			txt_num_not_enoughText = txt_num_not_enough.GetComponent<Text>();
		}
	}
}
