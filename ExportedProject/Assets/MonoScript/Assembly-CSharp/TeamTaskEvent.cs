using System;

public class TeamTaskEvent
{
	public static Utils.VoidDelegate HideTeamTaskPanelDelegate;

	public static Action<bool> TeamBtnsActiveChangeDelegate;

	public static Utils.VoidDelegate RefreshShowBtnsDelegate;
}
