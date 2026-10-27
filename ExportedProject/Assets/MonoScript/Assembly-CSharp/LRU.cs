using System;
using System.Collections.Generic;
using UnityEngine;

public class LRU<TKey, TValue>
{
	public delegate bool OnRemoveEntry(TKey key, TValue value);

	private class Node
	{
		public Node Next { get; set; }

		public Node Previous { get; set; }

		public TKey Key { get; set; }

		public TValue Value { get; set; }
	}

	private readonly Dictionary<TKey, Node> entries;

	private readonly int capacity;

	private Node head;

	private Node tail;

	public OnRemoveEntry onRemoveEntry;

	public LRU(int capacity = 16)
	{
		if (capacity <= 0)
		{
			throw new ArgumentOutOfRangeException("capacity", "Capacity should be greater than zero");
		}
		this.capacity = capacity;
		entries = new Dictionary<TKey, Node>();
		head = null;
	}

	public void Set(TKey key, TValue value)
	{
		Node value2;
		if (!entries.TryGetValue(key, out value2))
		{
			Node node = new Node();
			node.Key = key;
			node.Value = value;
			value2 = node;
			entries.Add(key, value2);
		}
		else
		{
			value2.Value = value;
		}
		MoveToHead(value2);
		if (tail == null)
		{
			tail = head;
			return;
		}
		Node node2 = tail;
		int num = entries.Count - capacity;
		while (num-- > 0 && node2 != null)
		{
			if (onRemoveEntry != null)
			{
				try
				{
					if (onRemoveEntry(node2.Key, node2.Value))
					{
						entries.Remove(node2.Key);
						Node previous = node2.Previous;
						if (previous != null)
						{
							previous.Next = node2.Next;
						}
						if (node2.Next != null)
						{
							node2.Next.Previous = previous;
						}
						node2.Previous = null;
						node2.Next = null;
						if (node2 == tail)
						{
							tail = previous;
						}
						node2 = previous;
					}
					else
					{
						node2 = node2.Previous;
					}
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			else
			{
				entries.Remove(tail.Key);
				tail = tail.Previous;
				if (tail != null)
				{
					tail.Next = null;
				}
			}
		}
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		value = default(TValue);
		Node value2;
		if (!entries.TryGetValue(key, out value2))
		{
			return false;
		}
		MoveToHead(value2);
		value = value2.Value;
		return true;
	}

	public List<TValue> GetValues()
	{
		List<TValue> list = new List<TValue>(entries.Count);
		foreach (Node value in entries.Values)
		{
			list.Add(value.Value);
		}
		return list;
	}

	public bool Remove(TKey key)
	{
		Node value;
		if (!entries.TryGetValue(key, out value))
		{
			return false;
		}
		entries.Remove(key);
		Node previous = value.Previous;
		Node next = value.Next;
		if (previous != null)
		{
			previous.Next = next;
		}
		else
		{
			head = next;
		}
		if (next != null)
		{
			next.Previous = previous;
		}
		else
		{
			tail = next;
		}
		if (tail == null)
		{
			tail = head;
		}
		value.Previous = null;
		value.Next = null;
		return true;
	}

	public void Clear()
	{
		entries.Clear();
		head = (tail = null);
	}

	private void MoveToHead(Node entry)
	{
		if (entry != head && entry != null)
		{
			Node next = entry.Next;
			Node previous = entry.Previous;
			if (next != null)
			{
				next.Previous = entry.Previous;
			}
			if (previous != null)
			{
				previous.Next = entry.Next;
			}
			entry.Previous = null;
			entry.Next = head;
			if (head != null)
			{
				head.Previous = entry;
			}
			head = entry;
			if (tail == entry)
			{
				tail = previous;
			}
		}
	}
}
