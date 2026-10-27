using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GLadderLevelAwardItem : MonoBehaviour
	{
		public GameObject m_junior;

		public GLadderLevelItem m_junior_item;

		public GameObject m_level_mask;

		public List<GLadderLevelItem> m_seniorslist = new List<GLadderLevelItem>();

		public GameObject[] m_seniors;

		public GameObject m_seniorsObj;

		public GameObject txt_level;

		public Text txt_levelText;

		public object context;

		private void Awake()
		{
			m_junior = base.transform.Find("m_junior").gameObject;
			m_junior_item = View.AddComponentIfNotExist<GLadderLevelItem>(base.transform.Find("m_junior/m_junior_item").gameObject);
			m_level_mask = base.transform.Find("level/m_level_mask").gameObject;
			m_seniors = base.transform.Find("m_seniors").gameObject.GetComponent<UIGameObjectList>().objects;
			m_seniorsObj = base.transform.Find("m_seniors").gameObject;
			if (m_seniorslist.Count <= 0)
			{
				for (int i = 0; i < m_seniors.Length; i++)
				{
					m_seniorslist.Add(View.AddComponentIfNotExist<GLadderLevelItem>(m_seniors[i].gameObject));
				}
			}
			txt_level = base.transform.Find("level/Image/txt_level").gameObject;
			txt_levelText = txt_level.GetComponent<Text>();
		}
	}
}
