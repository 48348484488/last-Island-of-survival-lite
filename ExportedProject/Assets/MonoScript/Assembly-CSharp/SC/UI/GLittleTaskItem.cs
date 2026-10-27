using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GLittleTaskItem : MonoBehaviour
	{
		public GameObject m_finish;

		public GameObject txt_desc;

		public Text txt_descText;

		public GameObject txt_desc_condition;

		public Text txt_desc_conditionText;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_progress;

		public Text txt_progressText;

		public object context;

		private void Awake()
		{
			m_finish = base.transform.Find("condition/m_finish").gameObject;
			txt_desc = base.transform.Find("txt_desc").gameObject;
			txt_descText = txt_desc.GetComponent<Text>();
			txt_desc_condition = base.transform.Find("condition/txt_desc_condition").gameObject;
			txt_desc_conditionText = txt_desc_condition.GetComponent<Text>();
			txt_name = base.transform.Find("txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_progress = base.transform.Find("condition/txt_progress").gameObject;
			txt_progressText = txt_progress.GetComponent<Text>();
		}
	}
}
