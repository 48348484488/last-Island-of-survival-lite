public class MsgBoxParam
{
	public delegate void Cancel();

	public delegate void Confirm();

	public string strMessage;

	public Cancel cancel;

	public Confirm confirm;
}
