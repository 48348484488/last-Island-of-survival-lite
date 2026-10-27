using UnityEngine;

public class T4MLodObjSC : MonoBehaviour
{
	public Renderer LOD1;

	public Renderer LOD2;

	public Renderer LOD3;

	[HideInInspector]
	public float Interval = 0.5f;

	[HideInInspector]
	public Transform PlayerCamera;

	[HideInInspector]
	public int Mode;

	private Vector3 OldPlayerPos;

	[HideInInspector]
	public int ObjLodStatus;

	[HideInInspector]
	public float MaxViewDistance = 60f;

	[HideInInspector]
	public float LOD2Start = 20f;

	[HideInInspector]
	public float LOD3Start = 40f;

	public void ActivateLODScrpt()
	{
		if (Mode == 2)
		{
			if (PlayerCamera == null)
			{
				PlayerCamera = Camera.main.transform;
			}
			InvokeRepeating("AFLODScrpt", Random.Range(0f, Interval), Interval);
		}
	}

	public void ActivateLODLay()
	{
		if (Mode == 2)
		{
			if (PlayerCamera == null)
			{
				PlayerCamera = Camera.main.transform;
			}
			InvokeRepeating("AFLODLay", Random.Range(0f, Interval), Interval);
		}
	}

	public void AFLODLay()
	{
		if (OldPlayerPos == PlayerCamera.position)
		{
			return;
		}
		OldPlayerPos = PlayerCamera.position;
		float num = Vector3.Distance(new Vector3(base.transform.position.x, PlayerCamera.position.y, base.transform.position.z), PlayerCamera.position);
		int layer = base.gameObject.layer;
		if (num <= PlayerCamera.GetComponent<Camera>().layerCullDistances[layer] + 5f)
		{
			if (num < LOD2Start && ObjLodStatus != 1)
			{
				Renderer lOD = LOD3;
				bool flag = false;
				LOD2.enabled = flag;
				lOD.enabled = flag;
				LOD1.enabled = true;
				ObjLodStatus = 1;
			}
			else if (num >= LOD2Start && num < LOD3Start && ObjLodStatus != 2)
			{
				Renderer lOD2 = LOD1;
				bool flag = false;
				LOD3.enabled = flag;
				lOD2.enabled = flag;
				LOD2.enabled = true;
				ObjLodStatus = 2;
			}
			else if (num >= LOD3Start && ObjLodStatus != 3)
			{
				Renderer lOD3 = LOD1;
				bool flag = false;
				LOD2.enabled = flag;
				lOD3.enabled = flag;
				LOD3.enabled = true;
				ObjLodStatus = 3;
			}
		}
	}

	public void AFLODScrpt()
	{
		if (OldPlayerPos == PlayerCamera.position)
		{
			return;
		}
		OldPlayerPos = PlayerCamera.position;
		float num = Vector3.Distance(new Vector3(base.transform.position.x, PlayerCamera.position.y, base.transform.position.z), PlayerCamera.position);
		if (num <= MaxViewDistance)
		{
			if (num < LOD2Start && ObjLodStatus != 1)
			{
				Renderer lOD = LOD3;
				bool flag = false;
				LOD2.enabled = flag;
				lOD.enabled = flag;
				LOD1.enabled = true;
				ObjLodStatus = 1;
			}
			else if (num >= LOD2Start && num < LOD3Start && ObjLodStatus != 2)
			{
				Renderer lOD2 = LOD1;
				bool flag = false;
				LOD3.enabled = flag;
				lOD2.enabled = flag;
				LOD2.enabled = true;
				ObjLodStatus = 2;
			}
			else if (num >= LOD3Start && ObjLodStatus != 3)
			{
				Renderer lOD3 = LOD1;
				bool flag = false;
				LOD2.enabled = flag;
				lOD3.enabled = flag;
				LOD3.enabled = true;
				ObjLodStatus = 3;
			}
		}
		else if (ObjLodStatus != 0)
		{
			Renderer lOD4 = LOD1;
			bool flag = false;
			LOD3.enabled = flag;
			flag = flag;
			LOD2.enabled = flag;
			lOD4.enabled = flag;
			ObjLodStatus = 0;
		}
	}
}
