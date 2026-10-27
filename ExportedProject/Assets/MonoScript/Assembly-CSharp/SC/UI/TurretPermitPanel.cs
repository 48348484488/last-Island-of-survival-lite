using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using gs.battle.map.turret.scmsg;
using gs.friends.scmsg;

namespace SC.UI
{
	public class TurretPermitPanel : View
	{
		private enum TurretPermitPafeType
		{
			Care = 0,
			Fans = 1
		}

		[CompilerGenerated]
		private sealed class _003CFillFriendCell_003Ec__AnonStorey0
		{
			internal Toggle tog;

			internal RoleInfo roleInfo;

			internal TurretPermitPanel _0024this;

			internal void _003C_003Em__0(GameObject go)
			{
				if (tog.isOn && cfg.Consts.MANOR_CHEST_MAX_SHARE_NUMBER <= _0024this._permitTemp.Count)
				{
					AlertBox.Show(244, cfg.Consts.MANOR_CHEST_MAX_SHARE_NUMBER);
					tog.isOn = !tog.isOn;
				}
				else if (tog.isOn && !_0024this._permitTemp.Contains(roleInfo.info.roleId))
				{
					_0024this._permitTemp.Add(roleInfo.info.roleId);
				}
			}
		}

		private RadioButton _attention;

		private Toggle _allToggle;

		private UIScrollPanel _roleScrollPanel;

		private STurretBuildingInfo _turretBuildingInfo;

		private List<long> _permitTemp = new List<long>();

		private TurretPermitPafeType _turretPermitPafeType;

		private GameObject btn_close;

		private GameObject scp_permit;

		private GTurretPermitItem m_cell;

		private GameObject btn_attention;

		private GameObject txt_on;

		private Text txt_onText;

		private GameObject txt_off;

		private Text txt_offText;

		private GameObject btn_fans;

		private GameObject txt_on_0;

		private Text txt_on_0Text;

		private GameObject txt_off_0;

		private Text txt_off_0Text;

		protected override void onInit()
		{
			base.onInit();
			Singleton<FriendScMgr>.Ins.SendRequestMsg();
			m_cell.gameObject.SetActiveBetter(false);
			_attention = btn_attention.GetComponent<RadioButton>();
			_roleScrollPanel = scp_permit.GetComponent<UIScrollPanel>();
			ClickListener.Get(btn_close, string.Empty).onClick = _003ConInit_003Em__0;
			ClickListener.Get(btn_attention, string.Empty).onClick = _003ConInit_003Em__1;
			ClickListener.Get(btn_fans, string.Empty).onClick = _003ConInit_003Em__2;
		}

		protected override void onShow(object param = null, string childView = null)
		{
			_permitTemp.Clear();
			base.onShow(param, childView);
			_turretBuildingInfo = param as STurretBuildingInfo;
			for (int i = 0; i < _turretBuildingInfo.permissionsList.Count; i++)
			{
				_permitTemp.Add(_turretBuildingInfo.permissionsList[i]);
			}
			STurretBuildingInfo.handler = (STurretBuildingInfo.Handler)Delegate.Combine(STurretBuildingInfo.handler, new STurretBuildingInfo.Handler(STurretBuildingInfoHandle));
			SFriendInfo.handler = (SFriendInfo.Handler)Delegate.Combine(SFriendInfo.handler, new SFriendInfo.Handler(SFriendInfoHandle));
			UpdateFriend();
		}

		private void SFriendInfoHandle(SFriendInfo msg)
		{
			UpdateFriend();
		}

		private void STurretBuildingInfoHandle(STurretBuildingInfo msg)
		{
			UpdateFriend();
		}

		public void UpdateFriend()
		{
			_roleScrollPanel.Clear();
			if (_turretPermitPafeType == TurretPermitPafeType.Care)
			{
				_roleScrollPanel.Reset(Singleton<FriendScMgr>.Ins.CareList.Count, FillCareCell);
			}
			else if (_turretPermitPafeType == TurretPermitPafeType.Fans)
			{
				_roleScrollPanel.Reset(Singleton<FriendScMgr>.Ins.FansList.Count, FillFansCell);
			}
		}

