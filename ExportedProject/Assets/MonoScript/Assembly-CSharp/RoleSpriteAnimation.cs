using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RoleSpriteAnimation : MonoBehaviour
{
	public enum Direction
	{
		Foward = 0,
		Backward = 1,
		Left = 2,
		Right = 3
	}

	private RectTransform mRectTransform;

	private Image mImage;

	private int mCurFrame;

	private float mDelta;

	private Direction mOrientation;

	private List<Sprite> mSpriteFrames;

	public float FPS = 5f;

	public bool IsPlaying = true;

	public bool AutoPlay = true;

	public bool Loop = true;

	public Direction Orientation;

	public List<Sprite> FowardFrames;

	private List<Sprite> BackwardFrames;

	private List<Sprite> LeftFrames;

	private List<Sprite> RightFrames;

	public RectTransform rectTransform
	{
		get
		{
			return mRectTransform;
		}
	}

	private void Awake()
	{
		mRectTransform = base.transform as RectTransform;
		mImage = GetComponent<Image>();
	}

	private void Start()
	{
		SetDirection(Orientation, false);
		if (AutoPlay)
		{
			Play();
		}
		else
		{
			IsPlaying = false;
		}
	}

	private void SetSprite(int idx)
	{
		Sprite sprite = mSpriteFrames[idx];
		if (mImage.sprite != sprite)
		{
			mImage.sprite = mSpriteFrames[idx];
			mImage.SetNativeSize();
			Vector2 pivot = sprite.pivot;
			pivot.x /= sprite.rect.width;
			pivot.y /= sprite.rect.height;
			mRectTransform.pivot = pivot;
		}
	}

	private void Update()
	{
		SetDirection(Orientation);
		if (mSpriteFrames == null)
		{
			return;
		}
		int count = mSpriteFrames.Count;
		if (!IsPlaying || count == 0)
		{
			return;
		}
		mDelta += Time.deltaTime;
		if (!(mDelta >= 1f / FPS))
		{
			return;
		}
		mDelta = 0f;
		mCurFrame++;
		if (mCurFrame >= count)
		{
			if (!Loop)
			{
				IsPlaying = false;
				return;
			}
			mCurFrame = 0;
		}
		SetSprite(mCurFrame);
	}

	public void SetDirection(float angle)
	{
		if (angle < 30f || angle > 330f)
		{
			SetDirection(Direction.Backward);
		}
		else if (angle >= 30f && angle < 135f)
		{
			SetDirection(Direction.Right);
		}
		else if (angle >= 135f && angle <= 225f)
		{
			SetDirection(Direction.Foward);
		}
		else
		{
			SetDirection(Direction.Left);
		}
	}

	public void SetDirection(Direction dir, bool check = true)
	{
		if (!check || mOrientation != dir)
		{
			mOrientation = dir;
			Orientation = dir;
			switch (mOrientation)
			{
			case Direction.Foward:
				mSpriteFrames = FowardFrames;
				break;
			case Direction.Backward:
				mSpriteFrames = BackwardFrames;
				break;
			case Direction.Left:
				mSpriteFrames = LeftFrames;
				break;
			case Direction.Right:
				mSpriteFrames = RightFrames;
				break;
			}
			mDelta = 0f;
			mCurFrame = 0;
			if (mSpriteFrames.Count > 0)
			{
				SetSprite(mCurFrame);
			}
		}
	}

	public void Play()
	{
		IsPlaying = true;
	}

	public void Pause()
	{
		IsPlaying = false;
	}

	public void Resume()
	{
		IsPlaying = true;
	}

	public void Stop()
	{
		mCurFrame = 0;
		SetSprite(mCurFrame);
		IsPlaying = false;
	}

	public void Rewind()
	{
		mCurFrame = 0;
		SetSprite(mCurFrame);
		Play();
	}
}
