using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GKillItem : MonoBehaviour
	{
		public GameObject m_killIcon;

		public GameObject txt_killedName;

		public Text txt_killedNameText;

		public GameObject txt_killerName;

		public Text txt_killerNameText;

		public GameObject txt_killTxt;

		public Text txt_killTxtText;

		public object context;

		private void Awake()
		{
			m_killIcon = base.transform.Find("kill/m_killIcon").gameObject;
			txt_killedName = base.transform.Find("kill/txt_killedName").gameObject;
			txt_killedNameText = txt_killedName.GetComponent<Text>();
			txt_killerName = base.transform.Find("kill/txt_killerName").gameObject;
			txt_killerNameText = txt_killerName.GetComponent<Text>();
			txt_killTxt = base.transform.Find("kill/txt_killTxt").gameObject;
			txt_killTxtText = txt_killTxt.GetComponent<Text>();
		}
	}
}
