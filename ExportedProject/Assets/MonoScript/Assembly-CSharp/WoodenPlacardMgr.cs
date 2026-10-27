using System;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Events;
using EasyBuildSystem.Runtimes.Internal.Managers;
using EasyBuildSystem.Runtimes.Internal.Part;
using SC.UI;
using Share;
using UnityEngine;
using cfg;
using gs.battle.map.board.scmsg;

public class WoodenPlacardMgr : Singleton<WoodenPlacardMgr>
{
	public enum HorizontalAlignment
	{
		Left = 0,
		Center = 1,
		Right = 2
	}

	public enum VerticalAlignment
	{
		Up = 0,
		Middle = 1,
		Down = 2
	}

	public Color[] Colors = new Color[12]
	{
		Color.white,
		Color.black,
		Color.yellow,
		Color.red,
		Color.blue,
		Color.green,
		new Color(0.6593f, 0f, 1f),
		new Color(1f, 0.4739f, 0f),
		new Color(1f, 0f, 0.7118f),
		new Color(0f, 1f, 0.747f),
		new Color(0.2125f, 0f, 1f),
		new Color(0.217f, 0.0486f, 0.0276f)
	};

	private Dictionary<long, int> _builds = new Dictionary<long, int>();

	private CEditorBoardInfo _cEditorBoardInfo = new CEditorBoardInfo();

	public void Init()
	{
		EventHandlers.OnBuildPartUpdate = (EventHandlers.BuildPartUpdate)Delegate.Combine(EventHandlers.OnBuildPartUpdate, new EventHandlers.BuildPartUpdate(OnBuildUpgrade));
		EventHandlers.OnBuildPart = (EventHandlers.BuildPartDelegate)Delegate.Combine(EventHandlers.OnBuildPart, new EventHandlers.BuildPartDelegate(OnBuildFinish));
		EventHandlers.OnBuildPartDestory = (EventHandlers.BuildPartDestory)Delegate.Combine(EventHandlers.OnBuildPartDestory, new EventHandlers.BuildPartDestory(OnBuildPartDestory));
		BattleEvent.OnClickUseBuild = (Utils.IntLongDelegate)Delegate.Combine(BattleEvent.OnClickUseBuild, new Utils.IntLongDelegate(OnClickUseBuild));
		SUpdateBoardInfo.handler = (SUpdateBoardInfo.Handler)Delegate.Combine(SUpdateBoardInfo.handler, new SUpdateBoardInfo.Handler(SUpdateBoardInfoHandle));
	}

	private void OnBuildPartDestory(long insid)
	{
	}

	private void SUpdateBoardInfoHandle(SUpdateBoardInfo msg)
	{
		PartBehaviour partByInsID = SingletonMono<BuildManager>.Ins.GetPartByInsID(msg.boardId);
		if (!partByInsID)
		{
			return;
		}
		ItemCfg itemCfg = ItemCfg.Get(partByInsID.Id);
		if (itemCfg != null && (itemCfg.childType == 109 || itemCfg.childType == 110 || itemCfg.childType == 111))
		{
			GameObject gameObject = partByInsID.gameObject;
			WoodenPlacard woodenPlacard = gameObject.GetComponent<WoodenPlacard>();
			if (!woodenPlacard)
			{
				woodenPlacard = gameObject.AddComponent<WoodenPlacard>();
			}
			woodenPlacard.Unmarshal(msg.boardInfo.extraInfo, msg.boardInfo.info);
		}
	}

	public int GetBuildItemId(long instanceId)
	{
		if (_builds.ContainsKey(instanceId))
		{
			return _builds[instanceId];
		}
		return -1;
	}

	private void OnClickUseBuild(int itemId, long instanceId)
	{
		_builds[instanceId] = itemId;
		ItemCfg itemCfg = ItemCfg.Get(itemId);
		int childType = itemCfg.childType;
		if (childType == 109 || childType == 110 || childType == 111)
		{
			ViewMgr.Ins.ShowView<WoodPanel>(instanceId);
		}
	}

	public void EditorBoardInfo(long boardId, string code, string content)
	{
		_cEditorBoardInfo.boardId = boardId;
		_cEditorBoardInfo.boardInfo.extraInfo = code;
		_cEditorBoardInfo.boardInfo.info = content;
		Client2Gs.Ins.Send(_cEditorBoardInfo);
	}

	private void OnBuildFinish(long insid, BuildPart buildpartcfg, int status, Octets extrainfooc)
	{
		ItemCfg itemCfg = ItemCfg.Get(buildpartcfg.id);
		if (itemCfg == null || (itemCfg.childType != 109 && itemCfg.childType != 110 && itemCfg.childType != 111))
		{
			return;
		}
		GameObject partGoByInsID = SingletonMono<BuildManager>.Ins.GetPartGoByInsID(insid);
		WoodenPlacard woodenPlacard = partGoByInsID.GetComponent<WoodenPlacard>();
		if (!woodenPlacard)
		{
			woodenPlacard = partGoByInsID.AddComponent<WoodenPlacard>();
		}
		BoardInfo boardInfo = new BoardInfo();
		if (extrainfooc.Size > 0)
		{
			boardInfo.unmarshal(extrainfooc.copy());
			if (!string.IsNullOrEmpty(boardInfo.info))
			{
				woodenPlacard.Unmarshal(boardInfo.extraInfo, boardInfo.info);
			}
			else
			{
				woodenPlacard.Clear();
			}
		}
	}

	private void OnBuildUpgrade(long insid, BuildPart buildpartcfg)
	{
	}
}
