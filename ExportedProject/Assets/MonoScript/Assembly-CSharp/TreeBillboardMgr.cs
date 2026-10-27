using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBillboardMgr
{
	public static TreeBillboardMgr Ins = new TreeBillboardMgr();

	private readonly List<TreeBillboard> treeBillboardList;

	private readonly Dictionary<TreeBillboard, int> tree2Index;

	private static readonly WaitForSeconds wait = new WaitForSeconds(0.1f);

	private Coroutine coroutine;

	private int nLastUpdateIndex;

	private int nUpdateCountPerFrame = 50;

	private TreeBillboardMgr()
	{
		treeBillboardList = new List<TreeBillboard>(200);
		tree2Index = new Dictionary<TreeBillboard, int>();
	}

	public void Init()
	{
		coroutine = Battle.Ins.StartCoroutine(CheckBillboard());
		nLastUpdateIndex = 0;
		treeBillboardList.Clear();
		tree2Index.Clear();
	}

	public void UnInit()
	{
		Battle.Ins.StopCoroutine(coroutine);
		coroutine = null;
		treeBillboardList.Clear();
		tree2Index.Clear();
	}

	private IEnumerator CheckBillboard()
	{
		while (true)
		{
			yield return wait;
			if (nLastUpdateIndex >= treeBillboardList.Count)
			{
				nLastUpdateIndex = 0;
			}
			int nEnd = nLastUpdateIndex + nUpdateCountPerFrame;
			if (nEnd >= treeBillboardList.Count)
			{
				nEnd = treeBillboardList.Count;
			}
			for (int i = nLastUpdateIndex; i < nEnd; i++)
			{
				treeBillboardList[i].CheckBillboard();
			}
			nLastUpdateIndex = nEnd;
		}
	}

	public void RegisTreeBoard(TreeBillboard treeboard)
	{
		if (!tree2Index.ContainsKey(treeboard))
		{
			tree2Index.Add(treeboard, treeBillboardList.Count);
			treeBillboardList.Add(treeboard);
		}
	}

	public void UnRegisTreeBoard(TreeBillboard treeboard)
	{
		int value = 0;
		if (tree2Index.TryGetValue(treeboard, out value))
		{
			int index = treeBillboardList.Count - 1;
			TreeBillboard treeBillboard = treeBillboardList[index];
			treeBillboardList[value] = treeBillboard;
			tree2Index[treeBillboard] = value;
			treeBillboardList.RemoveAt(index);
			tree2Index.Remove(treeboard);
		}
	}
}
