using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GActivityFirstChargeCell : MonoBehaviour
	{
		public GameObject btn_already_got;

		public GameObject btn_buy;

		public List<GActivityFirstChargeItemCell> m_itemslist = new List<GActivityFirstChargeItemCell>();

		public GameObject[] m_items;

		public GameObject m_itemsObj;

		public GameObject m_price;

		public GameObject m_price_icon;

		public GameObject txt_price;

		public Text txt_priceText;

		public object context;

		private void Awake()
		{
			btn_already_got = base.transform.Find("GameObject (1)/btn_already_got").gameObject;
			btn_buy = base.transform.Find("GameObject (1)/btn_buy").gameObject;
			m_items = base.transform.Find("m_items").gameObject.GetComponent<UIGameObjectList>().objects;
			m_itemsObj = base.transform.Find("m_items").gameObject;
			if (m_itemslist.Count <= 0)
			{
				for (int i = 0; i < m_items.Length; i++)
				{
					m_itemslist.Add(View.AddComponentIfNotExist<GActivityFirstChargeItemCell>(m_items[i].gameObject));
				}
			}
			m_price = base.transform.Find("m_price").gameObject;
			m_price_icon = base.transform.Find("m_price/m_price_icon").gameObject;
			txt_price = base.transform.Find("m_price/txt_price").gameObject;
			txt_priceText = txt_price.GetComponent<Text>();
		}
	}
}
