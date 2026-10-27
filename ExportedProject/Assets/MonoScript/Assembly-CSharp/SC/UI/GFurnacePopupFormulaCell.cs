using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GFurnacePopupFormulaCell : MonoBehaviour
	{
		public GameObject m_icon_material;

		public GameObject m_icon_production;

		public GameObject txt_num_material;

		public Text txt_num_materialText;

		public GameObject txt_num_production;

		public Text txt_num_productionText;

		public GameObject txt_time;

		public Text txt_timeText;

		public object context;

		private void Awake()
		{
			m_icon_material = base.transform.Find("GameObject/Image/m_icon_material").gameObject;
			m_icon_production = base.transform.Find("GameObject/Image (3)/m_icon_production").gameObject;
			txt_num_material = base.transform.Find("GameObject/Image/txt_num_material").gameObject;
			txt_num_materialText = txt_num_material.GetComponent<Text>();
			txt_num_production = base.transform.Find("GameObject/Image (3)/txt_num_production").gameObject;
			txt_num_productionText = txt_num_production.GetComponent<Text>();
			txt_time = base.transform.Find("GameObject/Image (2)/txt_time").gameObject;
			txt_timeText = txt_time.GetComponent<Text>();
		}
	}
}
