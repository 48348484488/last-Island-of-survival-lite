using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensitiveWordMgr : Singleton<SensitiveWordMgr>
{
	public class TrieNode
	{
		public static int num;

		public int id = num++;

		public List<TrieNode> childNodes;

		public char nodeChar;

		public int count;

		public TrieNode(int capacity = 2)
		{
			count = 0;
			childNodes = new List<TrieNode>(capacity);
		}
	}

	public class TrieKMP
	{
		public const int Defalut_value = int.MinValue;

		public TrieNode root = new TrieNode(1024);

		public Dictionary<long, int> idAndChar2index = new Dictionary<long, int>();

		public TrieNode[] fail;

		public void buildTrie(string word)
		{
			if (word.Length < 1)
			{
				return;
			}
			TrieNode trieNode = root;
			foreach (char c in word)
			{
				long indexKey = getIndexKey(trieNode, c);
				int num = ((!idAndChar2index.ContainsKey(indexKey)) ? int.MinValue : idAndChar2index[indexKey]);
				if (num == int.MinValue)
				{
					TrieNode trieNode2 = new TrieNode();
					trieNode2.nodeChar = c;
					int count = trieNode.childNodes.Count;
					trieNode.childNodes.Add(trieNode2);
					if (idAndChar2index.ContainsKey(indexKey))
					{
						idAndChar2index[indexKey] = count;
					}
					else
					{
						idAndChar2index.Add(indexKey, count);
					}
					trieNode = trieNode2;
				}
				else
				{
					trieNode = trieNode.childNodes[num];
				}
			}
			trieNode.count = word.Length;
		}

		public void buildFail()
		{
			fail = new TrieNode[TrieNode.num + 10];
			Queue<TrieNode> queue = new Queue<TrieNode>();
			queue.Enqueue(root);
			fail[root.id] = null;
			while (queue.Count != 0)
			{
				TrieNode trieNode = queue.Dequeue();
				TrieNode trieNode2 = null;
				for (int i = 0; i < trieNode.childNodes.Count; i++)
				{
					TrieNode trieNode3 = trieNode.childNodes[i];
					if (trieNode == root)
					{
						fail[trieNode3.id] = root;
					}
					else
					{
						for (trieNode2 = fail[trieNode.id]; trieNode2 != null; trieNode2 = fail[trieNode2.id])
						{
							long indexKey = getIndexKey(trieNode2, trieNode3.nodeChar);
							int num = ((!idAndChar2index.ContainsKey(indexKey)) ? int.MinValue : idAndChar2index[indexKey]);
							if (num != int.MinValue)
							{
								fail[trieNode3.id] = trieNode2.childNodes[num];
								break;
							}
						}
						if (trieNode2 == null)
						{
							fail[trieNode3.id] = root;
						}
					}
					queue.Enqueue(trieNode3);
				}
			}
		}

		public bool contains(string sentence)
		{
			bool result = false;
			TrieNode trieNode = root;
			foreach (char c in sentence)
			{
				long indexKey = getIndexKey(trieNode, c);
				int num = ((!idAndChar2index.ContainsKey(indexKey)) ? int.MinValue : idAndChar2index[indexKey]);
				while (num == int.MinValue && trieNode != root)
				{
					trieNode = fail[trieNode.id];
					indexKey = getIndexKey(trieNode, c);
					num = ((!idAndChar2index.ContainsKey(indexKey)) ? int.MinValue : idAndChar2index[indexKey]);
				}
				trieNode = ((num != int.MinValue) ? trieNode.childNodes[num] : root);
				if (trieNode.count > 0)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public long getIndexKey(TrieNode nowP, char c)
		{
			return ((long)nowP.id << 16) + (int)c;
		}

		public string replace(string sentence, char replace)
		{
			TrieNode trieNode = root;
			char[] array = sentence.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				char c = array[i];
				long indexKey = getIndexKey(trieNode, c);
				int num = ((!idAndChar2index.ContainsKey(indexKey)) ? int.MinValue : idAndChar2index[indexKey]);
				while (trieNode != root && num == int.MinValue)
				{
					trieNode = fail[trieNode.id];
					indexKey = getIndexKey(trieNode, c);
					num = ((!idAndChar2index.ContainsKey(indexKey)) ? int.MinValue : idAndChar2index[indexKey]);
				}
				trieNode = ((num != int.MinValue) ? trieNode.childNodes[num] : root);
				if (trieNode.count <= 0)
				{
					continue;
				}
				int num2 = int.MinValue;
				if (i < array.Length - 1)
				{
					long indexKey2 = getIndexKey(trieNode, array[i + 1]);
					num2 = ((!idAndChar2index.ContainsKey(indexKey2)) ? int.MinValue : idAndChar2index[indexKey2]);
				}
				if (num2 == int.MinValue)
				{
					for (int j = 0; j < trieNode.count; j++)
					{
						array[i - j] = replace;
					}
					trieNode = root;
				}
			}
			return new string(array);
		}
	}

	public TrieKMP trieKMP;

	public bool containsSensitiveWord(string sentence)
	{
		return trieKMP.contains(sentence);
	}

	public string replaceSensitiveWord(string sentence)
	{
		return trieKMP.replace(sentence, '*');
	}

	public void Init()
	{
		trieKMP = new TrieKMP();
		Utils.StartConroutine(LoadRecord());
	}

	public IEnumerator LoadRecord()
	{
		string path = Utils.GetStreamingAssetPathForWWW("sensitiveWord/sw.txt");
		WWW www = new WWW(path);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			setTrie(www.text);
		}
		else
		{
			Debug.LogError(www.error);
		}
	}

	private void setTrie(string str)
	{
		string[] array = str.Split('\n');
		int i = 0;
		for (int num = array.Length; i < num; i++)
		{
			string text = array[i].Trim();
			if (!string.IsNullOrEmpty(text))
			{
				trieKMP.buildTrie(text);
			}
		}
		trieKMP.buildFail();
	}
}
