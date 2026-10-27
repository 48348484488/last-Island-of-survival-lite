using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.battle.scmsg;
using gs.friends.scmsg;

namespace SC.UI
{
	public class FriendPermitPanel : View
	{
		[CompilerGenerated]
		private sealed class _003CUpdateLingDiGuiBtn_003Ec__AnonStorey0
		{
			internal int i1;

			internal FriendPermitPanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				_0024this.ChangeLingDiGui(i1);
			}
		}

		[CompilerGenerated]
		private sealed class _003CFillFriendCell_003Ec__AnonStorey1
		{
			internal Toggle tog;

			internal RoleInfo roleInfo;

			internal FriendPermitPanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (tog.isOn && cfg.Consts.MANOR_CHEST_MAX_SHARE_NUMBER <= _0024this._permit.roleIds.Count)
				{
					AlertBox.Show(244, cfg.Consts.MANOR_CHEST_MAX_SHARE_NUMBER);
					tog.isOn = !tog.isOn;
				}
				else
				{
					Singleton<FriendPermitMgr>.Ins.SendGivePermitMsg(_0024this._permit.instanceId, roleInfo.info.roleId, tog.isOn);
				}
			}
		}

		private RadioButton _attention;

		private UIScrollPanel _roleScrollPanel;

		private OneChestPermit _permit;

		private GameObject btn_close;

		private GameObject scp_permit;

		private GFriendPermitCell m_cell;

		private GameObject btn_attention;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_fans;

		private List<GManorNameGroup> m_manorlist = new List<GManorNameGroup>();

		private GameObject[] m_manor;

		private GameObject m_manorObj;

		private GameObject btn_give_all;

		private GameObject btn_cancel_all;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		protected override void onInit()
		{
			base.onInit();
			m_cell.gameObject.SetActiveBetter(false);
			Singleton<FriendScMgr>.Ins.SendRequestMsg();
			_attention = btn_attention.GetComponent<RadioButton>();
			_roleScrollPanel = scp_permit.GetComponent<UIScrollPanel>();
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_attention, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_fans, string.Empty).onClick = _003ConInit_003Em__2;
			ClickListener.Get(btn_give_all, string.Empty).onClick = _003ConInit_003Em__3;
			ClickListener.Get(btn_cancel_all, string.Empty).onClick = _003ConInit_003Em__4;
		}

		private void GiveOrCancelPermit(bool isGive)
		{
			if (_attention.isChecked)
			{
				List<CareRoleInfo> careList = Singleton<FriendScMgr>.Ins.CareList;
				int num = 0;
				int i = 0;
				for (int count = careList.Count; i < count; i++)
				{
					if (isGive && cfg.Consts.MANOR_CHEST_MAX_SHARE_NUMBER <= _permit.roleIds.Count + num)
					{
						break;
					}
					if (isGive != _permit.roleIds.Contains(careList[i].roleInfo.info.roleId))
					{
						Singleton<FriendPermitMgr>.Ins.SendGivePermitMsg(_permit.instanceId, careList[i].roleInfo.info.roleId, isGive);
						num++;
					}
				}
				return;
			}
			List<FansRoleInfo> fansList = Singleton<FriendScMgr>.Ins.FansList;
			int num2 = 0;
			int j = 0;
			for (int count2 = fansList.Count; j < count2; j++)
			{
				if (isGive && cfg.Consts.MANOR_CHEST_MAX_SHARE_NUMBER <= _permit.roleIds.Count + num2)
				{
					break;
				}
				if (isGive != _permit.roleIds.Contains(fansList[j].roleInfo.info.roleId))
				{
					Singleton<FriendPermitMgr>.Ins.SendGivePermitMsg(_permit.instanceId, fansList[j].roleInfo.info.roleId, isGive);
					num2++;
				}
			}
		}

		protected override void onShow(object param = null, string childView = null)
		{
			base.onShow(param, childView);
			UpdateLingDiGuiBtn();
			if (param == null)
			{
				ChangeLingDiGui(0);
			}
			else
			{
				int index = 0;
				bool flag = false;
				long num = (long)param;
				List<OneChestPermit> chestPermits = Singleton<FriendPermitMgr>.Ins.PermitInfo.ChestPermits;
				int i = 0;
				for (int count = chestPermits.Count; i < count; i++)
				{
					if (chestPermits[i].instanceId == num)
					{
						flag = true;
						index = i;
						break;
					}
				}
				if (!flag)
				{
					MessageBoxPanel.Show(243);
				}
				ChangeLingDiGui(index);
			}
			SGivePermit.handler = (SGivePermit.Handler)Delegate.Combine(SGivePermit.handler, new SGivePermit.Handler(OnSGivePermit));
			SFriendInfo.handler = (SFriendInfo.Handler)Delegate.Combine(SFriendInfo.handler, new SFriendInfo.Handler(OnSFriendInfo));
		}

		private void OnSFriendInfo(SFriendInfo msg)
		{
			UpdateFriend(true);
		}

		private void OnSGivePermit(SGivePermit msg)
		{
			UpdateFriend(true);
		}

		private void UpdateLingDiGuiBtn()
		{
			List<OneChestPermit> chestPermits = Singleton<FriendPermitMgr>.Ins.PermitInfo.ChestPermits;
			int i = 0;
			for (int num = m_manor.Length; i < num; i++)
			{
				if (chestPermits.Count > i)
				{
					_003CUpdateLingDiGuiBtn_003Ec__AnonStorey0 _003CUpdateLingDiGuiBtn_003Ec__AnonStorey = new _003CUpdateLingDiGuiBtn_003Ec__AnonStorey0();
					_003CUpdateLingDiGuiBtn_003Ec__AnonStorey._0024this = this;
					m_manor[i].SetActiveBetter(true);
					_003CUpdateLingDiGuiBtn_003Ec__AnonStorey.i1 = i;
					ClickListener.Get(m_manor[i], string.Empty).onClick = _003CUpdateLingDiGuiBtn_003Ec__AnonStorey._003C_003Em__0;
					OneChestPermit oneChestPermit = Singleton<FriendPermitMgr>.Ins.PermitInfo.ChestPermits[i];
					if (!string.IsNullOrEmpty(oneChestPermit.name))
					{
						View.SetLabelText(m_manorlist[i].txt_offText, oneChestPermit.name);
						View.SetLabelText(m_manorlist[i].txt_onText, oneChestPermit.name);
					}
					else
					{
						BuildPart buildPart = BuildPart.Get(cfg.Consts.MANOR_CHEST_ITEM_ID);
						View.SetLabelText(m_manorlist[i].txt_offText, buildPart.name);
						View.SetLabelText(m_manorlist[i].txt_onText, buildPart.name);
					}
				}
				else
				{
					m_manor[i].SetActiveBetter(false);
				}
			}
		}

		private void ChangeLingDiGui(int index)
		{
			RadioButton.ChooseBtn(btn_attention);
			if (Singleton<FriendPermitMgr>.Ins.PermitInfo.ChestPermits.Count > index)
			{
				_permit = Singleton<FriendPermitMgr>.Ins.PermitInfo.ChestPermits[index];
				UpdateFriend(false);
			}
		}

		private void UpdateFriend(bool isNoPos)
		{
			int cellNum = ((!_attention.isChecked) ? Singleton<FriendScMgr>.Ins.FansList.Count : Singleton<FriendScMgr>.Ins.CareList.Count);
			UIScrollPanel.FillCell fillFunc = ((!_attention.isChecked) ? new UIScrollPanel.FillCell(FillFansCell) : new UIScrollPanel.FillCell(FillCareCell));
			if (isNoPos)
			{
				_roleScrollPanel.ResetNoPosClear(cellNum, fillFunc);
			}
			else
			{
				_roleScrollPanel.Reset(cellNum, fillFunc);
			}
		}

		private void FillFansCell(GameObject go, int index)
		{
			GFriendPermitCell component = go.GetComponent<GFriendPermitCell>();
			RoleInfo roleInfo = Singleton<FriendScMgr>.Ins.FansList[index].roleInfo;
			FillFriendCell(component, roleInfo);
		}

		private void FillCareCell(GameObject go, int index)
		{
			GFriendPermitCell component = go.GetComponent<GFriendPermitCell>();
			RoleInfo roleInfo = Singleton<FriendScMgr>.Ins.CareList[index].roleInfo;
			FillFriendCell(component, roleInfo);
		}

		private void FillFriendCell(GFriendPermitCell cell, RoleInfo roleInfo)
		{
			_003CFillFriendCell_003Ec__AnonStorey1 _003CFillFriendCell_003Ec__AnonStorey = new _003CFillFriendCell_003Ec__AnonStorey1();
			_003CFillFriendCell_003Ec__AnonStorey.roleInfo = roleInfo;
			_003CFillFriendCell_003Ec__AnonStorey._0024this = this;
			View.SetItemSprite(cell.m_icon, RoleHeadCfg.Get(_003CFillFriendCell_003Ec__AnonStorey.roleInfo.info.headId).icon);
			View.SetLabelText(cell.txt_nameText, _003CFillFriendCell_003Ec__AnonStorey.roleInfo.info.name);
			_003CFillFriendCell_003Ec__AnonStorey.tog = cell.ckb_permit.GetComponent<Toggle>();
			_003CFillFriendCell_003Ec__AnonStorey.tog.isOn = _permit.roleIds.Contains(_003CFillFriendCell_003Ec__AnonStorey.roleInfo.info.roleId);
			ClickListener.Get(cell.ckb_permit, string.Empty).onClick = _003CFillFriendCell_003Ec__AnonStorey._003C_003Em__0;
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			SGivePermit.handler = (SGivePermit.Handler)Delegate.Remove(SGivePermit.handler, new SGivePermit.Handler(OnSGivePermit));
			SFriendInfo.handler = (SFriendInfo.Handler)Delegate.Remove(SFriendInfo.handler, new SFriendInfo.Handler(OnSFriendInfo));
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			scp_permit = component.GameObjects[1].gameObject;
			m_cell = View.AddComponentIfNotExist<GFriendPermitCell>(component.GameObjects[2].gameObject);
			btn_attention = component.GameObjects[3].gameObject;
			txt_on = component.GameObjects[4].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[5].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_fans = component.GameObjects[6].gameObject;
			m_manor = component.GameObjects[7].gameObject.GetComponent<UIGameObjectList>().objects;
			m_manorObj = component.GameObjects[7].gameObject;
			if (m_manorlist.Count <= 0)
			{
				for (int i = 0; i < m_manor.Length; i++)
				{
					m_manorlist.Add(View.AddComponentIfNotExist<GManorNameGroup>(m_manor[i].gameObject));
				}
			}
			btn_give_all = component.GameObjects[8].gameObject;
			btn_cancel_all = component.GameObjects[9].gameObject;
			txt_on_0 = component.GameObjects[10].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[11].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			UpdateFriend(false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			UpdateFriend(false);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__3(GameObject go)
		{
			GiveOrCancelPermit(true);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__4(GameObject go)
		{
			GiveOrCancelPermit(false);
		}
	}
}
