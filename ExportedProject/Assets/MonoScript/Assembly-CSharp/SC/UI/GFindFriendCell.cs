using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GFindFriendCell : MonoBehaviour
	{
		public GameObject btn_care;

		public GameObject btn_look_detailInfo;

		public GameObject m_care_eachother;

		public GameObject m_care_parent;

		public GameObject m_icon;

		public GameObject txt_roleId;

		public Text txt_roleIdText;

		public object context;

		private void Awake()
		{
			btn_care = base.transform.Find("btn_care").gameObject;
			btn_look_detailInfo = base.transform.Find("btn_look_detailInfo").gameObject;
			m_care_eachother = base.transform.Find("m_care_parent/m_care_eachother").gameObject;
			m_care_parent = base.transform.Find("m_care_parent").gameObject;
			m_icon = base.transform.Find("Image (1)/head/2/m_icon").gameObject;
			txt_roleId = base.transform.Find("ID (1)/txt_roleId").gameObject;
			txt_roleIdText = txt_roleId.GetComponent<Text>();
		}
	}
}
