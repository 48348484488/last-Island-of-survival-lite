using System.Collections.Generic;
using UnityEngine;

namespace SC.UI
{
	public class GSettingTrusteeshipCollectCell : MonoBehaviour
	{
		public List<GSettingTrusteeshipCollectCkbCell> m_ckbslist = new List<GSettingTrusteeshipCollectCkbCell>();

		public GameObject[] m_ckbs;

		public GameObject m_ckbsObj;

		public object context;

		private void Awake()
		{
			m_ckbs = base.transform.Find("m_ckbs").gameObject.GetComponent<UIGameObjectList>().objects;
			m_ckbsObj = base.transform.Find("m_ckbs").gameObject;
			if (m_ckbslist.Count <= 0)
			{
				for (int i = 0; i < m_ckbs.Length; i++)
				{
					m_ckbslist.Add(View.AddComponentIfNotExist<GSettingTrusteeshipCollectCkbCell>(m_ckbs[i].gameObject));
				}
			}
		}
	}
}
