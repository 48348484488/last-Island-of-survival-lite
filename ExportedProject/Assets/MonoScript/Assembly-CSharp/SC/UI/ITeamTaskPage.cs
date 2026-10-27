using UnityEngine;

namespace SC.UI
{
	public interface ITeamTaskPage
	{
		GameObject ThisGo { get; }

		void OnInit();

		void OnShow(object param);

		void OnHide();
	}
}
