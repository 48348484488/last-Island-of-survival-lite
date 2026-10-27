using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;

public abstract class Weapon
{
	protected ItemCfg m_weaponCfg;

	public GameObject WeaponObject;

	public bool m_visible = true;

	protected bool m_destroyed;

	public BasePlayerController Owner;

	public int InsId;

	public int Index;

	public ItemCfg WeaponCfg
	{
		get
		{
			return m_weaponCfg;
		}
	}

	public virtual bool Visible
	{
		get
		{
			return m_visible;
		}
		set
		{
			m_visible = value;
			if (WeaponObject != null)
			{
				WeaponObject.SetActiveBetter(m_visible);
			}
		}
	}

	protected Weapon(BasePlayerController owner, int id, int skinid = -1)
	{
		Owner = owner;
		m_weaponCfg = ItemCfg.Get(id);
		string abPath = ((skinid > 0) ? ItemCfg.Get(skinid).modelPath : m_weaponCfg.modelPath);
		ResMgr.Ins.CreateFromAB(abPath, null, _003CWeapon_003Em__0);
	}

	public virtual void SetWeaponGameObject(GameObject gunGo)
	{
		WeaponObject = gunGo;
		WeaponObject.SetActiveBetter(m_visible);
	}

	public virtual void Destroy()
	{
		m_destroyed = true;
		Owner = null;
		InsId = 0;
		if (WeaponObject != null)
		{
			Object.Destroy(WeaponObject);
			WeaponObject = null;
		}
	}

	[CompilerGenerated]
	private void _003CWeapon_003Em__0(GameObject go)
	{
		if (m_destroyed)
		{
			Object.Destroy(go);
			return;
		}
		SkinnedMeshRenderer componentInChildren = go.GetComponentInChildren<SkinnedMeshRenderer>();
		MeshRenderer componentInChildren2 = go.GetComponentInChildren<MeshRenderer>();
		MaterialLevel componentInChildren3 = go.GetComponentInChildren<MaterialLevel>();
		if (componentInChildren3 != null && componentInChildren != null)
		{
			componentInChildren.material = componentInChildren3.Low;
		}
		if (componentInChildren3 != null && componentInChildren2 != null)
		{
			componentInChildren2.material = componentInChildren3.Low;
		}
		SetWeaponGameObject(go);
		Owner.GuaWuqiInHand(InsId);
	}
}
