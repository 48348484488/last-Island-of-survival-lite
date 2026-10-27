using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GTaskItem : MonoBehaviour
	{
		public GameObject btn_already_get;

		public GameObject btn_get;

		public GameObject btn_go;

		public GameObject btn_hand_in;

		public List<GTaskAwardItem> m_awardslist = new List<GTaskAwardItem>();

		public GameObject[] m_awards;

		public GameObject m_awardsObj;

		public GameObject m_bg;

		public GameObject m_bg_jingying;

		public GameObject m_challenge;

		public GameObject m_jingying;

		public GameObject m_normal;

		public GameObject m_task_icon;

		public GameObject txt_condition;

		public Text txt_conditionText;

		public GameObject txt_task_desc;

		public Text txt_task_descText;

		public object context;

		private void Awake()
		{
			btn_already_get = base.transform.Find("btns/btn_already_get").gameObject;
			btn_get = base.transform.Find("btns/btn_get").gameObject;
			btn_go = base.transform.Find("btns/btn_go").gameObject;
			btn_hand_in = base.transform.Find("btns/btn_hand_in").gameObject;
			m_awards = base.transform.Find("m_awards").gameObject.GetComponent<UIGameObjectList>().objects;
			m_awardsObj = base.transform.Find("m_awards").gameObject;
			if (m_awardslist.Count <= 0)
			{
				for (int i = 0; i < m_awards.Length; i++)
				{
					m_awardslist.Add(View.AddComponentIfNotExist<GTaskAwardItem>(m_awards[i].gameObject));
				}
			}
			m_bg = base.transform.Find("m_bg").gameObject;
			m_bg_jingying = base.transform.Find("m_bg_jingying").gameObject;
			m_challenge = base.transform.Find("Image/m_challenge").gameObject;
			m_jingying = base.transform.Find("Image/m_jingying").gameObject;
			m_normal = base.transform.Find("Image/m_normal").gameObject;
			m_task_icon = base.transform.Find("m_task_icon").gameObject;
			txt_condition = base.transform.Find("btns/txt_condition").gameObject;
			txt_conditionText = txt_condition.GetComponent<Text>();
			txt_task_desc = base.transform.Find("txt_task_desc").gameObject;
			txt_task_descText = txt_task_desc.GetComponent<Text>();
		}
	}
}
