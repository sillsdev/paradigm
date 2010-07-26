namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	/// <summary>
	/// Interface for the GAFAWS position analysis.
	/// </summary>
	public interface IPositionAnalyzer
	{
		/// <summary>
		/// Process the given input GAFAWS data object.
		/// </summary>
		/// <param name="gData">GAFAWSData to process.</param>
		void Process(IGafawsData gData);
	}
}