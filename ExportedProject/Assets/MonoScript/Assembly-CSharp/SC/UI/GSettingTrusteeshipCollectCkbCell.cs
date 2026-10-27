using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GSettingTrusteeshipCollectCkbCell : MonoBehaviour
	{
		public GameObject txt_name;

		public Text txt_nameText;

		public object context;

		private void Awake()
		{
			txt_name = base.transform.Find("txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
		}
	}
}
