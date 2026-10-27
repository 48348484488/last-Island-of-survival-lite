using UnityEngine;

public class NearWeapon : Weapon
{
	private HandAttack m_handAttack;

	private int ItemId;

	private BasePlayerController m_owner;

	public NearWeapon(BasePlayerController owner, int id, int skinid = -1)
		: base(owner, id, skinid)
	{
		ItemId = id;
		m_owner = owner;
	}

	public override void SetWeaponGameObject(GameObject gunGo)
	{
		base.SetWeaponGameObject(gunGo);
		m_handAttack = gunGo.GetComponentInChildren<Collider>().gameObject.AddComponent<HandAttack>();
		m_handAttack.ItemId = ItemId;
		m_handAttack.First = false;
		m_handAttack.InsId = m_owner.InsId;
	}

	public void Close()
	{
		if (m_handAttack != null)
		{
			m_handAttack.First = false;
		}
	}

	public void Enable()
	{
		if (m_handAttack != null)
		{
			m_handAttack.OnTriggerEnter1();
		}
	}
}
