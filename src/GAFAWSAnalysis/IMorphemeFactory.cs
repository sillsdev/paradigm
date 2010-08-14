namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	/// <summary>
	/// Factory for creating IMorpheme instances.
	/// </summary>
	public interface IMorphemeFactory
	{
		IMorpheme Create(MorphemeType mType, string id);
	}
}