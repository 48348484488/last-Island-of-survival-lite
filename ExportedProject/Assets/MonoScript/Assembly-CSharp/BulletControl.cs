using EasyBuildSystem.Runtimes.Internal.Part;
using SC;
using UnityEngine;
using cfg;
using gs.battle.monster.scmsg;
using gs.battle.scmsg;

public class BulletControl : MonoBehaviour
{
	public const int HitLayer = 1847363585;

	private static readonly CBulletHitPlayer cSelfHitPlayerMsg = new CBulletHitPlayer();

	private static readonly COtherBulletHitPlayer cOtherHitPlayerMsg = new COtherBulletHitPlayer();

	private static readonly CBulletHitNoLiveObj cSelfHitNotLiveMsg = new CBulletHitNoLiveObj();

	private static readonly COtherBulletHitNoLiveObj cOtherHitNotLiveMsg = new COtherBulletHitNoLiveObj();

	private static readonly CBulletMiss cBulletMissMsg = new CBulletMiss();

	private static readonly COtherBulletMiss cOtherBulletMissMsg = new COtherBulletMiss();

	private static readonly CShellExplode cShellExplode = new CShellExplode();

	private static readonly CBulletHitNpc cBulletHitNpc = new CBulletHitNpc();

	private static readonly Color DebugColor = Color.green;

	private static readonly bool IsDebug = false;

	public Rigidbody BulletRigidbody;

	private RaycastHit hitInfo;

	private Vector3 m_ShootPostion;

	private Vector3 m_PreviousPosition;

	private int m_HitLayer;

	private long m_shooterInsId;

	private bool IsHit;

	public bool IsRpgBullet;

	public int ItemId;

	private int m_gunId;

	private bool m_needMiss;

	private bool m_PassWater;

	public bool isFromSelfGun
	{
		get
		{
			if (Battle.Ins.SelfPlayer == null)
			{
				return false;
			}
			return Battle.Ins.SelfPlayer.InsId == m_shooterInsId;
		}
	}

	private void Awake()
	{
		BulletRigidbody = GetComponent<Rigidbody>();
	}

	public void Fire(int gunId, long shooterInsId, Vector3 shootPosition, Vector3 dir, Vector3 vel, int hitLayerMask = 1847363585, bool needMiss = false)
	{
		IsHit = false;
		m_PassWater = false;
		m_gunId = gunId;
		m_needMiss = needMiss;
		base.gameObject.SetActiveBetter(true);
		m_shooterInsId = shooterInsId;
		base.transform.position = shootPosition;
		base.transform.rotation = Quaternion.LookRotation(dir);
		m_ShootPostion = shootPosition;
		m_PreviousPosition = m_ShootPostion - dir * 0.6f;
		GunCfg gunCfg = GunCfg.Get(gunId);
		if (gunCfg.bulletDown)
		{
			BulletRigidbody.useGravity = true;
		}
		else
		{
			BulletRigidbody.useGravity = false;
		}
		BulletRigidbody.AddForce(vel, ForceMode.VelocityChange);
		m_HitLayer = hitLayerMask;
	}

