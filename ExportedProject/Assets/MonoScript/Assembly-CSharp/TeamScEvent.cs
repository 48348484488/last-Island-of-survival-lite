using System;

public class TeamScEvent
{
	public static Utils.VoidDelegate UpdateLeaveOrAddTroopEvent;

	public static Action<long, bool> UpdateOnlineStatusEvent;

	public static Utils.VoidDelegate ChangeLeaderEvent;

	public static Utils.VoidDelegate ShowInvitePanelEvent;

	public static Utils.VoidDelegate SendInviteMsgSuccessEvent;

	public static Utils.VoidDelegate LeaveTeamEvent;

	public static Action<long> TeammateCutDownAction;

	public static Action TeammateChangeNameAction;
}
