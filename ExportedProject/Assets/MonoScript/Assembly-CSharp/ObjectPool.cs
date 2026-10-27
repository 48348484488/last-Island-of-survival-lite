using System.Collections.Generic;

public class ObjectPool<T>
{
	public delegate T0 CreateObject<T0>();

	public delegate void DestroyObject<T1>(T1 arg);

	public delegate void RecycleObject<T1>(T1 arg);

	protected int mCapacity;

	protected readonly Stack<T> m_Stack = new Stack<T>();

	protected CreateObject<T> mCreateFun;

	protected DestroyObject<T> mDestroyFun;

	protected RecycleObject<T> mRecycleFun;

	public int countAll { get; protected set; }

	public int countActive
	{
		get
		{
			return countAll - countInactive;
		}
	}

	public int countInactive
	{
		get
		{
			return m_Stack.Count;
		}
	}

	public ObjectPool()
	{
	}

	public ObjectPool(int capacity, CreateObject<T> createFun, DestroyObject<T> destroyFun = null, RecycleObject<T> recycleFun = null)
	{
		Init(capacity, createFun, destroyFun, recycleFun);
	}

	public void Init(int capacity, CreateObject<T> createFun, DestroyObject<T> destroyFun, RecycleObject<T> recycleFun = null)
	{
		mCapacity = capacity;
		mCreateFun = createFun;
		mDestroyFun = destroyFun;
		mRecycleFun = recycleFun;
	}

	public T Get()
	{
		T result;
		if (m_Stack.Count == 0)
		{
			result = mCreateFun();
			countAll++;
		}
		else
		{
			result = m_Stack.Pop();
		}
		return result;
	}

	public void Recycle(T element)
	{
		if (mRecycleFun != null)
		{
			mRecycleFun(element);
		}
		if (mCapacity > 0 && m_Stack.Count >= mCapacity)
		{
			if (mDestroyFun != null)
			{
				mDestroyFun(element);
			}
			countAll--;
		}
		else
		{
			m_Stack.Push(element);
		}
	}

	public void Clear()
	{
		if (mDestroyFun != null)
		{
			foreach (T item in m_Stack)
			{
				mDestroyFun(item);
			}
		}
		m_Stack.Clear();
		countAll = 0;
	}
}
