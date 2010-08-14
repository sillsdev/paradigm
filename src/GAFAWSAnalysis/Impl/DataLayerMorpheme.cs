namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	public abstract class DataLayerMorpheme : IDataLayerMorpheme
	{
		/// <summary>
		/// Reference to ID of morpheme.
		/// </summary>
		public string MidRef { get; set; }

		/// <summary>
		/// Model-specific data.
		/// </summary>
		public string Other { get; set; }
	}
}