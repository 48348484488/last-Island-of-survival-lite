using UnityEngine;

public class ThrowWeapon : Weapon
{
	private int ItemId;

	private BasePlayerController m_owner;

	public ThrowWeapon(BasePlayerController owner, int id, int skinid = -1)
		: base(owner, id, skinid)
	{
		ItemId = id;
		m_owner = owner;
	}

	public override void SetWeaponGameObject(GameObject gunGo)
	{
		base.SetWeaponGameObject(gunGo);
	}
}
