public class NewChatInfoData<T>
{
	public RoundListSc<T> chats;

	public int newMsgNum;

	public float nextTimeSendMsg;

	public NewChatInfoData(int capacity)
	{
		chats = new RoundListSc<T>(capacity);
	}

	public void add(T bean, bool isCount)
	{
		chats.add(bean);
		if (isCount)
		{
			newMsgNum++;
		}
	}
}
