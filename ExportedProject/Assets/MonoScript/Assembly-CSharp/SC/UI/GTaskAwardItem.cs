using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GTaskAwardItem : MonoBehaviour
	{
		public GameObject m_ward_icon;

		public GameObject txt_award_num;

		public Text txt_award_numText;

		public object context;

		private void Awake()
		{
			m_ward_icon = base.transform.Find("m_ward_icon").gameObject;
			txt_award_num = base.transform.Find("m_ward_icon/txt_award_num").gameObject;
			txt_award_numText = txt_award_num.GetComponent<Text>();
		}
	}
}
