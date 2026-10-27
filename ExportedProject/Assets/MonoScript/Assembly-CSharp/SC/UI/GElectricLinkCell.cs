using System.Collections.Generic;
using UnityEngine;

namespace SC.UI
{
	public class GElectricLinkCell : MonoBehaviour
	{
		public GameObject m_child_line;

		public GameObject m_child_no_h_line;

		public GameObject m_child_no_v_line;

		public GameObject m_child_tonglediandehengxian;

		public GameObject m_child_tonglediandeshuxian;

		public GameObject m_child_yellow_h_line;

		public GameObject m_child_yellow_v_line;

		public List<GElectricLinkElementCell> m_elementslist = new List<GElectricLinkElementCell>();

		public GameObject[] m_elements;

		public GameObject m_elementsObj;

		public object context;

		private void Awake()
		{
			m_child_line = base.transform.Find("GameObject/m_child_line").gameObject;
			m_child_no_h_line = base.transform.Find("GameObject/m_child_line/m_child_no_h_line").gameObject;
			m_child_no_v_line = base.transform.Find("GameObject/m_child_line/m_child_no_v_line").gameObject;
			m_child_tonglediandehengxian = base.transform.Find("GameObject/m_child_line/m_child_tonglediandehengxian").gameObject;
			m_child_tonglediandeshuxian = base.transform.Find("GameObject/m_child_line/m_child_tonglediandeshuxian").gameObject;
			m_child_yellow_h_line = base.transform.Find("GameObject/m_child_line/m_child_yellow_h_line").gameObject;
			m_child_yellow_v_line = base.transform.Find("GameObject/m_child_line/m_child_yellow_v_line").gameObject;
			m_elements = base.transform.Find("GameObject/m_elements").gameObject.GetComponent<UIGameObjectList>().objects;
			m_elementsObj = base.transform.Find("GameObject/m_elements").gameObject;
			if (m_elementslist.Count <= 0)
			{
				for (int i = 0; i < m_elements.Length; i++)
				{
					m_elementslist.Add(View.AddComponentIfNotExist<GElectricLinkElementCell>(m_elements[i].gameObject));
				}
			}
		}
	}
}
