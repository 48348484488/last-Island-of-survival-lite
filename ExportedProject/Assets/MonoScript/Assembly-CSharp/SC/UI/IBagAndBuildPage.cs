namespace SC.UI
{
	public interface IBagAndBuildPage
	{
		bool IsShow { get; }

		void OnInit();

		void OnShow(object param = null);

		void OnHide();
	}
}
