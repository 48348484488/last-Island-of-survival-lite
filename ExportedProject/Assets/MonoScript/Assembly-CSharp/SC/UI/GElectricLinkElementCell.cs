using UnityEngine;

namespace SC.UI
{
	public class GElectricLinkElementCell : MonoBehaviour
	{
		public GameObject btn_add;

		public GameObject m_connect;

		public GameObject m_connect_shuxian;

		public GameObject m_frame;

		public GameObject m_frame_0;

		public GameObject m_icon;

		public GameObject m_jiahao;

		public GameObject m_jiajianhao;

		public GameObject m_jianhao;

		public GameObject m_not_connect;

		public GameObject m_select;

		public GameObject m_tongdian_shuxian;

		public object context;

		private void Awake()
		{
			btn_add = base.transform.Find("m_not_connect/btn_add").gameObject;
			m_connect = base.transform.Find("m_connect").gameObject;
			m_connect_shuxian = base.transform.Find("m_connect/m_connect_shuxian").gameObject;
			m_frame = base.transform.Find("m_connect/m_frame").gameObject;
			m_frame_0 = base.transform.Find("m_not_connect/m_frame").gameObject;
			m_icon = base.transform.Find("m_connect/m_icon").gameObject;
			m_jiahao = base.transform.Find("m_connect/m_jiajianhao/m_jiahao").gameObject;
			m_jiajianhao = base.transform.Find("m_connect/m_jiajianhao").gameObject;
			m_jianhao = base.transform.Find("m_connect/m_jiajianhao/m_jianhao").gameObject;
			m_not_connect = base.transform.Find("m_not_connect").gameObject;
			m_select = base.transform.Find("m_connect/m_select").gameObject;
			m_tongdian_shuxian = base.transform.Find("m_connect/m_tongdian_shuxian").gameObject;
		}
	}
}
