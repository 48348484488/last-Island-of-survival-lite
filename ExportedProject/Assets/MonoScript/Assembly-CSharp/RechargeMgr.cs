using System;
using System.Collections;
using SC.UI;
using UnityEngine;
using gs.online.scmsg;

public class RechargeMgr : Singleton<RechargeMgr>
{
	private int _serverIdForCharge;

	public int realSencondFromServer;

	public void Init()
	{
		SServerInfo.handler = (SServerInfo.Handler)Delegate.Combine(SServerInfo.handler, new SServerInfo.Handler(OnSServerInfo));
	}

	private void OnSServerInfo(SServerInfo msg)
	{
		_serverIdForCharge = msg.serverId;
		RechargeEvent.OnReceiveServerTimeAction(msg.time);
		realSencondFromServer = msg.time - msg.serverOpenTime;
		Utils.StartConroutine(Tick());
	}

	public void ShowRecharge()
	{
		ShowRecharge(false);
	}

	public void ShowRecharge(bool hideParent)
	{
		if (ViewMgr.Ins.IsShow<ShoppingPanel>())
		{
			if (ShopEvent.JumpToRechargePage != null)
			{
				ShopEvent.JumpToRechargePage();
			}
		}
		else
		{
			ViewMgr.Ins.ShowView<ShoppingPanel>(-1, hideParent);
		}
	}

	private IEnumerator Tick()
	{
		while (true)
		{
			realSencondFromServer++;
			yield return new WaitForSeconds(1f);
		}
	}
}
