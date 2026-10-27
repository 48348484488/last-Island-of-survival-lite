using System;
using System.Runtime.CompilerServices;
using gs.mail.scmsg;

public sealed class MailMgr
{
	[CompilerGenerated]
	private sealed class _003COnSDeletaAllReaded_003Ec__AnonStorey0
	{
		internal SDeletaAllReaded msg;

		internal bool _003C_003Em__0(MailInfo info)
		{
			return msg.ids.Contains(info.id);
		}
	}

	public static MailMgr Ins = new MailMgr();

	public Utils.VoidDelegate MailNumChange;

	public Utils.VoidDelegate MailContentChange;

	public Utils.IntDelegate RedDotCountChange;

	public SMailInfo sMailInfo;

	public int haveRedDotCount;

	private MailMgr()
	{
	}

	public void Init()
	{
		SMailInfo.handler = (SMailInfo.Handler)Delegate.Combine(SMailInfo.handler, new SMailInfo.Handler(OnSMailInfo));
		SAddmail.handler = (SAddmail.Handler)Delegate.Combine(SAddmail.handler, new SAddmail.Handler(OnSAddmail));
		SDeletaAllReaded.handler = (SDeletaAllReaded.Handler)Delegate.Combine(SDeletaAllReaded.handler, new SDeletaAllReaded.Handler(OnSDeletaAllReaded));
		SGetAllMailReward.handler = (SGetAllMailReward.Handler)Delegate.Combine(SGetAllMailReward.handler, new SGetAllMailReward.Handler(OnSGetAllMailReward));
		SGetMailReward.handler = (SGetMailReward.Handler)Delegate.Combine(SGetMailReward.handler, new SGetMailReward.Handler(OnSGetMailReward));
		MailNumChange = (Utils.VoidDelegate)Delegate.Combine(MailNumChange, new Utils.VoidDelegate(updateRedDotCount));
	}

	public void Sort()
	{
		if (sMailInfo != null)
		{
			sMailInfo.infos.Sort(_003CSort_003Em__0);
		}
	}

	public bool haveRedDot(MailInfo info)
	{
		return !info.isRead || info.canGetReward;
	}

	public void updateRedDotCount()
	{
		if (sMailInfo == null)
		{
			Utils.TriggerEvent(RedDotCountChange, 0);
			return;
		}
		int num = 0;
		foreach (MailInfo info in sMailInfo.infos)
		{
			if (haveRedDot(info))
			{
				num++;
			}
		}
		haveRedDotCount = num;
		Utils.TriggerEvent(RedDotCountChange, haveRedDotCount);
	}

	private void OnSDeletaAllReaded(SDeletaAllReaded msg)
	{
		_003COnSDeletaAllReaded_003Ec__AnonStorey0 _003COnSDeletaAllReaded_003Ec__AnonStorey = new _003COnSDeletaAllReaded_003Ec__AnonStorey0();
		_003COnSDeletaAllReaded_003Ec__AnonStorey.msg = msg;
		if (sMailInfo.infos.RemoveAll(_003COnSDeletaAllReaded_003Ec__AnonStorey._003C_003Em__0) > 0)
		{
			Utils.TriggerEvent(MailNumChange);
		}
	}

	private void OnSMailInfo(SMailInfo msg)
	{
		sMailInfo = msg;
		Utils.TriggerEvent(MailNumChange);
	}

	private void OnSGetAllMailReward(SGetAllMailReward msg)
	{
		foreach (MailInfo info in sMailInfo.infos)
		{
			if (info.canGetReward)
			{
				Singleton<GainMgr>.Ins.Add(info.dropDetail);
			}
			info.canGetReward = false;
			if (msg.isReads.Contains(info.id))
			{
				info.isRead = true;
			}
		}
		Utils.TriggerEvent(MailContentChange);
	}

	private void OnSGetMailReward(SGetMailReward msg)
	{
		foreach (MailInfo info in sMailInfo.infos)
		{
			if (msg.id == info.id)
			{
				info.isRead = true;
				if (info.canGetReward)
				{
					Singleton<GainMgr>.Ins.Add(info.dropDetail);
				}
				info.canGetReward = false;
				Utils.TriggerEvent(MailContentChange);
				break;
			}
		}
	}

	private void OnSAddmail(SAddmail msg)
	{
		if (sMailInfo != null)
		{
			sMailInfo.infos.Insert(0, msg.info);
			Utils.TriggerEvent(MailNumChange);
		}
	}

	[CompilerGenerated]
	private int _003CSort_003Em__0(MailInfo info1, MailInfo info2)
	{
		bool flag = haveRedDot(info1);
		bool flag2 = haveRedDot(info2);
		return (flag == flag2) ? (info2.id - info1.id) : ((!flag) ? 1 : (-1));
	}
}
