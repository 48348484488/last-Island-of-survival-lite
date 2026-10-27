using System;
using UnityEngine;
using cfg;

public class CommonControllerFunc : MonoBehaviour
{
	private Rigidbody m_Rigidbody;

	private CapsuleCollider m_Collider;

	private PlayerController m_PlayerController;

	private OtherPlayerController m_OtherPlayerController;

	private void Awake()
	{
		m_Rigidbody = GetComponent<Rigidbody>();
		GameObject gameObject = GameObject.Find(base.gameObject.name + "/Collider");
		if ((bool)gameObject)
		{
			m_Collider = gameObject.GetComponent<CapsuleCollider>();
		}
	}

	private void Start()
	{
		m_PlayerController = GetComponent<PlayerController>();
		m_OtherPlayerController = GetComponent<OtherPlayerController>();
	}

	public void StartCross()
	{
		if (!(m_PlayerController == null))
		{
			m_Rigidbody.useGravity = false;
			m_Collider.height = 0f;
			m_Collider.center = new Vector3(0f, 1.48f, 0.1f);
			m_Collider.radius = 0.2f;
		}
	}

	public void StopCross()
	{
		if (!(m_PlayerController == null))
		{
			m_Rigidbody.useGravity = true;
			m_Collider.height = 0.5f;
			m_Collider.center = new Vector3(0f, 0.7f, 0.1f);
			m_Collider.radius = 0.4f;
		}
	}

	public void SyncFootTexture(float value)
	{
		if (!(m_PlayerController != null) && m_OtherPlayerController != null && Battle.Ins.NearOtherPlayersDic.ContainsKey(m_OtherPlayerController.RoleId))
		{
			PlayFootTexture(m_OtherPlayerController.FootTexture, m_OtherPlayerController.Pos, false);
			if (BattleEvent.roleFootSoundEvent != null)
			{
				BattleEvent.roleFootSoundEvent(m_OtherPlayerController.gameObject, m_OtherPlayerController.RoleId, 323);
			}
		}
	}

	private void PlayFootTexture(BasePlayerController.FootTextureType footTextureType, Vector3 pos, bool isSelf)
	{
		int num = 320;
		switch (footTextureType)
		{
		case BasePlayerController.FootTextureType.Building:
			num = 323;
			break;
		case BasePlayerController.FootTextureType.Ground:
			num = 319;
			break;
		case BasePlayerController.FootTextureType.Road:
			num = 162;
			break;
		case BasePlayerController.FootTextureType.Water:
			num = 322;
			break;
		case BasePlayerController.FootTextureType.Grass:
			num = 320;
			break;
		default:
			num = 320;
			break;
		}
		if (isSelf)
		{
			SingletonMono<AudioManager>.Ins.Play(num, pos);
		}
		else
		{
			Battle.Ins.SelfPlayer.SoundControll.PlaySound(num, Singleton<BattleScMgr>.Ins.GetAudioPercent(2, Vector3.Distance(Battle.Ins.SelfPlayer.Pos, pos)));
		}
	}

	public void SyncSwimHandEffect(int value)
	{
		if (m_PlayerController != null)
		{
			if (value == 1)
			{
				SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos(EffectCfg.Get(11).path, m_PlayerController.LeftHand.position);
			}
			else
			{
				SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos(EffectCfg.Get(11).path, m_PlayerController.RightHand.position);
			}
		}
		else if (m_OtherPlayerController != null)
		{
			if (value == 1)
			{
				SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos(EffectCfg.Get(11).path, m_OtherPlayerController.LeftHand.position);
			}
			else
			{
				SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos(EffectCfg.Get(11).path, m_OtherPlayerController.RightHand.position);
			}
		}
	}

	public void EnableNearWeapon()
	{
		if (m_PlayerController != null && m_PlayerController.HaveNearWeaponInHand)
		{
			(m_PlayerController.GetCurrentWeapon() as NearWeapon).Enable();
		}
	}

	public void DoHandAttack(int leftHand)
	{
		try
		{
			if (!(m_PlayerController == null) && !(m_PlayerController.LeftHand == null) && !(m_PlayerController.RightHand == null))
			{
				HandAttack handAttack = ((leftHand != 0) ? m_PlayerController.RightHand.GetComponent<HandAttack>() : m_PlayerController.LeftHand.GetComponent<HandAttack>());
				if (handAttack != null)
				{
					handAttack.OnTriggerEnter1();
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Do HandAttack error" + ex);
		}
	}

	public void CloseNearWeapon()
	{
		if (m_PlayerController != null && m_PlayerController.HaveNearWeaponInHand)
		{
			(m_PlayerController.GetCurrentWeapon() as NearWeapon).Close();
		}
	}
}
