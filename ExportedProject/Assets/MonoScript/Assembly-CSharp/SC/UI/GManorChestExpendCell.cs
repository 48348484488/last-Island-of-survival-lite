using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GManorChestExpendCell : MonoBehaviour
	{
		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_time;

		public Text txt_timeText;

		public object context;

		private void Awake()
		{
			txt_name = base.transform.Find("txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_time = base.transform.Find("txt_time").gameObject;
			txt_timeText = txt_time.GetComponent<Text>();
		}
	}
}
