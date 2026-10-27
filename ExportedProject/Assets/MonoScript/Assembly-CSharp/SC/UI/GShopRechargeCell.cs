using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class GShopRechargeCell : MonoBehaviour
	{
		public GameObject m_something;

		public GameObject m_token_icon_big;

		public GameObject m_token_icon_small;

		public GameObject m_token_icon_zeng;

		public GameObject m_zeng;

		public GameObject txt_num_zeng;

		public Text txt_num_zengText;

		public GameObject txt_rmb_num;

		public Text txt_rmb_numText;

		public GameObject txt_token_num;

		public Text txt_token_numText;

		public object context;

		private void Awake()
		{
			m_something = base.transform.Find("m_something").gameObject;
			m_token_icon_big = base.transform.Find("m_something/m_token_icon_big").gameObject;
			m_token_icon_small = base.transform.Find("m_something/m_token_icon_small").gameObject;
			m_token_icon_zeng = base.transform.Find("m_something/m_zeng/m_token_icon_zeng").gameObject;
			m_zeng = base.transform.Find("m_something/m_zeng").gameObject;
			txt_num_zeng = base.transform.Find("m_something/m_zeng/txt_num_zeng").gameObject;
			txt_num_zengText = txt_num_zeng.GetComponent<Text>();
			txt_rmb_num = base.transform.Find("m_something/txt_rmb_num").gameObject;
			txt_rmb_numText = txt_rmb_num.GetComponent<Text>();
			txt_token_num = base.transform.Find("m_something/m_token_icon_small/txt_token_num").gameObject;
			txt_token_numText = txt_token_num.GetComponent<Text>();
		}
	}
}
