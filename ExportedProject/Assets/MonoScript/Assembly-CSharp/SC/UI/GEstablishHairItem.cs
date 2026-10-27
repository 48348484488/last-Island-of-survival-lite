using UnityEngine;

namespace SC.UI
{
	public class GEstablishHairItem : MonoBehaviour
	{
		public GameObject m_hair_icon;

		public GameObject m_select;

		public object context;

		private void Awake()
		{
			m_hair_icon = base.transform.Find("Image/m_hair_icon").gameObject;
			m_select = base.transform.Find("Image/m_select").gameObject;
		}
	}
}
