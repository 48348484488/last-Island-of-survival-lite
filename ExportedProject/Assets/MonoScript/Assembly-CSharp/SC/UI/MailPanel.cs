using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.mail.scmsg;

namespace SC.UI
{
	public class MailPanel : View
	{
		private MailInfo curMailInfo;

		private UIScrollPanel mailListPanel;

		private float widthX_half;

		private Vector3 v3;

		private GameObject m_notMailInfo;

		private GameObject m_hasMailInfo;

		private GameObject m_mailOneInfo;

		private GameObject txt_mailName;

		private Text txt_mailNameText;

		private GameObject txt_content;

		private Text txt_contentText;

		private GameObject m_mailAward;

		private GameObject list_reward;

		private GameObject btn_yetGetAward;

		private GameObject btn_canGetAward;

		private GameObject scp_mailList;

		private MailListCell m_cell;

		private GameObject txt_mailNum_max;

		private Text txt_mailNum_maxText;

		private GameObject btn_allGetAward;

		private GameObject btn_allDelete;

		private GameObject btn_back;

		private GameObject txt_blood;

		private Text txt_bloodText;

		private GameObject txt_hunger;

		private Text txt_hungerText;

		private GameObject txt_thirst;

		private Text txt_thirstText;

		[CompilerGenerated]
		private static Predicate<MailInfo> _003C_003Ef__am_0024cache0;

		[CompilerGenerated]
		private static Predicate<MailInfo> _003C_003Ef__am_0024cache1;

		[CompilerGenerated]
		private static Predicate<MailInfo> _003C_003Ef__am_0024cache2;

		protected override void onInit()
		{
			mailListPanel = scp_mailList.GetComponent<UIScrollPanel>();
			UIEventListener.Get(btn_back, "ui_close").onClick = _003ConInit_003Em__0;
			UIEventListener.Get(btn_allGetAward, string.Empty).onClick = OnGetAllAward;
			UIEventListener.Get(btn_allDelete, string.Empty).onClick = OnDeleteAllRead;
			UIEventListener.Get(btn_canGetAward, string.Empty).onClick = OnGetReward;
			v3 = list_reward.transform.localPosition;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			if (MailMgr.Ins.sMailInfo == null)
			{
				Hide();
				return;
			}
			MailMgr.Ins.Sort();
			curMailInfo = ((MailMgr.Ins.sMailInfo.infos.Count <= 0) ? null : MailMgr.Ins.sMailInfo.infos[0]);
			FillMailList();
			FillMailInfo();
			View.SetLabelText(txt_mailNum_max, MailMgr.Ins.sMailInfo.infos.Count + "/" + cfg.Consts.MAX_MAIL_KEEP_NUM);
			OnMoneyChange();
			MailMgr ins = MailMgr.Ins;
			ins.MailNumChange = (Utils.VoidDelegate)Delegate.Combine(ins.MailNumChange, new Utils.VoidDelegate(OnMailNumChange));
			MailMgr ins2 = MailMgr.Ins;
			ins2.MailContentChange = (Utils.VoidDelegate)Delegate.Combine(ins2.MailContentChange, new Utils.VoidDelegate(OnMailContentChange));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Combine(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
		}

		private void OnDeleteAllRead(GameObject go)
		{
			List<MailInfo> infos = MailMgr.Ins.sMailInfo.infos;
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003COnDeleteAllRead_003Em__1;
			}
			if (infos.Find(_003C_003Ef__am_0024cache0) != null)
			{
				Client2Gs.Ins.Send(new CDeletaAllReaded());
			}
			else if (MailMgr.Ins.sMailInfo.infos.Count > 0)
			{
				AlertBox.Show(300);
			}
		}

		private void OnGetReward(GameObject go)
		{
			if (curMailInfo != null && curMailInfo.canGetReward)
			{
				if (!Singleton<BagMgr>.Ins.IsRemainCapacity())
				{
					AlertBox.Show(26);
					return;
				}
				CGetMailReward cGetMailReward = new CGetMailReward();
				cGetMailReward.id = curMailInfo.id;
				Client2Gs.Ins.Send(cGetMailReward);
			}
		}

