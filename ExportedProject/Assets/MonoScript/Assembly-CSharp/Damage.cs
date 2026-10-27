using System;
using UnityEngine;

[Serializable]
public class Damage
{
	[Tooltip("Apply damage to the Character HP")]
	public int DamageValue = 15;

	[Tooltip("Activated Ragdoll when hit the Character")]
	public bool ActiveRagdoll;

	[HideInInspector]
	public Transform Sender;

	[HideInInspector]
	public long SenderID;

	[HideInInspector]
	public Transform Receiver;

	[HideInInspector]
	public Vector3 HitPosition;

	public Damage(int value)
	{
		DamageValue = value;
	}

	public Damage(Damage damage)
	{
		DamageValue = damage.DamageValue;
		ActiveRagdoll = damage.ActiveRagdoll;
		Sender = damage.Sender;
		SenderID = damage.SenderID;
		Receiver = damage.Receiver;
		HitPosition = damage.HitPosition;
	}

	public void ReduceDamage(float damageReduction)
	{
		int num = (DamageValue = (int)((float)DamageValue - (float)DamageValue * damageReduction / 100f));
	}
}
