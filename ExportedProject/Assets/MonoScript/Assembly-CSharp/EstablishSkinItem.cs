using UnityEngine;

public class EstablishSkinItem : MonoBehaviour
{
	public GameObject m_icon;

	public object context;

	private void Awake()
	{
		m_icon = base.transform.Find("m_icon").gameObject;
	}
}
