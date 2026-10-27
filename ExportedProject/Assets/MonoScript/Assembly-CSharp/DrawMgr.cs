using System;
using SC.UI;
using gs.research.scmsg;

public class DrawMgr : Singleton<DrawMgr>
{
	private CResearchInfo _cResearchInfo = new CResearchInfo();

	private CStartResearch _cStartResearch = new CStartResearch();

	private CCancelResearch _cCancelResearch = new CCancelResearch();

	private CGetResearchItem _cGetResearchItem = new CGetResearchItem();

	public void Init()
	{
		SResearchInfo.handler = (SResearchInfo.Handler)Delegate.Combine(SResearchInfo.handler, new SResearchInfo.Handler(SResearchInfoHandle));
		SGetResearchItem.handler = (SGetResearchItem.Handler)Delegate.Combine(SGetResearchItem.handler, new SGetResearchItem.Handler(SGetResearchItemHandle));
		SResearchError.handler = (SResearchError.Handler)Delegate.Combine(SResearchError.handler, new SResearchError.Handler(SWorkbenchErrorHandle));
	}

	private void SWorkbenchErrorHandle(SResearchError msg)
	{
		if (msg.code < 13)
		{
			AlertBox.Show(103 + msg.code);
		}
		else
		{
			AlertBox.Show(118);
		}
	}

	private void SGetResearchItemHandle(SGetResearchItem msg)
	{
	}

	private void SResearchInfoHandle(SResearchInfo msg)
	{
	}

	public void ResearchInfo(long researchId)
	{
		_cResearchInfo.researchId = researchId;
		Client2Gs.Ins.Send(_cResearchInfo);
	}

	public void StartResearch(long researchId, int addInstanceId, bool isAddAssist)
	{
		_cStartResearch.researchId = researchId;
		_cStartResearch.addInstanceId = addInstanceId;
		_cStartResearch.isAddAssist = isAddAssist;
		Client2Gs.Ins.Send(_cStartResearch);
	}

	public void CancelResearch(long researchId)
	{
		_cCancelResearch.researchId = researchId;
		Client2Gs.Ins.Send(_cCancelResearch);
	}

	public void GetResearchItem(long researchId)
	{
		_cGetResearchItem.researchId = researchId;
		Client2Gs.Ins.Send(_cGetResearchItem);
	}

	public void CreateResearch()
	{
	}
}
