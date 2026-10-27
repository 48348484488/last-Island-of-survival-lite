public class RoundListSc<E>
{
	private E[] table;

	private int capacity;

	private int maxIndex = -1;

	public RoundListSc(int capacity)
	{
		this.capacity = capacity;
		table = new E[capacity];
	}

	public void add(E e)
	{
		table[++maxIndex % capacity] = e;
	}

	public int size()
	{
		return (maxIndex < capacity) ? (maxIndex + 1) : capacity;
	}

	public E get(int index)
	{
		return table[(maxIndex < capacity) ? index : ((maxIndex + index + 1) % capacity)];
	}

	public void Clear()
	{
		maxIndex = -1;
	}
}
