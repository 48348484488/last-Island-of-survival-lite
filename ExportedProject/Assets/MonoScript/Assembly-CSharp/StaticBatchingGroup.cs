using System.Collections;
using UnityEngine;

public class StaticBatchingGroup : MonoBehaviour
{
	private static float CheckPeriod = 3f;

	private Transform m_Transform;

	private bool m_Visible;

	private bool m_Batched;

	private WaitForSeconds Wait;

	public float ShowDistance = 600f;

	public MeshRenderer[] m_ChildRenderers;

	public Mesh[] m_ChildMeshes;

	public MeshFilter[] m_ChildMeshFilters;

	private void Start()
	{
		Wait = new WaitForSeconds(Random.Range(2.5f, 3.2f));
		StartCoroutine(Check());
	}

	private void OnEnable()
	{
		m_Transform = base.transform;
		m_ChildRenderers = GetComponentsInChildren<MeshRenderer>();
		m_ChildMeshes = new Mesh[m_ChildRenderers.Length];
		m_ChildMeshFilters = new MeshFilter[m_ChildRenderers.Length];
		int i = 0;
		for (int num = m_ChildRenderers.Length; i < num; i++)
		{
			MeshRenderer meshRenderer = m_ChildRenderers[i];
			meshRenderer.enabled = false;
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			m_ChildMeshFilters[i] = component;
			m_ChildMeshes[i] = component.sharedMesh;
		}
		m_Visible = false;
	}

	private IEnumerator Check()
	{
		while (true)
		{
			DoCheck();
			yield return Wait;
		}
	}

	private void DoCheck()
	{
		if (Battle.Ins == null || Battle.Ins.SelfPlayer == null)
		{
			return;
		}
		Vector3 vector = Battle.Ins.SelfPlayer.Pos - m_Transform.position;
		vector.y *= 1.5f;
		float num = Vector3.SqrMagnitude(vector);
		if (num > ShowDistance * ShowDistance)
		{
			if (m_Visible)
			{
				m_Visible = false;
				MeshRenderer[] childRenderers = m_ChildRenderers;
				foreach (MeshRenderer meshRenderer in childRenderers)
				{
					meshRenderer.enabled = false;
				}
			}
		}
		else if (!m_Visible)
		{
			m_Visible = true;
			MeshRenderer[] childRenderers2 = m_ChildRenderers;
			foreach (MeshRenderer meshRenderer2 in childRenderers2)
			{
				meshRenderer2.enabled = true;
			}
			if (!m_Batched && m_ChildMeshFilters.Length > 0)
			{
				m_Batched = true;
				StaticBatchingUtility.Combine(base.gameObject);
			}
		}
	}

	private void OnDisable()
	{
		if (m_Batched)
		{
			Mesh sharedMesh = m_ChildMeshFilters[0].sharedMesh;
			int i = 0;
			for (int num = m_ChildRenderers.Length; i < num; i++)
			{
				MeshRenderer meshRenderer = m_ChildRenderers[i];
				meshRenderer.enabled = false;
				m_ChildMeshFilters[i].sharedMesh = m_ChildMeshes[i];
			}
			Object.DestroyImmediate(sharedMesh, true);
			m_Batched = false;
		}
		m_ChildMeshFilters = null;
		m_ChildRenderers = null;
		m_ChildMeshes = null;
		m_Visible = false;
		m_Batched = false;
	}
}
