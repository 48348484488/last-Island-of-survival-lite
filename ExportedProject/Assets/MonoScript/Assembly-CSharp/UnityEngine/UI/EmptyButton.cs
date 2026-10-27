namespace UnityEngine.UI
{
	public class EmptyButton : Graphic
	{
		protected EmptyButton()
		{
			base.useLegacyMeshGeneration = false;
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			toFill.Clear();
		}
	}
}
