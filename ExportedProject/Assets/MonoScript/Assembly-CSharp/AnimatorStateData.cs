using System;
using UnityEngine;

[Serializable]
public class AnimatorStateData
{
	[SerializeField]
	protected string m_Name = "Movement";

	[SerializeField]
	protected float m_TransitionDuration = 0.2f;

	[SerializeField]
	protected float m_SpeedMultiplier = 1f;

	public string Name
	{
		get
		{
			return m_Name;
		}
	}

	public float TransitionDuration
	{
		get
		{
			return m_TransitionDuration;
		}
	}

	public float SpeedMultiplier
	{
		get
		{
			return m_SpeedMultiplier;
		}
	}

	public AnimatorStateData(string name, float transitionDuration)
	{
		m_Name = name;
		m_TransitionDuration = transitionDuration;
		m_SpeedMultiplier = 1f;
	}
}
