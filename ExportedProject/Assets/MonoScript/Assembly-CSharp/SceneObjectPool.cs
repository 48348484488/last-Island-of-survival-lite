using UnityEngine;

public class SceneObjectPool : ObjectPool<Transform>
{
	private readonly GameObject m_Protype;

	public SceneObjectPool(GameObject protype)
	{
		m_Protype = protype;
		m_Protype.SetActive(false);
		Init(int.MaxValue, CreateObject, DestroyObject, RecycleObject);
	}

	private Transform CreateObject()
	{
		return Object.Instantiate(m_Protype).transform;
	}

	private void DestroyObject(Transform t)
	{
		Object.Destroy(t.gameObject);
	}

	private void RecycleObject(Transform t)
	{
		t.gameObject.SetActive(false);
	}
}
