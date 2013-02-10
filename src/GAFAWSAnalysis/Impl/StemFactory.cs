using System.ComponentModel.Composition;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	[Export(typeof(IStemFactory))]
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