namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	internal class StemFactory : IStemFactory
	{
		#region Implementation of IStemFactory

		public IStem Create()
		{
			return new Stem();
		}

		#endregion
	}
}