using UnityEngine;

public class UVAnimation : MonoBehaviour
{
	public bool pauseTimer;

	public Vector2 uvDefaultTiling = Vector2.one;

	public Vector2 uvDefaultOffset = Vector2.zero;

	public Vector2 uvOffsetSpeed = Vector2.zero;

	public Color matDefaultColor = Color.white;

	private Renderer _renderer;

	private Material _mat;

	private Vector2 m_offset = default(Vector2);

	private Vector2 m_tiling = default(Vector2);

	private void Awake()
	{
		Renderer component = GetComponent<Renderer>();
		if ((bool)component)
		{
			_renderer = component;
		}
		else
		{
			base.enabled = false;
		}
		if (_renderer != null && _renderer.material != null)
		{
			_renderer.material.SetColor("_TintColor", matDefaultColor);
		}
	}

	private void Start()
	{
		if ((bool)_renderer)
		{
			_mat = _renderer.material;
			m_offset = uvDefaultOffset;
			m_tiling = uvDefaultTiling;
			if (_mat != null)
			{
				_mat.mainTextureOffset = m_offset;
				_mat.mainTextureScale = m_tiling;
			}
		}
	}

	private void Update()
	{
		if (uvOffsetSpeed.x != 0f || uvOffsetSpeed.y != 0f)
		{
			m_offset.x += uvOffsetSpeed.x * ((!pauseTimer) ? Time.unscaledDeltaTime : Time.deltaTime);
			if (m_offset.x > 1f)
			{
				m_offset.x -= 1f;
			}
			else if (m_offset.x < -1f)
			{
				m_offset.x += 1f;
			}
			m_offset.y += uvOffsetSpeed.y * ((!pauseTimer) ? Time.unscaledDeltaTime : Time.deltaTime);
			if (m_offset.y > 1f)
			{
				m_offset.y -= 1f;
			}
			else if (m_offset.y < -1f)
			{
				m_offset.y += 1f;
			}
			_mat.mainTextureOffset = m_offset;
		}
	}

	private void OnDestroy()
	{
		if (_mat != null)
		{
			Object.DestroyImmediate(_mat);
		}
	}
}
