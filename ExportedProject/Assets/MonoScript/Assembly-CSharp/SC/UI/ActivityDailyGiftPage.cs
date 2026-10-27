using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.drop.scmsg;

namespace SC.UI
{
	public class ActivityDailyGiftPage : MonoBehaviour, IActivityPage
	{
		private List<DaylyGiftCfg> _allDailyGiftInfoList;

		private bool _initItemCells;

		public List<GActivityDailyGiftCell> m_giftslist = new List<GActivityDailyGiftCell>();

		public GameObject[] m_gifts;

		public GameObject m_giftsObj;

		public GameObject txt_time;

		public Text txt_timeText;

		public object context;

		public GameObject ThisGo { get; private set; }

		public void OnInit()
		{
			txt_time.SetActiveBetter(false);
			ThisGo = base.gameObject;
			_allDailyGiftInfoList = DaylyGiftCfg.GetAllList();
		}

		public void OnShow(object param)
		{
			if (!_initItemCells)
			{
				InitAllGiftCell();
				_initItemCells = true;
				Singleton<ActivityMgr>.Ins.SetSeeDailyGiftTrue();
			}
			else
			{
				ResetAllGiftBtn();
			}
			ActivityEvent.OnBuyDailyGiftAction = (Action<DropDetail>)Delegate.Combine(ActivityEvent.OnBuyDailyGiftAction, new Action<DropDetail>(ResetAllGiftBtn));
		}

		public void OnHide()
		{
			ActivityEvent.OnBuyDailyGiftAction = (Action<DropDetail>)Delegate.Remove(ActivityEvent.OnBuyDailyGiftAction, new Action<DropDetail>(ResetAllGiftBtn));
		}

		private void InitAllGiftCell()
		{
			int i = 0;
			for (int num = m_gifts.Length; i < num; i++)
			{
				ActivityViewTool.SetDailyGiftCell(m_giftslist[i], _allDailyGiftInfoList[i]);
			}
		}

		private void ResetAllGiftBtn()
		{
			int i = 0;
			for (int num = m_gifts.Length; i < num; i++)
			{
				ResetBtn(m_giftslist[i], _allDailyGiftInfoList[i]);
			}
		}

		private void ResetAllGiftBtn(DropDetail details)
		{
			Singleton<GainMgr>.Ins.Add(details);
			ResetAllGiftBtn();
		}

		private void ResetBtn(GActivityDailyGiftCell cell, DaylyGiftCfg cfgInfo)
		{
			bool flag = Singleton<ActivityMgr>.Ins.IsAlreadyGotDailyGift(cfgInfo.id);
			cell.btn_buy.SetActiveBetter(!flag);
			cell.btn_already_got.SetActiveBetter(flag);
		}

		private void Awake()
		{
			m_gifts = base.transform.Find("m_gifts").gameObject.GetComponent<UIGameObjectList>().objects;
			m_giftsObj = base.transform.Find("m_gifts").gameObject;
			if (m_giftslist.Count <= 0)
			{
				for (int i = 0; i < m_gifts.Length; i++)
				{
					m_giftslist.Add(View.AddComponentIfNotExist<GActivityDailyGiftCell>(m_gifts[i].gameObject));
				}
			}
			txt_time = base.transform.Find("GameObject/txt_time").gameObject;
			txt_timeText = txt_time.GetComponent<Text>();
		}
	}
}
