using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GFirePopupFuelCell : MonoBehaviour
	{
		public GameObject m_icon;

		public GameObject txt_name;

		public Text txt_nameText;

		public object context;

		private void Awake()
		{
			m_icon = base.transform.Find("m_icon").gameObject;
			txt_name = base.transform.Find("txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
		}
	}
}
