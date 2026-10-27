using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GManorChestRepairCell : MonoBehaviour
	{
		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_num;

		public Text txt_numText;

		public object context;

		private void Awake()
		{
			txt_name = base.transform.Find("Image/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_num = base.transform.Find("Image/txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
		}
	}
}
