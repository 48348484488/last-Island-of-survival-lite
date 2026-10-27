using System.Runtime.CompilerServices;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
	public float Delay;

	private void Start()
	{
		DelayInvoker.DelayInvoke(base.gameObject.GetInstanceID().ToString(), Delay, _003CStart_003Em__0);
	}

	[CompilerGenerated]
	private void _003CStart_003Em__0(object[] o)
	{
		Object.Destroy(base.gameObject);
	}
}