	private void Update()
	{
		if (IsDebug)
		{
			Debug.DrawLine(base.transform.position, m_PreviousPosition, DebugColor, 3f);
		}
		if (Physics.Linecast(m_PreviousPosition, base.transform.position, out hitInfo, m_HitLayer))
		{
			if (m_needMiss)
			{
				Battle.Ins.BulletPool.Recycle(this);
				SendMissMsg();
				return;
			}
			GameObject gameObject = hitInfo.collider.gameObject;
			if (gameObject.layer == BattleScMgr.WindowLayer)
			{
				m_PreviousPosition = base.transform.position;
				return;
			}
			if (IsRpgBullet)
			{
				ThrowCfg throwCfg = ThrowCfg.Get(ItemId);
				Battle.Ins.PlayEffectAtWorldPos(39, hitInfo.point, base.transform.forward);
				cShellExplode.itemId = ItemId;
				cShellExplode.pos.x = hitInfo.point.x;
				cShellExplode.pos.y = hitInfo.point.y;
				cShellExplode.pos.z = hitInfo.point.z;
				BombMgr.ExplosionDamage(throwCfg.radius, hitInfo.point, cShellExplode.damagedList);
				Client2Gs.Ins.Send(cShellExplode);
				BulletRigidbody.velocity = Vector3.zero;
				Object.Destroy(base.gameObject);
			}
			else
			{
				if (gameObject.layer == BattleScMgr.PlayerLayer || gameObject.layer == BattleScMgr.SelfPlayerLayer)
				{
					BasePlayerController componentInParent = gameObject.GetComponentInParent<BasePlayerController>();
					if (!componentInParent)
					{
						BulletRigidbody.velocity = Vector3.zero;
						Battle.Ins.BulletPool.Recycle(this);
						return;
					}
					if (componentInParent.InsId == m_shooterInsId)
					{
						m_PreviousPosition = base.transform.position;
						return;
					}
					if (componentInParent.IsDie)
					{
						Rigidbody rigidbody = hitInfo.rigidbody;
						if ((bool)rigidbody)
						{
							rigidbody.AddForce(base.transform.forward * 5f, ForceMode.Impulse);
						}
						BulletRigidbody.velocity = Vector3.zero;
						Battle.Ins.BulletPool.Recycle(this);
						return;
					}
					PartType component = hitInfo.collider.transform.GetComponent<PartType>();
					if ((bool)component)
					{
						if (gameObject.layer == BattleScMgr.PlayerLayer)
						{
							(componentInParent as OtherPlayerController).PlayAttackIk(base.transform.forward, 0.1f);
						}
						PlayHitSound(component.partType, componentInParent);
						Vector3 normalized = (hitInfo.point - m_ShootPostion).normalized;
						Vector3 normalized2 = Vector3.Reflect(normalized, hitInfo.normal).normalized;
						Vector3 vector = hitInfo.collider.transform.InverseTransformPoint(hitInfo.point);
						if (isFromSelfGun)
						{
							cSelfHitPlayerMsg.roleId = componentInParent.PlayerInfo.roleId;
							cSelfHitPlayerMsg.bodyPart = (byte)component.partType;
							cSelfHitPlayerMsg.forward.x = MathUtils.Float2Short(normalized.x);
							cSelfHitPlayerMsg.forward.y = MathUtils.Float2Short(normalized.y);
							cSelfHitPlayerMsg.forward.z = MathUtils.Float2Short(normalized.z);
							cSelfHitPlayerMsg.hitPos.x = MathUtils.Float2Short(vector.x);
							cSelfHitPlayerMsg.hitPos.y = MathUtils.Float2Short(vector.y);
							cSelfHitPlayerMsg.hitPos.z = MathUtils.Float2Short(vector.z);
							Client2Gs.Ins.Send(cSelfHitPlayerMsg);
							SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos("effect/dk_jianxue_xiao.ab", hitInfo.point, normalized2);
							if (BattleEvent.OnHitOtherPlayer != null)
							{
								Utils.TriggerEvent(BattleEvent.OnHitOtherPlayer);
							}
						}
						else
						{
							cOtherHitPlayerMsg.roleId = componentInParent.PlayerInfo.roleId;
							cOtherHitPlayerMsg.bodyPart = (byte)component.partType;
							cOtherHitPlayerMsg.forward.x = MathUtils.Float2Short(normalized.x);
							cOtherHitPlayerMsg.forward.y = MathUtils.Float2Short(normalized.y);
							cOtherHitPlayerMsg.forward.z = MathUtils.Float2Short(normalized.z);
							cOtherHitPlayerMsg.hitPos.x = MathUtils.Float2Short(vector.x);
							cOtherHitPlayerMsg.hitPos.y = MathUtils.Float2Short(vector.y);
							cOtherHitPlayerMsg.hitPos.z = MathUtils.Float2Short(vector.z);
							cOtherHitPlayerMsg.otherInsId = m_shooterInsId;
							Client2Gs.Ins.Send(cOtherHitPlayerMsg);
						}
						IsHit = true;
					}
				}
				else if (gameObject.layer == BattleScMgr.CarForBulletLayer)
				{
					IsHit = true;
				}
				else
				{
					MapObject componentInParent2 = gameObject.GetComponentInParent<MapObject>();
					if ((bool)componentInParent2)
					{
						if (componentInParent2.InsId == m_shooterInsId)
						{
							m_PreviousPosition = base.transform.position;
							return;
						}
						if (componentInParent2 is MonsterController)
						{
							if (componentInParent2.InsId == m_shooterInsId)
							{
								m_PreviousPosition = base.transform.position;
								return;
							}
							PartType component2 = gameObject.GetComponent<PartType>();
							if ((bool)component2)
							{
								Vector3 normalized3 = (hitInfo.point - m_ShootPostion).normalized;
								Vector3 normalized4 = Vector3.Reflect(normalized3, hitInfo.normal).normalized;
								Vector3 vector2 = hitInfo.collider.transform.InverseTransformPoint(hitInfo.point);
								if (isFromSelfGun)
								{
									cBulletHitNpc.instanceId = componentInParent2.InsId;
									cBulletHitNpc.bodyPart = (byte)component2.partType;
									cBulletHitNpc.forward.x = MathUtils.Float2Short(normalized3.x);
									cBulletHitNpc.forward.y = MathUtils.Float2Short(normalized3.y);
									cBulletHitNpc.forward.z = MathUtils.Float2Short(normalized3.z);
									cBulletHitNpc.hitPos.x = MathUtils.Float2Short(vector2.x);
									cBulletHitNpc.hitPos.y = MathUtils.Float2Short(vector2.y);
									cBulletHitNpc.hitPos.z = MathUtils.Float2Short(vector2.z);
									Client2Gs.Ins.Send(cBulletHitNpc);
								}
								else
								{
									cOtherHitPlayerMsg.roleId = componentInParent2.InsId;
									cOtherHitPlayerMsg.bodyPart = (byte)component2.partType;
									cOtherHitPlayerMsg.forward.x = MathUtils.Float2Short(normalized3.x);
									cOtherHitPlayerMsg.forward.y = MathUtils.Float2Short(normalized3.y);
									cOtherHitPlayerMsg.forward.z = MathUtils.Float2Short(normalized3.z);
									cOtherHitPlayerMsg.hitPos.x = MathUtils.Float2Short(vector2.x);
									cOtherHitPlayerMsg.hitPos.y = MathUtils.Float2Short(vector2.y);
									cOtherHitPlayerMsg.hitPos.z = MathUtils.Float2Short(vector2.z);
									cOtherHitPlayerMsg.otherInsId = m_shooterInsId;
									Client2Gs.Ins.Send(cOtherHitPlayerMsg);
								}
								SingletonMono<EffectMgr>.Ins.PlayEffectAtWorldPos("effect/dk_jianxue_xiao.ab", hitInfo.point, normalized4);
							}
						}
						else
						{
							if (componentInParent2 is PartBehaviour && (componentInParent2 as PartBehaviour).CurrentState == StateType.Preview)
							{
								m_PreviousPosition = base.transform.position;
								return;
							}
							if (isFromSelfGun)
							{
								cSelfHitNotLiveMsg.insId = componentInParent2.InsId;
								Client2Gs.Ins.Send(cSelfHitNotLiveMsg);
							}
							else
							{
								cOtherHitNotLiveMsg.instanceId = componentInParent2.InsId;
								cOtherHitNotLiveMsg.otherInsId = m_shooterInsId;
								Client2Gs.Ins.Send(cOtherHitNotLiveMsg);
							}
							ShowDecal(hitInfo, m_ShootPostion);
						}
						IsHit = true;
					}
					else
					{
						ShowDecal(hitInfo, m_ShootPostion);
					}
				}
				BulletRigidbody.velocity = Vector3.zero;
				Battle.Ins.BulletPool.Recycle(this);
				SendMissMsg();
			}
		}
		if (Physics.Linecast(m_PreviousPosition, base.transform.position + base.transform.forward * 0.5f, out hitInfo, 1 << BattleScMgr.WaterLayer))
		{
			m_PassWater = true;
			Vector3 inDirection = hitInfo.point - m_ShootPostion;
			Vector3 forword = Vector3.Reflect(inDirection, hitInfo.normal);
			Battle.Ins.PlayEffectAtWorldPos(20, hitInfo.point, forword);
		}
		m_PreviousPosition = base.transform.position;
	}

