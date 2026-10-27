using UnityEngine;

public class TrailRenderReset : MonoBehaviour
{
	[HideInInspector]
	public TrailRenderer trailRenderer;

	[HideInInspector]
	public float _startWidth;

	[HideInInspector]
	public float _endWidth;

	private void Awake()
	{
		trailRenderer = GetComponent<TrailRenderer>();
		_startWidth = trailRenderer.startWidth;
		_endWidth = trailRenderer.endWidth;
	}

	private void OnEnable()
	{
		trailRenderer.startWidth = _startWidth;
		trailRenderer.endWidth = _endWidth;
	}

	private void OnDisable()
	{
		trailRenderer.startWidth = 0f;
		trailRenderer.endWidth = 0f;
		trailRenderer.Clear();
	}
}
