using UnityEngine;

public class ScenePrefabPool : ObjectPool<ObjectHolder>
{
	public string ABName;

	private AssetBundle m_AB;

	private GameObject m_Protype;

	private ObjectHolder m_ProtypeHolder;

	private Transform m_Parent;

	public int RefCount;

	public bool CanUnload = true;

	public ScenePrefabPool(string abName, GameObject protype, Transform parent, AssetBundle ab)
	{
		ABName = abName;
		m_Protype = protype;
		m_ProtypeHolder = new ObjectHolder();
		m_ProtypeHolder.Go = m_Protype;
		m_ProtypeHolder.Trans = m_Protype.transform;
		m_Protype.SetActiveBetter(false);
		m_Parent = parent;
		m_AB = ab;
		Init(int.MaxValue, CreateObject, DestroyObject, RecycleObject);
	}

	public ObjectHolder GetProtype()
	{
		return m_ProtypeHolder;
	}

	public void DestroySome(int count)
	{
		while (count-- > 0 && m_Stack.Count > 0)
		{
			ObjectHolder objectHolder = m_Stack.Pop();
			base.countAll--;
			objectHolder.Destroy();
		}
	}

	public void Destroy()
	{
		RefCount = 0;
		m_Parent = null;
		Clear();
		if (m_AB != null)
		{
			m_AB.Unload(true);
			m_AB = null;
		}
		else
		{
			Object.Destroy(m_Protype);
		}
		m_Protype = null;
	}

	private ObjectHolder CreateObject()
	{
		ObjectHolder objectHolder = new ObjectHolder();
		GameObject gameObject = Object.Instantiate(m_Protype);
		gameObject.SetActiveBetter(true);
		objectHolder.Go = gameObject;
		objectHolder.Trans = gameObject.transform;
		objectHolder.Trans.SetParent(m_Parent);
		return objectHolder;
	}

	private void DestroyObject(ObjectHolder holder)
	{
		holder.Destroy();
	}

	private void RecycleObject(ObjectHolder holder)
	{
	}
}
