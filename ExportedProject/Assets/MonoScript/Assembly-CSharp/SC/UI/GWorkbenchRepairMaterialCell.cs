using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GWorkbenchRepairMaterialCell : MonoBehaviour
	{
		public GameObject btn_add;

		public GameObject m_frame;

		public GameObject m_icon;

		public GameObject m_item;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			btn_add = base.transform.Find("m_item/btn_add").gameObject;
			m_frame = base.transform.Find("m_frame").gameObject;
			m_icon = base.transform.Find("m_item/m_icon").gameObject;
			m_item = base.transform.Find("m_item").gameObject;
			txt_name = base.transform.Find("m_item/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_num = base.transform.Find("m_item/txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
