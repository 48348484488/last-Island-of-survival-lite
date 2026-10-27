using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GGuideTaskConditionCell : MonoBehaviour
	{
		public GameObject m_finish;

		public GameObject txt_desc;

		public Text txt_descText;

		public object context;

		private void Awake()
		{
			m_finish = base.transform.Find("m_finish").gameObject;
			txt_desc = base.transform.Find("txt_desc").gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
		}
	}
}
