using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomListener : MonoBehaviour
{
	public delegate void OnZoomLevelChange(bool zoomIn, Vector2 center);

	private class DownUpData
	{
		public int pointId;

		public Vector2 downPos;

		public Vector2 upPos;
	}

	public OnZoomLevelChange onZoomLevelChange;

	private Dictionary<int, DownUpData> _mPointId2DownUpDataDic = new Dictionary<int, DownUpData>();

	private float lastDownUpTime;

	private DownUpData lastDownUpData;

	private void Start()
	{
		DownUpListener downUpListener = DownUpListener.Get(base.gameObject);
		downUpListener.onDown = (DownUpListener.VoidDelegate)Delegate.Combine(downUpListener.onDown, new DownUpListener.VoidDelegate(OnDown));
		DownUpListener downUpListener2 = DownUpListener.Get(base.gameObject);
		downUpListener2.onUp = (DownUpListener.VoidDelegate)Delegate.Combine(downUpListener2.onUp, new DownUpListener.VoidDelegate(OnUp));
	}

	private void OnDown(GameObject go)
	{
		PointerEventData pointEventData = DownUpListener.pointEventData;
		DownUpData downUpData = new DownUpData();
		downUpData.upPos = default(Vector2);
		downUpData.downPos = default(Vector2);
		downUpData.pointId = pointEventData.pointerId;
		downUpData.downPos.x = pointEventData.position.x;
		downUpData.downPos.y = pointEventData.position.y;
		_mPointId2DownUpDataDic[pointEventData.pointerId] = downUpData;
	}

	private void OnUp(GameObject go)
	{
		PointerEventData pointEventData = DownUpListener.pointEventData;
		if (!_mPointId2DownUpDataDic.ContainsKey(pointEventData.pointerId))
		{
			return;
		}
		DownUpData downUpData = _mPointId2DownUpDataDic[pointEventData.pointerId];
		downUpData.upPos.x = pointEventData.position.x;
		downUpData.upPos.y = pointEventData.position.y;
		float time = Time.time;
		if (time - lastDownUpTime < 0.2f)
		{
			if (lastDownUpData != null && lastDownUpData.pointId != downUpData.pointId)
			{
				float num = SquareDistance(lastDownUpData.downPos, downUpData.downPos);
				float num2 = SquareDistance(lastDownUpData.upPos, downUpData.upPos);
				Vector2 center = new Vector2((downUpData.upPos.x + lastDownUpData.upPos.x) * 0.5f, (downUpData.upPos.y + lastDownUpData.upPos.y) * 0.5f);
				bool zoomIn = num2 >= num;
				if (onZoomLevelChange != null)
				{
					onZoomLevelChange(zoomIn, center);
				}
				lastDownUpData = null;
				lastDownUpTime = 0f;
			}
			else
			{
				lastDownUpTime = time;
				lastDownUpData = downUpData;
			}
		}
		else
		{
			lastDownUpTime = time;
			lastDownUpData = downUpData;
		}
		_mPointId2DownUpDataDic.Remove(pointEventData.pointerId);
	}

	private float SquareDistance(Vector2 v1, Vector2 v2)
	{
		return (v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y);
	}

	private void OnDestroy()
	{
		DownUpListener downUpListener = DownUpListener.Get(base.gameObject);
		downUpListener.onDown = (DownUpListener.VoidDelegate)Delegate.Remove(downUpListener.onDown, new DownUpListener.VoidDelegate(OnDown));
		DownUpListener downUpListener2 = DownUpListener.Get(base.gameObject);
		downUpListener2.onUp = (DownUpListener.VoidDelegate)Delegate.Remove(downUpListener2.onUp, new DownUpListener.VoidDelegate(OnUp));
	}
}
