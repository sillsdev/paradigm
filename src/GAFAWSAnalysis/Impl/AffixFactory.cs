namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	public class AffixFactory : IAffixFactory
	{
		#region Implementation of IAffixFactory

		public IAffix Create()
		{
			return new Affix();
		}

		#endregion
	}
}