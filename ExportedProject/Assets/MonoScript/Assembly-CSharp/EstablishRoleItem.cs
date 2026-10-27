using UnityEngine;

public class EstablishRoleItem : MonoBehaviour
{
	public GameObject m_icon;

	public object context;

	private void Awake()
	{
		m_icon = base.transform.Find("On/m_icon").gameObject;
	}
}
