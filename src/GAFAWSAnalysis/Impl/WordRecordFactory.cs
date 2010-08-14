namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	internal class WordRecordFactory : IWordRecordFactory
	{
		private int _idx = 1;

		#region Implementation of IWordRecordFactory

		public IWordRecord Create()
		{
			return new WordRecord("WR" + _idx++);
		}

		#endregion
	}
}