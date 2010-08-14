namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	internal class AffixFactory : IAffixFactory
	{
		#region Implementation of IAffixFactory

		public IAffix Create()
		{
			return new Affix();
		}

		#endregion
	}
}