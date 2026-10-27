using UnityEngine;

public class ParticleUV : MonoBehaviour
{
	private ParticleSystem p;

	public float StartX;

	public float StartY;

	public float EndX;

	public float EndY;

	private float playProgress;

	private float delayTime;

	private void Awake()
	{
		p = base.gameObject.GetComponent<ParticleSystem>();
		p.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(StartX, StartY));
		delayTime = p.startDelay;
	}

	private void OnEnable()
	{
		playProgress = 0f;
		delayTime = p.startDelay;
	}

	private void Update()
	{
		delayTime -= Time.deltaTime;
		if (!(delayTime > 0f))
		{
			playProgress += Time.deltaTime / p.startLifetime;
			if (playProgress > 1f)
			{
				playProgress = 1f;
			}
			Vector2 zero = Vector2.zero;
			zero.x = StartX + (EndX - StartX) * playProgress;
			zero.y = StartY + (EndY - StartY) * playProgress;
			p.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", zero);
		}
	}
}
