namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	/// <summary>
	/// Interface for the GAFAWS Position and Cooccurrences analysis.
	/// </summary>
	public interface IGafawsAnalyzer
	{
		/// <summary>
		/// Analyze the given input GAFAWS data object.
		/// </summary>
		/// <param name="gafawsData">GAFAWSData to analyze.</param>
		void Analyze(IGafawsData gafawsData);
	}
}