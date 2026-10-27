using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GBagProduceRequirementCell : MonoBehaviour
	{
		public GameObject btn_add;

		public GameObject m_icon_desc;

		public GameObject txt_num_desc;

		public Text txt_num_descText;

		public object context;

		private void Awake()
		{
			btn_add = base.transform.Find("btn_add").gameObject;
			m_icon_desc = base.transform.Find("m_icon_desc").gameObject;
			txt_num_desc = base.transform.Find("txt_num_desc").gameObject;
			txt_num_descText = txt_num_desc.GetComponent<Text>();
		}
	}
}
