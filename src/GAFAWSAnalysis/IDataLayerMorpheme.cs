namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	public interface IDataLayerMorpheme
	{
		/// <summary>
		/// Reference to ID of morpheme.
		/// </summary>
		string Id { get; set; }

		/// <summary>
		/// Model-specific data.
		/// </summary>
		string Other { get; set; }
	}
}