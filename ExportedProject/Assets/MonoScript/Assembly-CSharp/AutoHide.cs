using System.Runtime.CompilerServices;
using UnityEngine;

public class AutoHide : MonoBehaviour
{
	public float Delay = 1f;

	private int m_instanceId;

	private void OnEnable()
	{
		m_instanceId = base.gameObject.GetInstanceID();
		DelayInvoker.DelayInvoke("AutoHide" + m_instanceId, Delay, _003COnEnable_003Em__0);
	}

	private void OnDisable()
	{
		DelayInvoker.CancelInvoke("AutoHide" + m_instanceId);
	}

	private void OnDestroy()
	{
		DelayInvoker.CancelInvoke("AutoHide" + m_instanceId);
	}

	[CompilerGenerated]
	private void _003COnEnable_003Em__0(object[] o)
	{
		base.gameObject.SetActive(false);
	}
}
