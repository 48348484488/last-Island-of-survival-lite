using System;
using UnityEngine;
using cfg;

namespace SC.UI
{
	public class ActivityFirstChargePage : MonoBehaviour, IActivityPage
	{
		private bool _initItemCells;

		public GActivityFirstChargeCell m_charge_gift;

		public object context;

		public GameObject ThisGo { get; private set; }

		public void OnInit()
		{
			ThisGo = base.gameObject;
		}

		public void OnShow(object param)
		{
			if (!_initItemCells)
			{
				InitAllGiftCell();
				_initItemCells = true;
				Singleton<ActivityMgr>.Ins.SetSeeFirstChargeTrue();
			}
			else
			{
				ResetAllGiftBtn();
			}
			ActivityEvent.UpdateActivityRedDot = (Action)Delegate.Combine(ActivityEvent.UpdateActivityRedDot, new Action(ResetAllGiftBtn));
		}

		public void OnHide()
		{
			ActivityEvent.UpdateActivityRedDot = (Action)Delegate.Remove(ActivityEvent.UpdateActivityRedDot, new Action(ResetAllGiftBtn));
		}

		private void InitAllGiftCell()
		{
			ActivityViewTool.SetFirstChargeCell(m_charge_gift, Singleton<DropMgr>.Ins.GetDropDetailInfo(cfg.Consts.FIRST_CHARGE_DROPID));
		}

		private void ResetAllGiftBtn()
		{
			ResetBtn(m_charge_gift);
		}

		private void ResetBtn(GActivityFirstChargeCell cell)
		{
			bool flag = Singleton<ActivityMgr>.Ins.IsAlreadyGotFirstCharge();
			cell.btn_buy.SetActiveBetter(!flag);
			cell.btn_already_got.SetActiveBetter(flag);
		}

		private void Awake()
		{
			m_charge_gift = View.AddComponentIfNotExist<GActivityFirstChargeCell>(base.transform.Find("GameObject (1)/m_charge_gift").gameObject);
		}
	}
}
