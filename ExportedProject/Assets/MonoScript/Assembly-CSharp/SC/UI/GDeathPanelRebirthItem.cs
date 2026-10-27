using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GDeathPanelRebirthItem : MonoBehaviour
	{
		public GameObject ckb_select;

		public GameObject ckb_select_random;

		public GameObject m_bed;

		public GameObject m_random_root;

		public GameObject m_rebirth_pos;

		public GameObject m_sleeping_bag;

		public GameObject m_time;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_time;

		public Text txt_timeText;

		public object context;

		private void Awake()
		{
			ckb_select = base.transform.Find("m_rebirth_pos/ckb_select").gameObject;
			ckb_select_random = base.transform.Find("m_random_root/ckb_select_random").gameObject;
			m_bed = base.transform.Find("m_rebirth_pos/m_bed").gameObject;
			m_random_root = base.transform.Find("m_random_root").gameObject;
			m_rebirth_pos = base.transform.Find("m_rebirth_pos").gameObject;
			m_sleeping_bag = base.transform.Find("m_rebirth_pos/m_sleeping_bag").gameObject;
			m_time = base.transform.Find("m_rebirth_pos/m_time").gameObject;
			txt_name = base.transform.Find("m_rebirth_pos/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_time = base.transform.Find("m_rebirth_pos/m_time/txt_time").gameObject;
			txt_timeText = txt_time.GetComponent<Text>();
		}
	}
}
