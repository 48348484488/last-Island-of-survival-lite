using UnityEngine;

public class TreeBillboard : MonoBehaviour
{
	private Transform camTransform;

	private MeshRenderer meshRender;

	private void OnEnable()
	{
		TreeBillboardMgr.Ins.RegisTreeBoard(this);
	}

	private void OnDisable()
	{
		TreeBillboardMgr.Ins.UnRegisTreeBoard(this);
	}

	private void Start()
	{
		camTransform = Camera.main.transform;
		meshRender = GetComponent<MeshRenderer>();
	}

	public void CheckBillboard()
	{
		if (meshRender.enabled)
		{
			base.transform.LookAt(camTransform);
		}
	}
}
