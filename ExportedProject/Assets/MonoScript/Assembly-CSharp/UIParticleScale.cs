using System.Collections.Generic;
using UnityEngine;

public class UIParticleScale : MonoBehaviour
{
	private class ScaleData
	{
		public Transform transform;

		public Vector3 beginScale = Vector3.one;
	}

	private List<ScaleData> scaleDatas;

	private void Awake()
	{
		scaleDatas = new List<ScaleData>();
		ParticleSystem[] componentsInChildren = base.transform.GetComponentsInChildren<ParticleSystem>(true);
		foreach (ParticleSystem particleSystem in componentsInChildren)
		{
			scaleDatas.Add(new ScaleData
			{
				transform = particleSystem.transform,
				beginScale = particleSystem.transform.localScale
			});
		}
	}

	private void Start()
	{
		float num = 1920f;
		float num2 = 1080f;
		float num3 = num / num2;
		float num4 = (float)Utils.ScreenWidth / (float)Utils.ScreenHeight;
		foreach (ScaleData scaleData in scaleDatas)
		{
			if (scaleData.transform != null)
			{
				if (num4 < num3)
				{
					float num5 = num4 / num3;
					scaleData.transform.localScale = scaleData.beginScale * num5;
				}
				else
				{
					scaleData.transform.localScale = scaleData.beginScale;
				}
			}
		}
	}
}
