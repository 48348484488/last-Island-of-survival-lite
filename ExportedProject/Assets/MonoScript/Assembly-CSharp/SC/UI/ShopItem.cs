using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class ShopItem : MonoBehaviour
	{
		public GameObject m_discount;

		public GameObject m_fen_ge_xian;

		public GameObject m_icon;

		public GameObject m_price_icon;

		public GameObject m_select;

		public GameObject m_sell_out;

		public GameObject txt_discount;

		public Text txt_discountText;

		public GameObject txt_extra_string;

		public Text txt_extra_stringText;

		public GameObject txt_full_price;

		public Text txt_full_priceText;

		public GameObject txt_limit;

		public Text txt_limitText;

		public GameObject txt_name;

		public Text txt_nameText;

		public GameObject txt_num;

		public Text txt_numText;

		public GameObject txt_real_price;

		public Text txt_real_priceText;

		public object context;

		private void Awake()
		{
			m_discount = base.transform.Find("m_discount").gameObject;
			m_fen_ge_xian = base.transform.Find("m_fen_ge_xian").gameObject;
			m_icon = base.transform.Find("m_icon").gameObject;
			m_price_icon = base.transform.Find("22/GameObject/m_price_icon").gameObject;
			m_select = base.transform.Find("m_select").gameObject;
			m_sell_out = base.transform.Find("m_sell_out").gameObject;
			txt_discount = base.transform.Find("m_discount/txt_discount").gameObject;
			txt_discountText = txt_discount.GetComponent<Text>();
			txt_extra_string = base.transform.Find("txt_extra_string").gameObject;
			txt_extra_stringText = txt_extra_string.GetComponent<Text>();
			txt_full_price = base.transform.Find("22/txt_full_price").gameObject;
			txt_full_priceText = txt_full_price.GetComponent<Text>();
			txt_limit = base.transform.Find("txt_limit").gameObject;
			txt_limitText = txt_limit.GetComponent<Text>();
			txt_name = base.transform.Find("txt_name").gameObject;
			txt_nameText = txt_name.GetComponent<Text>();
			txt_num = base.transform.Find("txt_limit/txt_num").gameObject;
			txt_numText = txt_num.GetComponent<Text>();
			txt_real_price = base.transform.Find("22/Image (1)/txt_real_price").gameObject;
			txt_real_priceText = txt_real_price.GetComponent<Text>();
		}
	}
}
