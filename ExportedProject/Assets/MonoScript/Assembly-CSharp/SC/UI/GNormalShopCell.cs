using UnityEngine;

namespace SC.UI
{
	public class GNormalShopCell : MonoBehaviour
	{
		public GameObject m_nothing;

		public ShopItem m_something;

		public object context;

		private void Awake()
		{
			m_nothing = base.transform.Find("m_nothing").gameObject;
			m_something = View.AddComponentIfNotExist<ShopItem>(base.transform.Find("m_something").gameObject);
		}
	}
}
