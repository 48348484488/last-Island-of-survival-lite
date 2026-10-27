using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GLeftTeamInfo : MonoBehaviour
	{
		public GameObject btn_add;

		public GameObject m_add;

		public GameObject m_car;

		public GameObject m_die;

		public GameObject m_frame;

		public GameObject m_hp;

		public GameObject m_icon;

		public GameObject m_leader;

		public GameObject m_mark;

		public GameObject m_notselect;

		public GameObject m_offline_dead;

		public GameObject m_quanxian;

		public GameObject m_select;

		public GameObject m_teamate;

		public GameObject m_waring;

		public GameObject txt_index;

		public Text txt_indexText;

		public GameObject txt_on;

		public Text txt_onText;

		public object context;

		private void Awake()
		{
			btn_add = base.transform.Find("m_add/btn_add").gameObject;
			m_add = base.transform.Find("m_add").gameObject;
			m_car = base.transform.Find("m_teamate/buff/m_car").gameObject;
			m_die = base.transform.Find("m_teamate/buff/m_die").gameObject;
			m_frame = base.transform.Find("m_teamate/Image (1)/m_icon/m_frame").gameObject;
			m_hp = base.transform.Find("m_teamate/Image/m_hp").gameObject;
			m_icon = base.transform.Find("m_teamate/Image (1)/m_icon").gameObject;
			m_leader = base.transform.Find("m_teamate/buff/m_leader").gameObject;
			m_mark = base.transform.Find("m_teamate/buff/m_mark").gameObject;
			m_notselect = base.transform.Find("m_teamate/buff_di/m_notselect").gameObject;
			m_offline_dead = base.transform.Find("m_teamate/buff_di/m_offline_dead").gameObject;
			m_quanxian = base.transform.Find("m_teamate/buff/m_quanxian").gameObject;
			m_select = base.transform.Find("m_teamate/buff_di/m_select").gameObject;
			m_teamate = base.transform.Find("m_teamate").gameObject;
			m_waring = base.transform.Find("m_teamate/buff/m_waring").gameObject;
			txt_index = base.transform.Find("m_teamate/Image/txt_index").gameObject;
			txt_indexText = txt_index.GetComponent<Text>();
			txt_on = base.transform.Find("m_teamate/buff_di/txt_on").gameObject;
			txt_onText = txt_on.GetComponent<Text>();
		}
	}
}
