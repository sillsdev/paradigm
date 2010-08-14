namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	/// <summary>
	/// Factory for creating IWordRecord instances.
	/// </summary>
	public interface IWordRecordFactory
	{
		IWordRecord Create();
	}
}