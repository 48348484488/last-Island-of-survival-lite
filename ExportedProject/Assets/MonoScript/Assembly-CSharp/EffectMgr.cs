using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

public class EffectMgr : SingletonMono<EffectMgr>
{
	public delegate void OnEffectLoaded(EffectInfo effectInfo);

	[CompilerGenerated]
	private sealed class _003CPlayEffectAtPos_003Ec__AnonStorey1
	{
		internal Transform effectParent;

		internal Vector3 pos;

		internal string path;

		internal EffectMgr _0024this;

		internal void _003C_003Em__0(EffectInfo info)
		{
			Transform transform = info.transform;
			transform.SetParent(effectParent, false);
			transform.localPosition = pos;
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.identity;
			transform.localEulerAngles = Vector3.zero;
			info.gameObject.SetActive(true);
			if (_0024this.effectName2LoadFinishDelegateDic.ContainsKey(path) && _0024this.effectName2LoadFinishDelegateDic[path] != null)
			{
				_0024this.effectName2LoadFinishDelegateDic[path](info.gameObject);
			}
			RenderQueueFixerInUIForParticle component = info.GetComponent<RenderQueueFixerInUIForParticle>();
			if (component != null)
			{
				component.StartSetting();
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CPlayEffectAtPos_003Ec__AnonStorey2
	{
		internal Transform effectParent;

		internal Vector3 pos;

		internal Vector3 forward;

		internal string path;

		internal EffectMgr _0024this;

		internal void _003C_003Em__0(EffectInfo info)
		{
			Transform transform = info.transform;
			transform.SetParent(effectParent, false);
			transform.localPosition = pos;
			transform.localScale = Vector3.one;
			transform.localRotation = Quaternion.identity;
			transform.localEulerAngles = Vector3.zero;
			transform.forward = forward;
			info.gameObject.SetActive(true);
			if (_0024this.effectName2LoadFinishDelegateDic.ContainsKey(path) && _0024this.effectName2LoadFinishDelegateDic[path] != null)
			{
				_0024this.effectName2LoadFinishDelegateDic[path](info.gameObject);
			}
			RenderQueueFixerInUIForParticle component = info.GetComponent<RenderQueueFixerInUIForParticle>();
			if (component != null)
			{
				component.StartSetting();
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CPlayEffectAtPos_003Ec__AnonStorey3
	{
		internal Transform effectParent;

		internal Vector3 pos;

		internal Quaternion rotation;

		internal string path;

		internal EffectMgr _0024this;

		internal void _003C_003Em__0(EffectInfo info)
		{
			Transform transform = info.transform;
			transform.SetParent(effectParent, false);
			transform.localPosition = pos;
			transform.localScale = Vector3.one;
			transform.rotation = rotation;
			info.gameObject.SetActive(true);
			if (_0024this.effectName2LoadFinishDelegateDic.ContainsKey(path) && _0024this.effectName2LoadFinishDelegateDic[path] != null)
			{
				_0024this.effectName2LoadFinishDelegateDic[path](info.gameObject);
			}
			RenderQueueFixerInUIForParticle component = info.GetComponent<RenderQueueFixerInUIForParticle>();
			if (component != null)
			{
				component.StartSetting();
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CPlayEffectAtWorldPos_003Ec__AnonStorey4
	{
		internal Vector3 pos;

		internal Vector3 forward;

		internal string path;

		internal EffectMgr _0024this;

		internal void _003C_003Em__0(EffectInfo info)
		{
			Transform transform = info.transform;
			transform.SetParent(transform, false);
			transform.position = pos;
			transform.localScale = Vector3.one;
			transform.rotation = Quaternion.identity;
			transform.forward = forward;
			info.gameObject.SetActive(true);
			if (_0024this.effectName2LoadFinishDelegateDic.ContainsKey(path) && _0024this.effectName2LoadFinishDelegateDic[path] != null)
			{
				_0024this.effectName2LoadFinishDelegateDic[path](info.gameObject);
			}
			RenderQueueFixerInUIForParticle component = info.GetComponent<RenderQueueFixerInUIForParticle>();
			if (component != null)
			{
				component.StartSetting();
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CPlayEffectAtWorldPos_003Ec__AnonStorey5
	{
		internal Vector3 pos;

		internal string path;

		internal EffectMgr _0024this;

		internal void _003C_003Em__0(EffectInfo info)
		{
			Transform transform = info.transform;
			transform.SetParent(transform, false);
			transform.position = pos;
			transform.localScale = Vector3.one;
			transform.rotation = Quaternion.identity;
			info.gameObject.SetActive(true);
			if (_0024this.effectName2LoadFinishDelegateDic.ContainsKey(path) && _0024this.effectName2LoadFinishDelegateDic[path] != null)
			{
				_0024this.effectName2LoadFinishDelegateDic[path](info.gameObject);
			}
			RenderQueueFixerInUIForParticle component = info.GetComponent<RenderQueueFixerInUIForParticle>();
			if (component != null)
			{
				component.StartSetting();
			}
		}
	}

	private const int CacheSize = 20;

	private readonly LinkedList<EffectInfo> mEffectCache = new LinkedList<EffectInfo>();

	private Transform mCachedTrans;

	private Dictionary<string, Utils.GameObjectDelegate> effectName2LoadFinishDelegateDic = new Dictionary<string, Utils.GameObjectDelegate>();

	protected override void Awake()
	{
		base.Awake();
		mCachedTrans = base.transform;
		mCachedTrans.localScale = Vector3.one;
		mCachedTrans.rotation = Quaternion.identity;
	}

	private void OnDestroy()
	{
		foreach (EffectInfo item in mEffectCache)
		{
			Object.Destroy(item.gameObject);
		}
		mEffectCache.Clear();
	}

	public EffectMgr PlayEffectAtPos(string path, Transform effectParent, Vector3 pos)
	{
		_003CPlayEffectAtPos_003Ec__AnonStorey1 _003CPlayEffectAtPos_003Ec__AnonStorey = new _003CPlayEffectAtPos_003Ec__AnonStorey1();
		_003CPlayEffectAtPos_003Ec__AnonStorey.effectParent = effectParent;
		_003CPlayEffectAtPos_003Ec__AnonStorey.pos = pos;
		_003CPlayEffectAtPos_003Ec__AnonStorey.path = path;
		_003CPlayEffectAtPos_003Ec__AnonStorey._0024this = this;
		StartCoroutine(_LoadEffect(_003CPlayEffectAtPos_003Ec__AnonStorey.path, _003CPlayEffectAtPos_003Ec__AnonStorey._003C_003Em__0));
		return this;
	}

	public EffectMgr PlayEffectAtPos(string path, Transform effectParent, Vector3 pos, Vector3 forward)
	{
		_003CPlayEffectAtPos_003Ec__AnonStorey2 _003CPlayEffectAtPos_003Ec__AnonStorey = new _003CPlayEffectAtPos_003Ec__AnonStorey2();
		_003CPlayEffectAtPos_003Ec__AnonStorey.effectParent = effectParent;
		_003CPlayEffectAtPos_003Ec__AnonStorey.pos = pos;
		_003CPlayEffectAtPos_003Ec__AnonStorey.forward = forward;
		_003CPlayEffectAtPos_003Ec__AnonStorey.path = path;
		_003CPlayEffectAtPos_003Ec__AnonStorey._0024this = this;
		StartCoroutine(_LoadEffect(_003CPlayEffectAtPos_003Ec__AnonStorey.path, _003CPlayEffectAtPos_003Ec__AnonStorey._003C_003Em__0));
		return this;
	}

	public EffectMgr PlayEffectAtPos(string path, Transform effectParent, Vector3 pos, Quaternion rotation)
	{
		_003CPlayEffectAtPos_003Ec__AnonStorey3 _003CPlayEffectAtPos_003Ec__AnonStorey = new _003CPlayEffectAtPos_003Ec__AnonStorey3();
		_003CPlayEffectAtPos_003Ec__AnonStorey.effectParent = effectParent;
		_003CPlayEffectAtPos_003Ec__AnonStorey.pos = pos;
		_003CPlayEffectAtPos_003Ec__AnonStorey.rotation = rotation;
		_003CPlayEffectAtPos_003Ec__AnonStorey.path = path;
		_003CPlayEffectAtPos_003Ec__AnonStorey._0024this = this;
		StartCoroutine(_LoadEffect(_003CPlayEffectAtPos_003Ec__AnonStorey.path, _003CPlayEffectAtPos_003Ec__AnonStorey._003C_003Em__0));
		return this;
	}

	public EffectMgr PlayEffectAtWorldPos(string path, Vector3 pos, Vector3 forward)
	{
		_003CPlayEffectAtWorldPos_003Ec__AnonStorey4 _003CPlayEffectAtWorldPos_003Ec__AnonStorey = new _003CPlayEffectAtWorldPos_003Ec__AnonStorey4();
		_003CPlayEffectAtWorldPos_003Ec__AnonStorey.pos = pos;
		_003CPlayEffectAtWorldPos_003Ec__AnonStorey.forward = forward;
		_003CPlayEffectAtWorldPos_003Ec__AnonStorey.path = path;
		_003CPlayEffectAtWorldPos_003Ec__AnonStorey._0024this = this;
		StartCoroutine(_LoadEffect(_003CPlayEffectAtWorldPos_003Ec__AnonStorey.path, _003CPlayEffectAtWorldPos_003Ec__AnonStorey._003C_003Em__0));
		return this;
	}

	public EffectMgr PlayEffectAtWorldPos(string path, Vector3 pos)
	{
		_003CPlayEffectAtWorldPos_003Ec__AnonStorey5 _003CPlayEffectAtWorldPos_003Ec__AnonStorey = new _003CPlayEffectAtWorldPos_003Ec__AnonStorey5();
		_003CPlayEffectAtWorldPos_003Ec__AnonStorey.pos = pos;
		_003CPlayEffectAtWorldPos_003Ec__AnonStorey.path = path;
		_003CPlayEffectAtWorldPos_003Ec__AnonStorey._0024this = this;
		StartCoroutine(_LoadEffect(_003CPlayEffectAtWorldPos_003Ec__AnonStorey.path, _003CPlayEffectAtWorldPos_003Ec__AnonStorey._003C_003Em__0));
		return this;
	}

	public void OnComplete(string path, Utils.GameObjectDelegate gameObjectDelegate)
	{
		if (!effectName2LoadFinishDelegateDic.ContainsKey(path))
		{
			effectName2LoadFinishDelegateDic.Add(path, gameObjectDelegate);
		}
	}

	public EffectMgr PlayEffect(string path, Transform effectParent)
	{
		return PlayEffectAtPos(path, effectParent, Vector3.zero);
	}

	public Coroutine LoadEffect(string path, OnEffectLoaded callback)
	{
		return StartCoroutine(_LoadEffect(path, callback));
	}

	public void RecycleEffect(EffectInfo effectInfo)
	{
		effectInfo.transform.SetParent(mCachedTrans);
		if (!(effectInfo.lifetime > 0f))
		{
			_RecycleEffect(effectInfo);
		}
	}

	public void ForceRecycleEffect(EffectInfo effectInfo)
	{
		effectInfo.transform.SetParent(mCachedTrans);
		_RecycleEffect(effectInfo);
	}

	public string Dump()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (EffectInfo item in mEffectCache)
		{
			stringBuilder.Append(item.path);
			stringBuilder.Append(",");
		}
		return stringBuilder.ToString();
	}

	public void _RecycleEffect(EffectInfo effectInfo)
	{
		effectInfo.transform.SetParent(mCachedTrans, false);
		effectInfo.gameObject.SetActive(false);
		mEffectCache.AddFirst(effectInfo);
		while (mEffectCache.Count > 20)
		{
			LinkedListNode<EffectInfo> last = mEffectCache.Last;
			mEffectCache.RemoveLast();
			if (last != null && last.Value != null)
			{
				Object.Destroy(last.Value.gameObject);
			}
		}
	}

	private IEnumerator _LoadEffect(string path, OnEffectLoaded callback)
	{
		_003C_LoadEffect_003Ec__Iterator0._003C_LoadEffect_003Ec__AnonStorey6 _003C_LoadEffect_003Ec__AnonStorey = new _003C_LoadEffect_003Ec__Iterator0._003C_LoadEffect_003Ec__AnonStorey6();
		_003C_LoadEffect_003Ec__AnonStorey._003C_003Ef__ref_00240 = this;
		EffectInfo info2 = TakeOne(path);
		if (info2 != null)
		{
			callback(info2);
			yield break;
		}
		_003C_LoadEffect_003Ec__AnonStorey.go = null;
		yield return ResMgr.Ins.CreateFromAB(path, null, _003C_LoadEffect_003Ec__AnonStorey._003C_003Em__0);
		if (!(_003C_LoadEffect_003Ec__AnonStorey.go == null))
		{
			info2 = _003C_LoadEffect_003Ec__AnonStorey.go.GetComponent<EffectInfo>();
			if (info2 == null)
			{
				Debug.LogError(path + " no EffectInfo");
			}
			info2.path = path;
			_003C_LoadEffect_003Ec__AnonStorey.go.transform.SetParent(mCachedTrans, false);
			callback(info2);
		}
	}

	private EffectInfo TakeOne(string path)
	{
		for (LinkedListNode<EffectInfo> linkedListNode = mEffectCache.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			if (linkedListNode.Value.path == path)
			{
				mEffectCache.Remove(linkedListNode);
				return linkedListNode.Value;
			}
		}
		return null;
	}
}
