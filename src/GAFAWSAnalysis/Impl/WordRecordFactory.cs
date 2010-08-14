namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	public class WordRecordFactory : IWordRecordFactory
	{
		#region Implementation of IWordRecordFactory

		public IWordRecord Create()
		{
			return new WordRecord();
		}

		#endregion
	}
}