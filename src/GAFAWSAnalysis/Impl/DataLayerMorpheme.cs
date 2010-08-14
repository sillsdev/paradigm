namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	internal abstract class DataLayerMorpheme : IDataLayerMorpheme
	{
		/// <summary>
		/// Reference to ID of morpheme.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Model-specific data.
		/// </summary>
		public string Other { get; set; }
	}
}