namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	internal class MorphemeFactory : IMorphemeFactory
	{
		#region Implementation of IMorphemeFactory

		public IMorpheme Create(MorphemeType mType, string id)
		{
			return new Morpheme(mType, id);
		}

		#endregion
	}
}