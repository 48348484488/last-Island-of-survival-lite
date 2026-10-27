using System.Runtime.CompilerServices;
using UnityEngine;

public class AutoRecycle : MonoBehaviour
{
	public float Delay = 15f;

	public string Name;

	private int m_instanceId;

	private void OnEnable()
	{
		m_instanceId = base.gameObject.GetInstanceID();
		DelayInvoker.CancelInvoke("AutoRecycle" + m_instanceId);
		DelayInvoker.DelayInvoke("AutoRecycle" + m_instanceId, Delay, _003COnEnable_003Em__0);
	}

	private void OnDisable()
	{
		DelayInvoker.CancelInvoke("AutoRecycle" + m_instanceId);
	}

	private void OnDestroy()
	{
		DelayInvoker.CancelInvoke("AutoRecycle" + m_instanceId);
	}

	[CompilerGenerated]
	private void _003COnEnable_003Em__0(object[] o)
	{
		if (Battle.Ins != null)
		{
			Battle.Ins.AutoRecycle(Name, base.gameObject);
		}
	}
}
