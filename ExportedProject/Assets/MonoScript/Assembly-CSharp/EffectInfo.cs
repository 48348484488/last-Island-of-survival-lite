using System;
using System.Collections;
using UnityEngine;

public class EffectInfo : MonoBehaviour
{
	public float lifetime;

	public ParticleSystem root;

	private float mPlayTime;

	private ParticleSystem[] mParticles;

	[NonSerialized]
	public string path;

	private void Awake()
	{
		if (root != null)
		{
			mParticles = GetComponentsInChildren<ParticleSystem>();
		}
	}

	public void Set(float time, uint seed)
	{
		mPlayTime = time;
		if (mParticles != null)
		{
			ParticleSystem[] array = mParticles;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.randomSeed = seed;
			}
		}
	}

	private void OnEnable()
	{
		StopAllCoroutines();
		base.gameObject.SetActive(true);
		if (root != null && mPlayTime > 0f)
		{
			root.Clear(true);
			root.Simulate(mPlayTime, true, true);
			StartCoroutine(SimulateEffect());
		}
		if (lifetime > 0f)
		{
			StartCoroutine(DelayRecycle((!(lifetime > mPlayTime)) ? 0f : (lifetime - mPlayTime)));
		}
	}

	private void OnDisable()
	{
		base.gameObject.SetActive(false);
	}

	private IEnumerator DelayRecycle(float time)
	{
		yield return new WaitForSeconds(time);
		SingletonMono<EffectMgr>.Ins._RecycleEffect(this);
	}

	private IEnumerator SimulateEffect()
	{
		while (lifetime <= 0f || mPlayTime < lifetime)
		{
			yield return null;
			root.Simulate(Time.deltaTime, true, false);
			mPlayTime += Time.deltaTime;
		}
	}
}
