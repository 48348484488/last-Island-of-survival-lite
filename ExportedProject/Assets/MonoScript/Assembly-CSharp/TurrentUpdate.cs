using System.Collections.Generic;
using UnityEngine;

public class TurrentUpdate : MonoBehaviour
{
	private List<Utils.VoidDelegate> _updateVoidDelegate = new List<Utils.VoidDelegate>();

	private static GameObject _turrentUpdateGameObject;

	public static TurrentUpdate Init()
	{
		_turrentUpdateGameObject = new GameObject("TurrentUpdateGameObject");
		return _turrentUpdateGameObject.AddComponent<TurrentUpdate>();
	}

	public void AddUpdate(Utils.VoidDelegate callback)
	{
		if (_updateVoidDelegate.Contains(callback))
		{
			_updateVoidDelegate.Remove(callback);
		}
		_updateVoidDelegate.Add(callback);
	}

	public void RemoveUpdate(Utils.VoidDelegate callback)
	{
		if (_updateVoidDelegate.Contains(callback))
		{
			_updateVoidDelegate.Remove(callback);
		}
	}

	private void Update()
	{
		foreach (Utils.VoidDelegate item in _updateVoidDelegate)
		{
			Utils.TriggerEvent(item);
		}
	}
}