		private void OnGetAllAward(GameObject go)
		{
			List<MailInfo> infos = MailMgr.Ins.sMailInfo.infos;
			if (_003C_003Ef__am_0024cache1 == null)
			{
				_003C_003Ef__am_0024cache1 = _003COnGetAllAward_003Em__2;
			}
			if (infos.Find(_003C_003Ef__am_0024cache1) != null)
			{
				if (!Singleton<BagMgr>.Ins.IsRemainCapacity())
				{
					AlertBox.Show(26);
				}
				else
				{
					Client2Gs.Ins.Send(new CGetAllMailReward());
				}
			}
			else
			{
				AlertBox.Show(385);
			}
		}

		private void OnAllDelete(GameObject go)
		{
			List<MailInfo> infos = MailMgr.Ins.sMailInfo.infos;
			if (_003C_003Ef__am_0024cache2 == null)
			{
				_003C_003Ef__am_0024cache2 = _003COnAllDelete_003Em__3;
			}
			if (infos.Find(_003C_003Ef__am_0024cache2) != null)
			{
				CDeletaAllReaded msg = new CDeletaAllReaded();
				Client2Gs.Ins.Send(msg);
			}
		}

		private void OnMailNumChange()
		{
			if (!MailMgr.Ins.sMailInfo.infos.Contains(curMailInfo))
			{
				curMailInfo = ((MailMgr.Ins.sMailInfo.infos.Count <= 0) ? null : MailMgr.Ins.sMailInfo.infos[0]);
			}
			FillMailList();
			FillMailInfo();
			View.SetLabelText(txt_mailNum_max, MailMgr.Ins.sMailInfo.infos.Count + "/" + cfg.Consts.MAX_MAIL_KEEP_NUM);
		}

		private void OnMailContentChange()
		{
			mailListPanel.UpdateAllCell(FillMailListOne);
			FillMailInfo();
		}

		private void FillMailList()
		{
			scp_mailList.SetActive(MailMgr.Ins.sMailInfo.infos.Count > 0);
			mailListPanel.Reset(MailMgr.Ins.sMailInfo.infos.Count, FillMailListOne);
		}

		private void FillMailListOne(GameObject go, int index)
		{
			MailInfo mailInfo = MailMgr.Ins.sMailInfo.infos[index];
			MailListCell component = go.GetComponent<MailListCell>();
			component.m_yidu.SetActive(mailInfo.isRead);
			component.m_weidu.SetActive(!mailInfo.isRead);
			component.m_isSelect.SetActive(mailInfo.id == curMailInfo.id);
			component.m_red.SetActive(!mailInfo.isRead || mailInfo.canGetReward);
			View.SetLabelText(component.txt_mailName, mailInfo.title);
			string text = Utils.ConvertJavaTime(mailInfo.timeCreate, "yyyy-MM-dd");
			string strB = DateTime.Today.ToString("yyyy-MM-dd");
			View.SetLabelText(component.txt_timeHour, (string.CompareOrdinal(text, strB) != 0) ? text : Utils.ConvertJavaTime(mailInfo.timeCreate, "HH:mm"));
			View.SetLabelText(component.txt_from, mailInfo.senderName);
			UIContext.Attach(go, mailInfo);
			ClickListener.Get(go, string.Empty).onClick = OnClickMailListOne;
		}

		private void OnClickMailListOne(GameObject go)
		{
			MailInfo mailInfo = UIContext.Get<MailInfo>(go);
			if (curMailInfo != mailInfo)
			{
				curMailInfo = mailInfo;
				mailListPanel.UpdateAllCell(FillMailListOne);
				FillMailInfo();
			}
		}

