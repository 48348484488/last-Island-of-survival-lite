using System.Collections;

public class XMLNodeList : ArrayList
{
	public XMLNode Pop()
	{
		XMLNode xMLNode = null;
		if (Count <= 0)
		{
			return null;
		}
		xMLNode = (XMLNode)this[Count - 1];
		Remove(xMLNode);
		return xMLNode;
	}

	public int Push(XMLNode item)
	{
		Add(item);
		return Count;
	}
}
