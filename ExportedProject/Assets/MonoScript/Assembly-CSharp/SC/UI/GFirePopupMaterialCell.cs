using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GFirePopupMaterialCell : MonoBehaviour
	{
		public GameObject m_icon_material;

		public GameObject txt_name_num;

		public Text txt_name_numText;

		public object context;

		private void Awake()
		{
			m_icon_material = base.transform.Find("m_icon_material").gameObject;
			txt_name_num = base.transform.Find("txt_name_num").gameObject;
			txt_name_numText = txt_name_num.GetComponent<Text>();
		}
	}
}
