using UnityEngine;
using UnityEngine.UI;

namespace SC.UI
{
	public class BagGGunPartItem : MonoBehaviour
	{
		public GameObject m_duration;

		public GameObject m_gun_part_red;

		public GameObject m_has;

		public GameObject m_icon;

		public GameObject m_no_has;

		public GameObject m_suo;

		public GameObject txt_partname;

		public Text txt_partnameText;

		public GameObject txt_partname_0;

		public Text txt_partname_0Text;

		public object context;

		private void Awake()
		{
			m_duration = base.transform.Find("m_has/m_duration").gameObject;
			m_gun_part_red = base.transform.Find("m_gun_part_red").gameObject;
			m_has = base.transform.Find("m_has").gameObject;
			m_icon = base.transform.Find("m_has/m_icon").gameObject;
			m_no_has = base.transform.Find("m_no_has").gameObject;
			m_suo = base.transform.Find("m_has/m_suo").gameObject;
			txt_partname = base.transform.Find("m_no_has/Image/txt_partname").gameObject;
			txt_partnameText = txt_partname.GetComponent<Text>();
			txt_partname_0 = base.transform.Find("m_has/Image/txt_partname").gameObject;
			txt_partname_0Text = txt_partname_0.GetComponent<Text>();
		}
	}
}
