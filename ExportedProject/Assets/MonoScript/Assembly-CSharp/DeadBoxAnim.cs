using UnityEngine;

public class DeadBoxAnim : MonoBehaviour
{
	public Animator Ani;

	private void Start()
	{
		base.transform.Rotate(90f, 0f, 0f);
		if (Ani == null)
		{
			Ani = base.gameObject.GetComponent<Animator>();
		}
		if (Ani != null)
		{
			Ani.Play("close");
		}
	}

	public void Open()
	{
		if (Ani != null)
		{
			Ani.SetBool("toOpen", true);
			Ani.SetBool("toIdle", true);
			Ani.SetBool("toClose", false);
		}
	}

	public void Close()
	{
		if (Ani != null)
		{
			Ani.SetBool("toOpen", false);
			Ani.SetBool("toIdle", false);
			Ani.SetBool("toClose", true);
		}
	}
}
