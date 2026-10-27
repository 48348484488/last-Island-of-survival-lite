public struct IntVector2
{
	public int x;

	public int y;

	public IntVector2(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public static bool operator ==(IntVector2 lhs, IntVector2 rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y;
	}

	public static bool operator !=(IntVector2 lhs, IntVector2 rhs)
	{
		return lhs.x != rhs.x || lhs.y != rhs.y;
	}

	public void Set(int newX, int newY)
	{
		x = newX;
		y = newY;
	}

	public override bool Equals(object other)
	{
		if (!(other is IntVector2))
		{
			return false;
		}
		IntVector2 intVector = (IntVector2)other;
		return x == intVector.x && y == intVector.y;
	}

	public override int GetHashCode()
	{
		return x ^ (y << 2);
	}

	public override string ToString()
	{
		return string.Format("({0}, {1})", x, y);
	}
}
