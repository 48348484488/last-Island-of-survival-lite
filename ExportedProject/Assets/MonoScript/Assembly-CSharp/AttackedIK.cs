using System.Collections.Generic;
using UnityEngine;

public class AttackedIK : MonoBehaviour
{
	public const float HitSmooth = 10f;

	public const float BackSmooth = 4.5f;

	public List<Transform> spines;

	private Quaternion maxRot = Quaternion.identity;

	private Quaternion currentRot = Quaternion.identity;

	private bool playing;

	private bool backing;

	public void Init(List<Transform> spines)
	{
		this.spines = spines;
	}

	public void Play(Vector3 force)
	{
		force.y = 1f;
		maxRot = Quaternion.FromToRotation(Vector3.up, force);
		playing = true;
		backing = false;
	}

	public void Stop()
	{
		playing = false;
		maxRot = (currentRot = Quaternion.identity);
	}

	public void DoUpdate()
	{
		if (!playing)
		{
			return;
		}
		if (backing)
		{
			currentRot = Quaternion.Slerp(currentRot, Quaternion.identity, 4.5f * Time.deltaTime);
			if (Quaternion.Angle(currentRot, Quaternion.identity) < 1f)
			{
				playing = false;
			}
		}
		else
		{
			currentRot = Quaternion.Slerp(currentRot, maxRot, 10f * Time.deltaTime);
			if (Quaternion.Angle(currentRot, maxRot) < 1f)
			{
				backing = true;
			}
		}
		SetBonesRotation();
	}

	private void SetBonesRotation()
	{
		foreach (Transform spine in spines)
		{
			spine.rotation = currentRot * spine.rotation;
		}
	}
}
