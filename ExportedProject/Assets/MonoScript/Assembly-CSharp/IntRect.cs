public struct IntRect
{
	public int Left;

	public int Right;

	public int Bottom;

	public int Top;

	public int Width
	{
		get
		{
			return Right - Left + 1;
		}
	}

	public int Height
	{
		get
		{
			return Top - Bottom + 1;
		}
	}

	public IntRect(int left, int right, int bottom, int top)
	{
		Left = left;
		Right = right;
		Bottom = bottom;
		Top = top;
	}

	public void Set(int left, int right, int bottom, int top)
	{
		Left = left;
		Right = right;
		Bottom = bottom;
		Top = top;
	}

	public bool Contains(IntVector2 point)
	{
		return point.x >= Left && point.x <= Right && point.y >= Bottom && point.y <= Top;
	}

	public bool Contains(int x, int y)
	{
		return x >= Left && x <= Right && y >= Bottom && y <= Top;
	}

	public void EnlargeToInclude(int row, int col)
	{
		if (Left > col)
		{
			Left = col;
		}
		else if (Right < col)
		{
			Right = col;
		}
		if (Bottom > row)
		{
			Bottom = row;
		}
		else if (Top < row)
		{
			Top = row;
		}
	}

	public static bool operator ==(IntRect lhs, IntRect rhs)
	{
		return lhs.Left == rhs.Left && lhs.Right == rhs.Right && lhs.Bottom == rhs.Bottom && lhs.Top == rhs.Top;
	}

	public static bool operator !=(IntRect lhs, IntRect rhs)
	{
		return lhs.Left != rhs.Left || lhs.Right != rhs.Right || lhs.Bottom != rhs.Bottom || lhs.Top != rhs.Top;
	}

	public override int GetHashCode()
	{
		return Left ^ (Right << 2) ^ (Bottom >> 2) ^ (Top >> 1);
	}

	public override bool Equals(object other)
	{
		if (!(other is IntRect))
		{
			return false;
		}
		IntRect intRect = (IntRect)other;
		return Left == intRect.Left && Right == intRect.Right && Bottom == intRect.Bottom && Top == intRect.Top;
	}

	public override string ToString()
	{
		return string.Format("(Left:{0}, Right:{1}, Bottom:{2}, Top:{3})", Left, Right, Bottom, Top);
	}
}
