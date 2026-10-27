using UnityEngine;

public interface IActivityPage
{
	GameObject ThisGo { get; }

	void OnInit();

	void OnShow(object param);

	void OnHide();
}
