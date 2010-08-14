namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	public interface IClass
	{
		/// <summary>
		/// Class ID.
		/// </summary>
		string Id { get; set; }

		/// <summary>
		/// Class name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// 0 for an unknown position, otherwise 1.
		/// </summary>
		string IsFogBank { get; set; }
	}
}