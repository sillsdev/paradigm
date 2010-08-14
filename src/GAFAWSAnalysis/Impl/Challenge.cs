namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// A problem report. (Reserved for use by the Paradigm DLL.)
	/// </summary>
	internal sealed class Challenge : IChallenge
	{
		/// <summary>
		/// Report message.
		/// </summary>
		public string Message { get; set; }
	}
}