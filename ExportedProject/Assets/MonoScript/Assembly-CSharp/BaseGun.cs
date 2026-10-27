using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;

public class BaseGun : Weapon
{
	[CompilerGenerated]
	private sealed class _003C_LoadPartModel_003Ec__AnonStorey0
	{
		internal ItemCfg partCfg;

		internal BaseGun _0024this;

		internal void _003C_003Em__0(GameObject o)
		{
			if (_0024this.Parts == null || _0024this.m_partGameObjects == null)
			{
				UnityEngine.Object.Destroy(o);
				return;
			}
			if (!_0024this.Parts.Contains(partCfg.id) || _0024this.m_partGameObjects.ContainsKey(partCfg.type))
			{
				UnityEngine.Object.Destroy(o);
				return;
			}
			_0024this.m_partGameObjects.Add(partCfg.type, o);
			o.transform.SetParent(_0024this.m_partAnchors[partCfg.type], false);
			o.SetActiveBetter(_0024this.m_partVisible);
		}
	}

	protected GunCfg m_gunCfg;

	public GunInfo GunInfo;

	protected Dictionary<int, Transform> m_partAnchors;

	public HashSet<int> Parts;

	protected Dictionary<int, GameObject> m_partGameObjects;

	protected bool m_partVisible = true;

	public int BulletMax;

	public int CurBulletNum3Rd;

	public string Name
	{
		get
		{
			return m_gunCfg.name;
		}
	}

	public override bool Visible
	{
		get
		{
			return base.Visible;
		}
		set
		{
			base.Visible = value;
			PartVisible = m_partVisible;
		}
	}

	public bool PartVisible
	{
		get
		{
			return m_partVisible;
		}
		set
		{
			m_partVisible = value;
			if (m_visible && m_partVisible)
			{
				ShowAllParts();
			}
			else
			{
				HideAllParts();
			}
		}
	}

	public GunCfg GunCfg
	{
		get
		{
			return m_gunCfg;
		}
	}

	public bool HaveJing
	{
		get
		{
			return GetPartCfg(14) != null;
		}
	}

	public BaseGun(BasePlayerController owner, int gunId, int skinid, HashSet<int> parts)
		: base(owner, gunId, skinid)
	{
		m_gunCfg = GunCfg.Get(gunId);
		Parts = new HashSet<int>(parts);
		m_partGameObjects = new Dictionary<int, GameObject>();
	}

	public override void SetWeaponGameObject(GameObject gunGo)
	{
		base.SetWeaponGameObject(gunGo);
		WeaponObject.name = m_gunCfg.name;
		GunInfo = WeaponObject.GetComponent<GunInfo>();
		m_partAnchors = new Dictionary<int, Transform>
		{
			{ 14, GunInfo.Miaojiu },
			{ 20, GunInfo.Danjia },
			{ 18, GunInfo.Muzzle },
			{ 19, GunInfo.Chuizhiwoba }
		};
		if (m_visible && m_partVisible)
		{
			foreach (int part in Parts)
			{
				ItemCfg itemCfg = ItemCfg.Get(part);
				if (itemCfg == null)
				{
					Debug.LogError("[BaseGun]wrong part id " + part);
				}
				else
				{
					_LoadPartModel(itemCfg);
				}
			}
		}
		if (BattleEvent.OnGunModeLoadFinishDelegate != null)
		{
			BattleEvent.OnGunModeLoadFinishDelegate(this);
		}
	}

	private void ShowAllParts()
	{
		if (WeaponObject == null)
		{
			return;
		}
		foreach (int part in Parts)
		{
			ItemCfg itemCfg = ItemCfg.Get(part);
			if (itemCfg != null)
			{
				GameObject value;
				if (m_partGameObjects.TryGetValue(itemCfg.type, out value))
				{
					value.SetActiveBetter(true);
				}
				else
				{
					_LoadPartModel(itemCfg);
				}
			}
		}
	}

	private void HideAllParts()
	{
		if (WeaponObject == null)
		{
			return;
		}
		foreach (KeyValuePair<int, GameObject> partGameObject in m_partGameObjects)
		{
			partGameObject.Value.SetActiveBetter(false);
		}
	}

	public ItemCfg GetPartCfg(int partType)
	{
		foreach (int part in Parts)
		{
			ItemCfg itemCfg = ItemCfg.Get(part);
			if (itemCfg.type == partType)
			{
				return itemCfg;
			}
		}
		return null;
	}

	public void PlayShootEffectAndSound(bool haveFireEffect, bool haveSound)
	{
		if (GunInfo != null)
		{
			EffectCfg effectCfg = EffectCfg.Get(m_gunCfg.effects[(!haveFireEffect) ? 1 : 0]);
			SingletonMono<EffectMgr>.Ins.PlayEffect(effectCfg.path, GunInfo.Muzzle);
		}
		float num = Vector3.Distance(Battle.Ins.SelfPlayer.Pos, Owner.Pos);
		if (haveSound)
		{
			Owner.SoundControll.PlaySound(m_gunCfg.audios[(!(num < 100f)) ? 1 : 0], Singleton<BattleScMgr>.Ins.GetAudioPercent(1, num));
		}
		else
		{
			Owner.SoundControll.PlaySound(m_gunCfg.audios[2], Singleton<BattleScMgr>.Ins.GetAudioPercent(1, num));
		}
	}

	public void AddPart(int partId)
	{
		ItemCfg itemCfg = ItemCfg.Get(partId);
		_RemovePart(itemCfg.type);
		Parts.Add(partId);
		OnAddPart(partId);
		if (WeaponObject != null)
		{
			_LoadPartModel(ItemCfg.Get(partId));
		}
	}

	public void RemovePart(int partId)
	{
		if (Parts.Contains(partId))
		{
			_RemovePart(ItemCfg.Get(partId).type);
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		m_gunCfg = null;
		GunInfo = null;
		m_partAnchors = null;
		Parts = null;
		m_partGameObjects = null;
	}

	protected virtual void OnRemovePart(int partId)
	{
	}

	protected virtual void OnAddPart(int partId)
	{
	}

	private void _RemovePart(int partType)
	{
		int num = -1;
		foreach (int part in Parts)
		{
			if (ItemCfg.Get(part).type == partType)
			{
				num = part;
				break;
			}
		}
		if (num >= 0)
		{
			Parts.Remove(num);
			OnRemovePart(num);
		}
		GameObject value;
		if (m_partGameObjects.TryGetValue(partType, out value))
		{
			m_partGameObjects.Remove(partType);
			UnityEngine.Object.Destroy(value);
		}
	}

	private void _LoadPartModel(ItemCfg partCfg)
	{
		_003C_LoadPartModel_003Ec__AnonStorey0 _003C_LoadPartModel_003Ec__AnonStorey = new _003C_LoadPartModel_003Ec__AnonStorey0();
		_003C_LoadPartModel_003Ec__AnonStorey.partCfg = partCfg;
		_003C_LoadPartModel_003Ec__AnonStorey._0024this = this;
		if (!m_visible || !m_partVisible)
		{
			return;
		}
		try
		{
			if (!string.IsNullOrEmpty(_003C_LoadPartModel_003Ec__AnonStorey.partCfg.modelPath))
			{
				ResMgr.Ins.CreateFromAB(_003C_LoadPartModel_003Ec__AnonStorey.partCfg.modelPath, null, _003C_LoadPartModel_003Ec__AnonStorey._003C_003Em__0);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}
}
