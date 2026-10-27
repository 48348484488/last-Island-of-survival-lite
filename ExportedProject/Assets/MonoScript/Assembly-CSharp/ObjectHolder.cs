using UnityEngine;

public class ObjectHolder
{
	public GameObject Go;

	public Transform Trans;

	private bool m_RendererTried;

	private Renderer m_Renderer;

	private bool m_LodTried;

	private SceneLod m_Lod;

	private bool m_BuildingInfoTried;

	private BuildingInfo m_BuildingInfo;

	private bool m_BatchingGroupTried;

	private StaticBatchingGroup m_BatchingGroup;

	private bool m_MeshTried;

	private Mesh m_Mesh;

	public Renderer Renderer
	{
		get
		{
			if (!m_RendererTried)
			{
				m_Renderer = Go.GetComponent<Renderer>();
				m_RendererTried = true;
			}
			return m_Renderer;
		}
	}

	public SceneLod Lod
	{
		get
		{
			if (!m_LodTried)
			{
				m_Lod = Go.GetComponent<SceneLod>();
				m_LodTried = true;
			}
			return m_Lod;
		}
	}

	public BuildingInfo BuildingInfo
	{
		get
		{
			if (!m_BuildingInfoTried)
			{
				m_BuildingInfo = Go.GetComponent<BuildingInfo>();
				m_BuildingInfoTried = true;
			}
			return m_BuildingInfo;
		}
	}

	public StaticBatchingGroup BatchingGroup
	{
		get
		{
			if (!m_BatchingGroupTried)
			{
				m_BatchingGroup = Go.GetComponent<StaticBatchingGroup>();
				m_BatchingGroupTried = true;
			}
			return m_BatchingGroup;
		}
	}

	public Mesh Mesh
	{
		get
		{
			if (!m_MeshTried)
			{
				m_Mesh = Go.GetComponent<MeshFilter>().sharedMesh;
				m_MeshTried = true;
			}
			return m_Mesh;
		}
	}

	public void Show()
	{
		if (Lod != null)
		{
			m_Lod.enabled = true;
		}
		else if (Renderer != null)
		{
			m_Renderer.enabled = true;
		}
	}

	public void Hide()
	{
		if (Lod != null)
		{
			m_Lod.HideAll();
			m_Lod.enabled = false;
		}
		else if (Renderer != null)
		{
			m_Renderer.enabled = false;
		}
	}

	public void Destroy()
	{
		Trans = null;
		m_Renderer = null;
		m_Lod = null;
		m_BuildingInfo = null;
		m_BatchingGroup = null;
		m_RendererTried = false;
		m_LodTried = false;
		m_BuildingInfoTried = false;
		m_BatchingGroupTried = false;
		Object.Destroy(Go);
		Go = null;
	}
}
