using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GFirePopupFormulaCell : MonoBehaviour
	{
		public GameObject m_icon_production;

		public List<GFirePopupMaterialCell> m_materialslist = new List<GFirePopupMaterialCell>();

		public GameObject[] m_materials;

		public GameObject m_materialsObj;

		public GameObject txt_name_num;

		public Text txt_name_numText;

		public GameObject txt_time;

		public Text txt_timeText;

		public object context;

		private void Awake()
		{
			m_icon_production = base.transform.Find("m_materials/GameObject/Image (3)/m_icon_production").gameObject;
			m_materials = base.transform.Find("m_materials").gameObject.GetComponent<UIGameObjectList>().objects;
			m_materialsObj = base.transform.Find("m_materials").gameObject;
			if (m_materialslist.Count <= 0)
			{
				for (int i = 0; i < m_materials.Length; i++)
				{
					m_materialslist.Add(View.AddComponentIfNotExist<GFirePopupMaterialCell>(m_materials[i].gameObject));
				}
			}
			txt_name_num = base.transform.Find("m_materials/GameObject/Image (3)/txt_name_num").gameObject;
			txt_name_numText = txt_name_num.GetComponent<Text>();
			txt_time = base.transform.Find("m_materials/GameObject/Image (2)/txt_time").gameObject;
			txt_timeText = txt_time.GetComponent<Text>();
		}
	}
}
