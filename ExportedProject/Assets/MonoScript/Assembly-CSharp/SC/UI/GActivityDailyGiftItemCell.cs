using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GActivityDailyGiftItemCell : MonoBehaviour
	{
		public GameObject m_bind;

		public GameObject m_icon;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			m_bind = base.transform.Find("m_bind").gameObject;
			m_icon = base.transform.Find("m_icon").gameObject;
			txt_num = base.transform.Find("txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