		private void FillMailInfo()
		{
			m_notMailInfo.SetActive(curMailInfo == null);
			m_hasMailInfo.SetActive(curMailInfo != null);
			if (curMailInfo != null)
			{
				if (!curMailInfo.isRead)
				{
					curMailInfo.isRead = true;
					CReadMail cReadMail = new CReadMail();
					cReadMail.id = curMailInfo.id;
					Client2Gs.Ins.Send(cReadMail);
					mailListPanel.UpdateAllCell(FillMailListOne);
				}
				View.SetLabelText(txt_mailName, curMailInfo.title);
				View.SetLabelText(txt_content, curMailInfo.content);
				if (Singleton<DropMgr>.Ins.isEmpty(curMailInfo.dropDetail))
				{
					m_mailAward.SetActive(false);
					return;
				}
				m_mailAward.SetActive(true);
				List<DropMgr.DropDesInfo> dropDetailInfo = Singleton<DropMgr>.Ins.GetDropDetailInfo(curMailInfo.dropDetail);
				ItemHead.FillList(list_reward, dropDetailInfo);
				list_reward.SetActive(curMailInfo.canGetReward);
				btn_canGetAward.SetActive(curMailInfo.canGetReward);
				btn_yetGetAward.SetActive(!curMailInfo.canGetReward);
			}
		}

		protected override void onHide(string childView = null)
		{
			MailMgr ins = MailMgr.Ins;
			ins.MailNumChange = (Utils.VoidDelegate)Delegate.Remove(ins.MailNumChange, new Utils.VoidDelegate(OnMailNumChange));
			MailMgr ins2 = MailMgr.Ins;
			ins2.MailContentChange = (Utils.VoidDelegate)Delegate.Remove(ins2.MailContentChange, new Utils.VoidDelegate(OnMailContentChange));
			RoleEvent.MoneyChangeDelegate = (Utils.VoidDelegate)Delegate.Remove(RoleEvent.MoneyChangeDelegate, new Utils.VoidDelegate(OnMoneyChange));
			MailMgr.Ins.updateRedDotCount();
		}

		private void OnMoneyChange()
		{
			View.SetLabelText(txt_bloodText, Singleton<RoleMgr>.Ins.Blood);
			View.SetLabelText(txt_hungerText, Singleton<RoleMgr>.Ins.Hunger);
			View.SetLabelText(txt_thirstText, Singleton<RoleMgr>.Ins.Water);
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			m_notMailInfo = component.GameObjects[0].gameObject;
			m_hasMailInfo = component.GameObjects[1].gameObject;
			m_mailOneInfo = component.GameObjects[2].gameObject;
			txt_mailName = component.GameObjects[3].gameObject;
			txt_mailNameText = txt_mailName.GetComponent<Text>();
			txt_content = component.GameObjects[4].gameObject;
			txt_contentText = txt_content.GetComponent<Text>();
			m_mailAward = component.GameObjects[5].gameObject;
			list_reward = component.GameObjects[6].gameObject;
			btn_yetGetAward = component.GameObjects[7].gameObject;
			btn_canGetAward = component.GameObjects[8].gameObject;
			scp_mailList = component.GameObjects[9].gameObject;
			m_cell = View.AddComponentIfNotExist<MailListCell>(component.GameObjects[10].gameObject);
			txt_mailNum_max = component.GameObjects[11].gameObject;
			txt_mailNum_maxText = txt_mailNum_max.GetComponent<Text>();
			btn_allGetAward = component.GameObjects[12].gameObject;
			btn_allDelete = component.GameObjects[13].gameObject;
			btn_back = component.GameObjects[14].gameObject;
			txt_blood = component.GameObjects[15].gameObject;
			txt_bloodText = txt_blood.GetComponent<Text>();
			txt_hunger = component.GameObjects[16].gameObject;
			txt_hungerText = txt_hunger.GetComponent<Text>();
			txt_thirst = component.GameObjects[17].gameObject;
			txt_thirstText = txt_thirst.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private static bool _003COnDeleteAllRead_003Em__1(MailInfo info)
		{
			return info.isRead && !info.canGetReward;
		}

		[CompilerGenerated]
		private static bool _003COnGetAllAward_003Em__2(MailInfo info)
		{
			return info.canGetReward;
		}

		[CompilerGenerated]
		private static bool _003COnAllDelete_003Em__3(MailInfo info)
		{
			return info.isRead && !info.canGetReward;
		}
	}
}
