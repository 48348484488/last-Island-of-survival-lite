using System;
using Share;

namespace Net
{
	public abstract class Message : Marshal
	{
		protected Session session;

		public void sendResponse(Message msg)
		{
			if (session == null)
			{
				throw new Exception("not associate with a session");
			}
			session.Send(msg);
		}

		public void setSession(Session session)
		{
			this.session = session;
		}

		public Session getSession()
		{
			return session;
		}

		public void dispatch()
		{
			session.Manager.dispatchMessage(session, this);
		}

		public abstract int getType();

		public abstract void handle();

		public abstract Octets marshal(Octets oc);

		public abstract Octets unmarshal(Octets oc);
	}
}
