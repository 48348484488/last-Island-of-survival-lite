using System.Collections.Generic;
using UnityEngine;
using gs.battle.drop.scmsg;
using gs.battle.scmsg;

public class TrashcanStationGameObject : MonoBehaviour
{
	private long _instanceId;

	private SShowTrashcanStation _sShowTrashcanStation;

	private LajiduiInfo _lajiduiInfo;

	private List<GameObject> _trashGameObjects = new List<GameObject>();

	public long InsId
	{
		get
		{
			if (_sShowTrashcanStation == null)
			{
				return -1L;
			}
			return _sShowTrashcanStation.instanceId;
		}
	}

	public int CfgId
	{
		get
		{
			if (_lajiduiInfo == null)
			{
				return -1;
			}
			return _lajiduiInfo.cfgId;
		}
	}

	public void SetData(SShowTrashcanStation showTrashcanStation)
	{
		_lajiduiInfo = base.gameObject.GetComponent<LajiduiInfo>();
		_trashGameObjects.Clear();
		_sShowTrashcanStation = showTrashcanStation;
		SetPos(showTrashcanStation.pos);
		SetRotation(showTrashcanStation.orientation);
		if (BattleDropEvent.LaJiZhanBaiXiangZiAction != null)
		{
			BattleDropEvent.LaJiZhanBaiXiangZiAction(_sShowTrashcanStation.trashcanInfos, SetItem);
		}
	}

	private void SetPos(Vec3 pos)
	{
		base.transform.position = new Vector3(pos.x, pos.y, pos.z);
	}

	private void SetRotation(Vec3 angle)
	{
		base.transform.eulerAngles = new Vector3(angle.x, angle.y, angle.z);
	}

	private void SetItem(TrashcanInfo trashcanInfo, GameObject go)
	{
		if ((bool)_lajiduiInfo)
		{
			List<Transform> list = ((!trashcanInfo.isBox) ? _lajiduiInfo._tongs : _lajiduiInfo._xiangzis);
			if (trashcanInfo.index < list.Count)
			{
				go.SetActiveBetter(true);
				go.transform.SetParent(list[trashcanInfo.index]);
				go.transform.localScale = Vector3.one;
				go.transform.localPosition = Vector3.zero;
				go.transform.localEulerAngles = Vector3.zero;
				_trashGameObjects.Add(go);
			}
		}
	}

	public void RecycleTrashGameObject()
	{
		Singleton<BattleDropMgr>.Ins.RecycleLaJiZhanLaJiTong(_trashGameObjects);
		_trashGameObjects.Clear();
	}
}
