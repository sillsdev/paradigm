using System.ComponentModel.Composition;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	[Export(typeof(IAffixFactory))]
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