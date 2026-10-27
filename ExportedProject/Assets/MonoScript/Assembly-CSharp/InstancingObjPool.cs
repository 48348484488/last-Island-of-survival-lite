public class InstancingObjPool : ObjectPool<InstancingObj>
{
	public InstancingObjPool()
	{
		Init(int.MaxValue, CreateObject, DestroyObject, RecycleObject);
	}

	private InstancingObj CreateObject()
	{
		return new InstancingObj();
	}

	private void DestroyObject(InstancingObj t)
	{
	}

	private void RecycleObject(InstancingObj t)
	{
		t.Reset();
	}
}
