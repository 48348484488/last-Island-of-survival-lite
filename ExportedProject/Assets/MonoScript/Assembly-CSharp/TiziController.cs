using UnityEngine;

public class TiziController : MonoBehaviour
{
	private Animator m_animator;

	public bool Opening;

	private GameObject tizi;

	private void Awake()
	{
		m_animator = GetComponentInChildren<Animator>();
		tizi = base.transform.Find("lvp").Find("tizi").gameObject;
		tizi.SetActiveBetter(false);
	}

	public void Open()
	{
		Opening = true;
		m_animator.Play("open");
		tizi.SetActiveBetter(true);
	}

	public void Close()
	{
		if (Opening)
		{
			Opening = false;
			m_animator.Play("close");
			tizi.SetActiveBetter(false);
		}
	}
}
