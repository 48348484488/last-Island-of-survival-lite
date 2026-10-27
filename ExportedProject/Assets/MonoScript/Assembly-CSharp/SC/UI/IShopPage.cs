using UnityEngine;

namespace SC.UI
{
	public interface IShopPage
	{
		GameObject ThisGo { get; }

		void OnInit();

		void OnShow(object param);

		void OnHide();
	}
}
