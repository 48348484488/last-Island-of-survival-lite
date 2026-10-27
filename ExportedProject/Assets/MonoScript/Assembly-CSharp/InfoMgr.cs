using System;
using gs.role.scmsg;

public class InfoMgr : Singleton<InfoMgr>
{
	private CGetRoleMoreInformation _cGetRoleMoreInformation = new CGetRoleMoreInformation();

	private CChangeName _cChangeName = new CChangeName();

	public void Init()
	{
		SGetRoleMoreInformation.handler = (SGetRoleMoreInformation.Handler)Delegate.Combine(SGetRoleMoreInformation.handler, new SGetRoleMoreInformation.Handler(SGetRoleMoreInformationHandle));
		SChangeName.handler = (SChangeName.Handler)Delegate.Combine(SChangeName.handler, new SChangeName.Handler(SChangeNameHandle));
	}

	private void SChangeNameHandle(SChangeName msg)
	{
	}

	private void SGetRoleMoreInformationHandle(SGetRoleMoreInformation msg)
	{
	}

	private void GetRoleMoreInformation(long roleId)
	{
		_cGetRoleMoreInformation.targetRoleId = roleId;
		Client2Gs.Ins.Send(_cGetRoleMoreInformation);
	}

	private void ChangeName(string roleName)
	{
		_cChangeName.name = roleName;
		Client2Gs.Ins.Send(_cChangeName);
	}
}
