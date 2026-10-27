using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GManorNameGroup : MonoBehaviour
	{
		public GameObject txt_off;

		public Text txt_offText;

		public GameObject txt_on;

		public Text txt_onText;

		public object context;

		private void Awake()
		{
			txt_off = base.transform.Find("Off/txt_off").gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			txt_on = base.transform.Find("On/txt_on").gameObject;
			txt_onText = txt_on.GetComponent<Text>();
		}
	}
}
