using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GBattlePickCell : MonoBehaviour
	{
		public GameObject m_armor_value;

		public GameObject m_hl_bg;

		public GameObject m_icon;

		public GameObject m_novice_effect;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			m_armor_value = base.transform.Find("m_armor_value").gameObject;
			m_hl_bg = base.transform.Find("m_hl_bg").gameObject;
			m_icon = base.transform.Find("m_icon").gameObject;
			m_novice_effect = base.transform.Find("m_novice_effect").gameObject;
			txt_name = base.transform.Find("Image (3)/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_num = base.transform.Find("txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
