using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;
using gs.gm.scmsg;

namespace SC.UI
{
	public class ShopRechargePage : MonoBehaviour, IShopPage
	{
		[CompilerGenerated]
		private sealed class _003CFillRechargeCell_003Ec__AnonStorey0
		{
			internal ShopPriceInfo priceInfo;

			internal ShopRechargePage _0024this;

			internal void _003C_003Em__0(GameObject o)
			{
				int num = priceInfo.num + priceInfo.zengArg;
				CGMCommand msg = new CGMCommand
				{
					cmd = _0024this.AddMoney + num
				};
				Client2Gs.Ins.Send(msg);
			}
		}

		private readonly string AddMoney = "addmoney 2 ";

		private List<RechargeCfg> _rechargeInfoList = new List<RechargeCfg>();

		private UIScrollPanel _rechargePanel;

		public GShopRechargeCell m_cell;

		public GameObject scp_recharge;

		public object context;

		public GameObject ThisGo
		{
			get
			{
				return base.gameObject;
			}
		}

		public void OnInit()
		{
			m_cell.gameObject.SetActiveBetter(false);
			List<RechargeCfg> allList = RechargeCfg.GetAllList();
			int i = 0;
			for (int count = allList.Count; i < count; i++)
			{
				RechargeCfg rechargeCfg = allList[i];
				if (rechargeCfg.rechargeType == 1)
				{
					_rechargeInfoList.Add(rechargeCfg);
				}
			}
			_rechargePanel = scp_recharge.GetComponent<UIScrollPanel>();
		}

		public void OnShow(object param)
		{
			UpdateAllCell(false);
		}

		public void OnHide()
		{
		}

		private void UpdateAllCell(bool isNoPos)
		{
			if (isNoPos)
			{
				_rechargePanel.ResetNoPosClear(_rechargeInfoList.Count, FillRechargeCell);
			}
			else
			{
				_rechargePanel.Reset(_rechargeInfoList.Count, FillRechargeCell);
			}
		}

		private void FillRechargeCell(GameObject go, int index)
		{
			_003CFillRechargeCell_003Ec__AnonStorey0 _003CFillRechargeCell_003Ec__AnonStorey = new _003CFillRechargeCell_003Ec__AnonStorey0();
			_003CFillRechargeCell_003Ec__AnonStorey._0024this = this;
			GShopRechargeCell component = go.GetComponent<GShopRechargeCell>();
			RechargeCfg rechargeCfg = _rechargeInfoList[index];
			bool flag = Singleton<PlatformMgr>.Ins.IsOverSea();
			int num = 0;
			int index2;
			if (flag)
			{
				index2 = 0;
				num = 276;
			}
			else
			{
				index2 = 1;
				num = 275;
			}
			_003CFillRechargeCell_003Ec__AnonStorey.priceInfo = rechargeCfg.priceInfos[index2];
			View.SetLabelText(component.txt_token_numText, _003CFillRechargeCell_003Ec__AnonStorey.priceInfo.name);
			View.SetTexture(component.m_token_icon_big, rechargeCfg.icon);
			if (_003CFillRechargeCell_003Ec__AnonStorey.priceInfo.zengArg > 0)
			{
				component.m_zeng.SetActiveBetter(true);
				View.SetLabelText(component.txt_num_zengText, _003CFillRechargeCell_003Ec__AnonStorey.priceInfo.zengArg);
				Singleton<NormalShopMgr>.Ins.SetMoneyIcon(component.m_token_icon_zeng, _003CFillRechargeCell_003Ec__AnonStorey.priceInfo.zengType);
			}
			else
			{
				component.m_zeng.SetActiveBetter(false);
			}
			View.SetLabelText(component.txt_rmb_numText, Utils.GetString(num, _003CFillRechargeCell_003Ec__AnonStorey.priceInfo.price));
			ClickListener.Get(go, string.Empty).onClick = _003CFillRechargeCell_003Ec__AnonStorey._003C_003Em__0;
		}

		private void Awake()
		{
			m_cell = View.AddComponentIfNotExist<GShopRechargeCell>(base.transform.Find("zuo (1)/scp_recharge/content/m_cell").gameObject);
			scp_recharge = base.transform.Find("zuo (1)/scp_recharge").gameObject;
		}
	}
}
