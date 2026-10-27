using System;
using System.Collections;
using UnityEngine;
using cfg;
using gs.battle.scmsg;

public class ThrowLei : MonoBehaviour
{
	public int Id;

	private long m_BattleID;

	private ThrowCfg m_LeiCfg;

	private Rigidbody m_Rigidbody;

	private Coroutine m_SyncTick;

	private bool m_throwed;

	private CSyncMapObjectPos syncGrenadeMsg = new CSyncMapObjectPos();

	private float m_DurationTime;

	private bool m_Flying;

	private void Awake()
	{
		BattleEvent.OnCanThrowLei = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnCanThrowLei, new Utils.VoidDelegate(OnCanThrowLei));
		BattleEvent.OnCanFlyLei = (Utils.VoidDelegate)Delegate.Combine(BattleEvent.OnCanFlyLei, new Utils.VoidDelegate(OnCanFlyLei));
		m_throwed = false;
		m_Rigidbody = GetComponent<Rigidbody>();
		base.transform.localPosition = Vector3.zero;
		base.transform.localEulerAngles = Vector3.zero;
		m_Rigidbody.useGravity = false;
		m_Rigidbody.isKinematic = true;
		SThrowGrenade.handler = (SThrowGrenade.Handler)Delegate.Combine(SThrowGrenade.handler, new SThrowGrenade.Handler(OnSThrowGrenade));
		m_DurationTime = 0f;
	}

	private void OnDestroy()
	{
		SThrowGrenade.handler = (SThrowGrenade.Handler)Delegate.Remove(SThrowGrenade.handler, new SThrowGrenade.Handler(OnSThrowGrenade));
		BattleEvent.OnCanThrowLei = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnCanThrowLei, new Utils.VoidDelegate(OnCanThrowLei));
		BattleEvent.OnCanFlyLei = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnCanFlyLei, new Utils.VoidDelegate(OnCanFlyLei));
		StopAllCoroutines();
		CancelInvoke();
	}

	private void OnCanThrowLei()
	{
		m_throwed = true;
		ThrowWeaponState.IsUpLei = true;
		BattleEvent.OnCanThrowLei = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnCanThrowLei, new Utils.VoidDelegate(OnCanThrowLei));
		m_SyncTick = StartCoroutine(Tick());
		Invoke("StopSyncTick", 8f);
		Battle.Ins.SelfPlayer.FSMUpBody.SwitchState(StateID.Shoot);
	}

	private void StopSyncTick()
	{
		StopCoroutine(m_SyncTick);
	}

	private void Start()
	{
		m_Flying = false;
		m_LeiCfg = ThrowCfg.Get(Id);
		if (m_LeiCfg.type == 1)
		{
			Battle.Ins.SyncPlaySound(271);
			StartCoroutine(Boom());
		}
		else if (m_LeiCfg.type == 3)
		{
			StartCoroutine(Yanwu());
		}
	}

	private void Update()
	{
		if (base.transform.parent != null)
		{
			base.transform.localPosition = Vector3.zero;
			base.transform.localEulerAngles = Vector3.zero;
		}
	}

	private void OnCanFlyLei()
	{
		BattleEvent.OnCanFlyLei = (Utils.VoidDelegate)Delegate.Remove(BattleEvent.OnCanFlyLei, new Utils.VoidDelegate(OnCanFlyLei));
		if (m_Rigidbody == null)
		{
			Debug.LogError("lei rigidbody is null");
			return;
		}
		CThrowGrenade cThrowGrenade = new CThrowGrenade();
		cThrowGrenade.id = Id;
		Client2Gs.Ins.Send(cThrowGrenade);
		Vector3 normalized = Battle.Ins.MainCamera.transform.TransformDirection(new Vector3(0f, 0.5f, 1f)).normalized;
		m_Rigidbody.isKinematic = false;
		m_Rigidbody.AddForce(normalized * ((float)m_LeiCfg.speed * (1f + GetAddDis())), ForceMode.VelocityChange);
		m_Rigidbody.AddRelativeTorque(base.transform.forward * 200f, ForceMode.VelocityChange);
		base.transform.parent = null;
		m_Rigidbody.useGravity = true;
		m_Flying = true;
	}

	private void OnSThrowGrenade(SThrowGrenade msg)
	{
		if (msg.playerInsId == Battle.Ins.SelfPlayer.InsId)
		{
			m_BattleID = msg.grenadeInsId;
		}
	}

	private IEnumerator Boom()
	{
		yield return new WaitForSeconds(3f);
		if (!m_throwed)
		{
			OnCanThrowLei();
		}
		yield return new WaitForSeconds(1f);
		base.transform.parent = null;
		m_Rigidbody.useGravity = true;
		CSyncMapObjectPos c = new CSyncMapObjectPos
		{
			insId = m_BattleID,
			pos = 
			{
				x = base.transform.position.x,
				y = base.transform.position.y,
				z = base.transform.position.z
			}
		};
		Client2Gs.Ins.Send(c);
		Battle.Ins.SyncPlaySound(m_LeiCfg.boomSound);
		Battle.Ins.PlayEffectAtWorldPos(m_LeiCfg.boomEffect, base.transform.position, Vector3.zero);
		Collider[] collider = Physics.OverlapSphere(base.transform.position, m_LeiCfg.radius);
		CShellExplode cShellExplode = new CShellExplode
		{
			pos = 
			{
				x = base.transform.position.x,
				y = base.transform.position.y,
				z = base.transform.position.z
			},
			itemId = m_LeiCfg.id
		};
		BombMgr.ExplosionDamage(m_LeiCfg.radius, base.transform.position, cShellExplode.damagedList);
		Client2Gs.Ins.Send(cShellExplode);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private bool CheckThroughthings(GameObject go)
	{
		Vector3 normalized = (go.transform.position + Vector3.up - base.transform.position + Vector3.up * 0.2f).normalized;
		Ray ray = new Ray(base.transform.position + Vector3.up * 0.2f, normalized);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 6f, (1 << go.layer) | (1 << BattleScMgr.DefaultLayer)) && hitInfo.collider.gameObject.layer == go.layer)
		{
			return true;
		}
		return false;
	}

	private IEnumerator Yanwu()
	{
		yield return new WaitForSeconds(3f);
		if (!m_throwed)
		{
			OnCanThrowLei();
		}
		yield return new WaitForSeconds(1f);
		base.transform.parent = null;
		m_Rigidbody.useGravity = true;
		m_Rigidbody.isKinematic = false;
		Battle.Ins.SyncPlaySound(m_LeiCfg.boomSound);
		Battle.Ins.PlayEffectAtWorldPos(m_LeiCfg.boomEffect, base.transform.position, Vector3.zero);
		yield return new WaitForSeconds(45f);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator Tick()
	{
		while (true)
		{
			syncGrenadeMsg.pos.x = base.transform.position.x;
			syncGrenadeMsg.pos.y = base.transform.position.y;
			syncGrenadeMsg.pos.z = base.transform.position.z;
			syncGrenadeMsg.rotation.x = base.transform.eulerAngles.x;
			syncGrenadeMsg.rotation.y = base.transform.eulerAngles.y;
			syncGrenadeMsg.rotation.z = base.transform.eulerAngles.z;
			syncGrenadeMsg.insId = m_BattleID;
			Client2Gs.Ins.Send(syncGrenadeMsg);
			CheckLuoDi();
			m_DurationTime += Time.deltaTime;
			if (m_DurationTime > 9f)
			{
				break;
			}
			yield return new WaitForSeconds(0.04f);
		}
	}

	private void CheckLuoDi()
	{
		if (m_Flying && Battle.Ins.GetAwayGroundDistance(base.transform.position) < 0.1f)
		{
			Battle.Ins.SyncPlaySound(273);
			m_Flying = false;
		}
	}

	private float GetAddDis()
	{
		return Battle.Ins.SelfPlayer.GetSkillValueIndex0(133);
	}
}
