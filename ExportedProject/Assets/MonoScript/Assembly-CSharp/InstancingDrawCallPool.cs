public class InstancingDrawCallPool : ObjectPool<InstancingDrawCall>
{
	public InstancingDrawCallPool()
	{
		Init(int.MaxValue, CreateObject, DestroyObject, RecycleObject);
	}

	private InstancingDrawCall CreateObject()
	{
		return new InstancingDrawCall();
	}

	private void DestroyObject(InstancingDrawCall t)
	{
	}

	private void RecycleObject(InstancingDrawCall t)
	{
		t.Reset();
	}
}
