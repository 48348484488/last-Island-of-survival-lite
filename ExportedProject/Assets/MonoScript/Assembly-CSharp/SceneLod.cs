using UnityEngine;

public class SceneLod : MonoBehaviour
{
	private readonly float[] m_LodDitances = new float[4];

	private readonly Renderer[][] m_LodRenderers = new Renderer[4][];

	private int m_CurrentLodLevel = -1;

	private Transform m_Trans;

	public float Lod0 = 100f;

	public Renderer[] Lod0Renderers;

	[Header("--------------------")]
	public float Lod1 = 200f;

	public Renderer[] Lod1Renderers;

	[Header("--------------------")]
	public float Lod2 = 300f;

	public Renderer[] Lod2Renderers;

	[Header("--------------------")]
	public float Lod3 = 600f;

	public Renderer[] Lod3Renderers;

	public Renderer[] RenderersWithLightmap;

	private void Awake()
	{
		m_Trans = base.transform;
		m_LodDitances[0] = Lod0 * Lod0;
		m_LodDitances[1] = Lod1 * Lod1;
		m_LodDitances[2] = Lod2 * Lod2;
		m_LodDitances[3] = Lod3 * Lod3;
		m_LodRenderers[0] = Lod0Renderers;
		m_LodRenderers[1] = Lod1Renderers;
		m_LodRenderers[2] = Lod2Renderers;
		m_LodRenderers[3] = Lod3Renderers;
		Renderer[][] lodRenderers = m_LodRenderers;
		foreach (Renderer[] array in lodRenderers)
		{
			if (array == null)
			{
				break;
			}
			Renderer[] array2 = array;
			foreach (Renderer renderer in array2)
			{
				if (renderer != null)
				{
					renderer.enabled = false;
				}
			}
		}
		if (m_LodRenderers[2] != null && m_LodRenderers[2].Length > 0 && m_LodRenderers[2][0] != null)
		{
			m_LodRenderers[2][0].enabled = true;
			m_CurrentLodLevel = 2;
		}
		else
		{
			m_CurrentLodLevel = -1;
		}
	}

	public void Resetlod()
	{
		m_LodDitances[2] = Lod2 * Lod2;
		m_LodRenderers[2] = Lod2Renderers;
		Renderer[][] lodRenderers = m_LodRenderers;
		foreach (Renderer[] array in lodRenderers)
		{
			if (array == null)
			{
				break;
			}
			Renderer[] array2 = array;
			foreach (Renderer renderer in array2)
			{
				if (renderer != null)
				{
					renderer.enabled = false;
				}
			}
		}
		Renderer[] array3 = m_LodRenderers[2];
		foreach (Renderer renderer2 in array3)
		{
			renderer2.enabled = true;
		}
		m_CurrentLodLevel = 2;
	}

	private void OnEnable()
	{
		SmallSceneMgr.Ins.RegisSceneLod(this);
	}

	private void OnDisable()
	{
		SmallSceneMgr.Ins.UnRegisSceneLod(this);
	}

	public void DoCheckLod()
	{
		if (!Battle.Ins || Battle.Ins.SelfPlayer == null)
		{
			return;
		}
		Vector3 vector = Battle.Ins.SelfPlayer.Pos - m_Trans.position;
		vector.y *= 1.5f;
		float num = Vector3.SqrMagnitude(vector);
		float num2 = Battle.Ins.MainCamera.Fov / 48f;
		num *= num2 * num2;
		int num3 = -1;
		int i = 0;
		for (int num4 = m_LodRenderers.Length; i < num4; i++)
		{
			Renderer[] array = m_LodRenderers[i];
			if (array == null)
			{
				break;
			}
			if (num < m_LodDitances[i])
			{
				num3 = i;
				break;
			}
		}
		if (num3 == m_CurrentLodLevel)
		{
			return;
		}
		if (m_CurrentLodLevel >= 0)
		{
			Renderer[] array2 = m_LodRenderers[m_CurrentLodLevel];
			Renderer[] array3 = array2;
			foreach (Renderer renderer in array3)
			{
				if (renderer != null)
				{
					renderer.enabled = false;
				}
			}
		}
		m_CurrentLodLevel = num3;
		if (m_CurrentLodLevel < 0)
		{
			return;
		}
		Renderer[] array4 = m_LodRenderers[m_CurrentLodLevel];
		Renderer[] array5 = array4;
		foreach (Renderer renderer2 in array5)
		{
			if (renderer2 != null)
			{
				renderer2.enabled = true;
			}
		}
	}

	public void HideAll()
	{
		Renderer[][] lodRenderers = m_LodRenderers;
		foreach (Renderer[] array in lodRenderers)
		{
			if (array == null)
			{
				break;
			}
			Renderer[] array2 = array;
			foreach (Renderer renderer in array2)
			{
				if (renderer != null)
				{
					renderer.enabled = false;
				}
			}
		}
		m_CurrentLodLevel = -1;
	}
}
