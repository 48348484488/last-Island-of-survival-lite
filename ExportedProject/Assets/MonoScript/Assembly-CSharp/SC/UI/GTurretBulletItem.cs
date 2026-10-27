using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GTurretBulletItem : MonoBehaviour
	{
		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject m_select;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			m_frame = base.transform.Find("m_frame").gameObject;
			m_icon = base.transform.Find("m_icon").gameObject;
			m_select = base.transform.Find("m_select").gameObject;
			txt_num = base.transform.Find("txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