		private void ChangePage(TurretPermitPafeType turretPermitPafeType)
		{
			_turretPermitPafeType = turretPermitPafeType;
			UpdateFriend();
		}

		private void FillFansCell(GameObject go, int index)
		{
			GTurretPermitItem component = go.GetComponent<GTurretPermitItem>();
			RoleInfo roleInfo = Singleton<FriendScMgr>.Ins.FansList[index].roleInfo;
			FillFriendCell(component, roleInfo);
		}

		private void FillCareCell(GameObject go, int index)
		{
			GTurretPermitItem component = go.GetComponent<GTurretPermitItem>();
			RoleInfo roleInfo = Singleton<FriendScMgr>.Ins.CareList[index].roleInfo;
			FillFriendCell(component, roleInfo);
		}

		private void FillFriendCell(GTurretPermitItem cell, RoleInfo roleInfo)
		{
			_003CFillFriendCell_003Ec__AnonStorey0 _003CFillFriendCell_003Ec__AnonStorey = new _003CFillFriendCell_003Ec__AnonStorey0();
			_003CFillFriendCell_003Ec__AnonStorey.roleInfo = roleInfo;
			_003CFillFriendCell_003Ec__AnonStorey._0024this = this;
			RoleHeadCfg roleHeadCfg = RoleHeadCfg.Get(_003CFillFriendCell_003Ec__AnonStorey.roleInfo.info.headId);
			if (roleHeadCfg != null)
			{
				View.SetItemSprite(cell.m_icon, roleHeadCfg.icon);
			}
			View.SetLabelText(cell.txt_nameText, _003CFillFriendCell_003Ec__AnonStorey.roleInfo.info.name);
			_003CFillFriendCell_003Ec__AnonStorey.tog = cell.ckb_permit.GetComponent<Toggle>();
			_003CFillFriendCell_003Ec__AnonStorey.tog.isOn = _permitTemp.Contains(_003CFillFriendCell_003Ec__AnonStorey.roleInfo.info.roleId);
			ClickListener.Get(cell.ckb_permit, string.Empty).onClick = _003CFillFriendCell_003Ec__AnonStorey._003C_003Em__0;
		}

		protected override void onHide(string childView = null)
		{
			base.onHide(childView);
			STurretBuildingInfo.handler = (STurretBuildingInfo.Handler)Delegate.Remove(STurretBuildingInfo.handler, new STurretBuildingInfo.Handler(STurretBuildingInfoHandle));
			SFriendInfo.handler = (SFriendInfo.Handler)Delegate.Remove(SFriendInfo.handler, new SFriendInfo.Handler(SFriendInfoHandle));
		}

		protected override void Awake()
		{
			base.Awake();
			GameObjectData component = base.gameObject.GetComponent<GameObjectData>();
			btn_close = component.GameObjects[0].gameObject;
			scp_permit = component.GameObjects[1].gameObject;
			m_cell = View.AddComponentIfNotExist<GTurretPermitItem>(component.GameObjects[2].gameObject);
			btn_attention = component.GameObjects[3].gameObject;
			txt_on = component.GameObjects[4].gameObject;
			txt_onText = txt_on.GetComponent<Text>();
			txt_off = component.GameObjects[5].gameObject;
			txt_offText = txt_off.GetComponent<Text>();
			btn_fans = component.GameObjects[6].gameObject;
			txt_on_0 = component.GameObjects[7].gameObject;
			txt_on_0Text = txt_on_0.GetComponent<Text>();
			txt_off_0 = component.GameObjects[8].gameObject;
			txt_off_0Text = txt_off_0.GetComponent<Text>();
			ViewMgr.Ins.addView(this);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__0(GameObject go)
		{
			Hide();
			Singleton<TurretMgr>.Ins.AddPermissions(_turretBuildingInfo.turretId, _permitTemp);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__1(GameObject go)
		{
			ChangePage(TurretPermitPafeType.Care);
		}

		[CompilerGenerated]
		private void _003ConInit_003Em__2(GameObject go)
		{
			ChangePage(TurretPermitPafeType.Fans);
		}
	}
}
