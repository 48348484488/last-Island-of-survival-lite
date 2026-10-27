using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GFriendRecordCell : MonoBehaviour
	{
		public GameObject txt_content;

		public Text txt_contentText;

		public GameObject txt_date;

		public Text txt_dateText;

		public object context;

		private void Awake()
		{
			txt_content = base.transform.Find("txt_content").gameObject;
			txt_contentText = txt_content.GetComponent<Text>();
			txt_date = base.transform.Find("txt_date").gameObject;
			txt_dateText = txt_date.GetComponent<Text>();
		}
	}
}