	private void SendMissMsg()
	{
		if (!IsHit)
		{
			if (isFromSelfGun)
			{
				Client2Gs.Ins.Send(cBulletMissMsg);
				return;
			}
			cOtherBulletMissMsg.otherInsId = m_shooterInsId;
			Client2Gs.Ins.Send(cOtherBulletMissMsg);
		}
	}

	private void OnDisable()
	{
		SendMissMsg();
	}

	private void ShowDecal(RaycastHit hit, Vector3 FireGoPos)
	{
		if (hit.collider.CompareTag("TieSiWang"))
		{
			return;
		}
		Quaternion rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
		Vector3 inDirection = hit.point - FireGoPos;
		Vector3 forword = Vector3.Reflect(inDirection, hit.normal);
		Decal.DecalType decalType = Decal.DecalType.Default;
		int num = 12;
		if (hit.collider.gameObject.layer == BattleScMgr.CarForBulletLayer || hit.collider.gameObject.layer == BattleScMgr.StopBulletLayer)
		{
			num = 6;
			decalType = Decal.DecalType.JinShu;
		}
		else if (hit.collider.CompareTag("road") || hit.collider.CompareTag("builder") || hit.collider.CompareTag("Shitou") || hit.collider.CompareTag("Shitou2"))
		{
			num = 15;
			decalType = Decal.DecalType.Default;
		}
		else if (hit.collider.CompareTag("Door") || hit.collider.CompareTag("tree"))
		{
			num = 14;
			decalType = Decal.DecalType.Wood;
		}
		else if (hit.collider.CompareTag("Ground") || hit.transform.gameObject.layer == 18)
		{
			if (m_PassWater)
			{
				num = -1;
				decalType = Decal.DecalType.Null;
			}
			else
			{
				num = 5;
				decalType = Decal.DecalType.Default;
			}
		}
		else
		{
			num = 12;
			decalType = Decal.DecalType.Default;
		}
		Battle.Ins.PlayEffectAtWorldPos(num, hit.point, forword);
		Decal decal = Battle.Ins.DecalPool.Get();
		if ((bool)decal)
		{
			decal.SetDecal(decalType);
			decal.transform.SetParent(hit.collider.transform);
			decal.transform.position = hit.point;
			decal.transform.rotation = rotation;
			if (decal.transform.localScale != Vector3.one)
			{
				decal.transform.localScale = new Vector3(1f / decal.transform.parent.transform.localScale.x, 1f / decal.transform.parent.transform.localScale.y, 1f / decal.transform.parent.transform.localScale.z);
			}
			else
			{
				decal.transform.localScale = Vector3.one;
			}
			decal.gameObject.SetActive(true);
		}
	}

