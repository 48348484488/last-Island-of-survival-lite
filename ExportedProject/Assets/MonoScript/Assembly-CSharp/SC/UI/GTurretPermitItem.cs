using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GTurretPermitItem : MonoBehaviour
	{
		public GameObject ckb_permit;

		public GameObject m_icon;

		public GameObject txt_name;

		public Text txt_nameText;

		public object context;

		private void Awake()
		{
			ckb_permit = base.transform.Find("GameObject/ckb_permit").gameObject;
			m_icon = base.transform.Find("GameObject/head (1)/2/m_icon").gameObject;
			txt_name = base.transform.Find("GameObject/txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
		}
	}
}
