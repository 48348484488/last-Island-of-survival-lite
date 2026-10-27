using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using cfg;
using gs.role.scmsg;

public class RoleHead : MonoBehaviour
{
	public GameObject frame;

	public GameObject icon;

	public GameObject playerName;

	public GameObject level;

	public GameObject male;

	public GameObject female;

	public GameObject vipLevel;

	[CompilerGenerated]
	private static ClickListener.VoidDelegate _003C_003Ef__am_0024cache0;

	public void Fill(int frameId, int headId, int rLevel, string rName = "404")
	{
		if (frame != null)
		{
		}
		if (icon != null)
		{
			if (RoleHeadCfg.Get(headId) == null)
			{
				return;
			}
			View.SetItemSprite(icon, RoleHeadCfg.Get(headId).icon);
		}
		if (level != null)
		{
			View.SetLabelText(level, rLevel);
		}
		if (playerName != null)
		{
			View.SetLabelText(playerName, rName, false);
		}
	}

	public void Fill(BasicRoleInfo info, int vipFormat = 0)
	{
		if (frame != null)
		{
			try
			{
				if (info.frameId > 0)
				{
					View.SetItemSprite(frame, RoleHeadFrameCfg.Get(info.frameId).frame);
				}
			}
			catch (Exception)
			{
			}
		}
		if (icon != null)
		{
			ClickListener clickListener = ClickListener.Get(icon, string.Empty);
			if (_003C_003Ef__am_0024cache0 == null)
			{
				_003C_003Ef__am_0024cache0 = _003CFill_003Em__0;
			}
			clickListener.onClick = _003C_003Ef__am_0024cache0;
			if (RoleHeadCfg.Get(info.headId) == null)
			{
				return;
			}
			View.SetItemSprite(icon, RoleHeadCfg.Get(info.headId).icon);
		}
		if (level != null)
		{
			View.SetLabelText(level, info.level);
		}
		if (playerName != null)
		{
			View.SetLabelText(playerName, info.name, false);
		}
		if (female != null && male != null)
		{
			female.SetActive(!info.sex);
			male.SetActive(info.sex);
		}
	}

	[CompilerGenerated]
	private static void _003CFill_003Em__0(GameObject go)
	{
	}
}
