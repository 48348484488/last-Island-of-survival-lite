using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GBuildBagItem : MonoBehaviour
	{
		public GameObject m_bind;

		public GameObject[] m_bluePoint;

		public GameObject m_bluePointObj;

		public GameObject[] m_di;

		public GameObject m_diObj;

		public GameObject m_durationSlider;

		public GameObject m_gunInfo;

		public GameObject m_heightLignt;

		public GameObject m_icon;

		public GameObject m_newbee_effect;

		public GameObject m_s;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			m_bind = base.transform.Find("m_s/m_bind").gameObject;
			m_bluePoint = base.transform.Find("m_gunInfo/m_bluePoint").gameObject.GetComponent<UIGameObjectList>().objects;
			m_bluePointObj = base.transform.Find("m_gunInfo/m_bluePoint").gameObject;
			m_di = base.transform.Find("m_gunInfo/m_di").gameObject.GetComponent<UIGameObjectList>().objects;
			m_diObj = base.transform.Find("m_gunInfo/m_di").gameObject;
			m_durationSlider = base.transform.Find("m_s/m_durationSlider").gameObject;
			m_gunInfo = base.transform.Find("m_gunInfo").gameObject;
			m_heightLignt = base.transform.Find("m_heightLignt").gameObject;
			m_icon = base.transform.Find("m_s/m_icon").gameObject;
			m_newbee_effect = base.transform.Find("m_newbee_effect").gameObject;
			m_s = base.transform.Find("m_s").gameObject;
			txt_num = base.transform.Find("m_s/txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