	public void PlayHitSound(int bodyType, BasePlayerController hitTarget)
	{
		switch (bodyType)
		{
		case 9:
		{
			ItemCfg clothType2 = hitTarget.GetClothType(33);
			if (clothType2 != null)
			{
				SingletonMono<AudioManager>.Ins.Play(108, base.transform.position);
			}
			else
			{
				SingletonMono<AudioManager>.Ins.Play(106, base.transform.position);
			}
			break;
		}
		case 8:
		{
			ItemCfg clothType = hitTarget.GetClothType(22);
			if (clothType != null)
			{
				SingletonMono<AudioManager>.Ins.Play(107, base.transform.position);
			}
			else
			{
				SingletonMono<AudioManager>.Ins.Play(106, base.transform.position);
			}
			break;
		}
		case 0:
		case 1:
			SingletonMono<AudioManager>.Ins.Play(106, base.transform.position);
			break;
		case 4:
		case 5:
			SingletonMono<AudioManager>.Ins.Play(106, base.transform.position);
			break;
		case 2:
		case 3:
			SingletonMono<AudioManager>.Ins.Play(106, base.transform.position);
			break;
		case 6:
		case 7:
			SingletonMono<AudioManager>.Ins.Play(106, base.transform.position);
			break;
		}
	}
}
