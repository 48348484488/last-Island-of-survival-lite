using UnityEngine;

namespace SC.UI
{
	public class GEstablishRoleItem : MonoBehaviour
	{
		public GameObject m_role_face_icon;

		public GameObject m_select;

		public object context;

		private void Awake()
		{
			m_role_face_icon = base.transform.Find("Image/m_role_face_icon").gameObject;
			m_select = base.transform.Find("Image/m_select").gameObject;
		}
	}
}
