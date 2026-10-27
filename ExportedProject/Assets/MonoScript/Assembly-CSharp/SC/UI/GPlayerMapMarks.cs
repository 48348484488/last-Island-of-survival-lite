using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GPlayerMapMarks : MonoBehaviour
	{
		public GameObject m_dead;

		public GameObject m_drive;

		public GameObject m_mark;

		public GBattlePanelMarkDir m_mark_dir;

		public GameObject m_pos;

		public GameObject m_pos_light;

		public GameObject txt_role_index;

		public Text txt_role_indexText;

		public object context;

		private void Awake()
		{
			m_dead = base.transform.Find("m_dead").gameObject;
			m_drive = base.transform.Find("m_drive").gameObject;
			m_mark = base.transform.Find("m_mark").gameObject;
			m_mark_dir = View.AddComponentIfNotExist<GBattlePanelMarkDir>(base.transform.Find("m_mark_dir").gameObject);
			m_pos = base.transform.Find("m_pos").gameObject;
			m_pos_light = base.transform.Find("m_pos_light").gameObject;
			txt_role_index = base.transform.Find("txt_role_index").gameObject;
			txt_role_indexText = txt_role_index.GetComponent<Text>();
		}
	}
}
