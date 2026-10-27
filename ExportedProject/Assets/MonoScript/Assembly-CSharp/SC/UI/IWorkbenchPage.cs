using UnityEngine;
using gs.bag.scmsg;

namespace SC.UI
{
	public interface IWorkbenchPage
	{
		GameObject ThisGo { get; }

		void OnInit();

		void OnShow(int level);

		void OnHide();

		void OnClickBagItem(BagItem bagItemInfo, int num);
	}
}
