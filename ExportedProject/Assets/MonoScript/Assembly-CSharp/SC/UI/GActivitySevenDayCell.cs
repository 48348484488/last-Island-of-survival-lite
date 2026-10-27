using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GActivitySevenDayCell : MonoBehaviour
	{
		public GameObject btn_already_got;

		public GameObject btn_get;

		public GActivitySevenDayItemCell m_frame;

		public GameObject m_item_parent;

		public GameObject txt_name;

		public Text txt_nameText;

		public object context;

		private void Awake()
		{
			btn_already_got = base.transform.Find("btn_already_got").gameObject;
			btn_get = base.transform.Find("btn_get").gameObject;
			m_frame = View.AddComponentIfNotExist<GActivitySevenDayItemCell>(base.transform.Find("m_item_parent/m_frame").gameObject);
			m_item_parent = base.transform.Find("m_item_parent").gameObject;
			txt_name = base.transform.Find("txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
		}
	}
}
