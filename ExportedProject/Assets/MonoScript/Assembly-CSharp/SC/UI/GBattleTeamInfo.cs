using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GBattleTeamInfo : MonoBehaviour
	{
		public GameObject m_car;

		public GameObject m_die;

		public GameObject m_hp;

		public GameObject m_mark;

		public GameObject m_waring;

		public GameObject txt_index;

		public Text txt_indexText;

		public GameObject txt_off;

		public Text txt_offText;

		public GameObject txt_on;

		public Text txt_onText;

		public object context;

		private void Awake()
		{
			m_car = base.transform.Find("GameObject/buff/m_car").gameObject;
			m_die = base.transform.Find("GameObject/buff/m_die").gameObject;
			m_hp = base.transform.Find("GameObject/m_hp").gameObject;
			m_mark = base.transform.Find("GameObject/buff/m_mark").gameObject;
			m_waring = base.transform.Find("GameObject/buff/m_waring").gameObject;
			txt_index = base.transform.Find("GameObject/Image/txt_index").gameObject;
			txt_indexText = txt_index.GetComponent<Text>();
			txt_off = base.transform.Find("GameObject/Team1/Off/txt_off").gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			txt_on = base.transform.Find("GameObject/Team1/On/txt_on").gameObject;
			txt_onText = txt_on.GetComponent<Text>();
		}
	}
}
