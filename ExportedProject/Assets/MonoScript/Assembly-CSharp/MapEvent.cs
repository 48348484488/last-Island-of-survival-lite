using UnityEngine;

public class MapEvent : MonoBehaviour
{
	public static Utils.VoidDelegate OnBigPlaneStart;

	public static Utils.VoidDelegate OnBigPlaneEnd;

	public static Utils.VoidDelegate OnBigPlaneOtherStart;

	public static Utils.VoidDelegate OnBigPlaneOtherEnd;

	public static Utils.VoidDelegate OnShowLastCarPos;

	public static Utils.VoidDelegate OnHideLastCarPos;

	public static Utils.BoolDelegate ToolBoxDelegate;

	public static Utils.BoolDelegate SecondBattlePosDelegate;

	public static Utils.BoolDelegate TeamPosDelegate;

	public static Utils.BoolDelegate AirDropPosDelegate;

	public static Utils.BoolDelegate DiePosDelegate;

	public static Utils.BoolDelegate ToolBoxLittleMapDelegate;

	public static Utils.BoolDelegate SecondBattlePosLittleMapDelegate;

	public static Utils.BoolDelegate TeamPosLittleMapDelegate;

	public static Utils.BoolDelegate AirDropPosLittleMapDelegate;

	public static Utils.BoolDelegate DiePosLittleMapDelegate;

	public static Utils.VoidDelegate OnSTeamInfoDelegate;

	public static Utils.VoidDelegate OnDropedItemsDuringOfflineDelegate;
}
