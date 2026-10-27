using UnityEngine;
using UnityEngine.UI;

public class EstablishGCountryItem : MonoBehaviour
{
	public GameObject m_flag_icon;

	public GameObject txt_country_name;

	public Text txt_country_nameText;

	public object context;

	private void Awake()
	{
		m_flag_icon = base.transform.Find("Image/m_flag_icon").gameObject;
		txt_country_name = base.transform.Find("Image (1)/txt_country_name").gameObject;
		txt_country_nameText = txt_country_name.GetComponent<Text>();
	}
}
