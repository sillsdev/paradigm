namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// Affix position class. (Reserved for use by the Paradigm DLL.)
	/// </summary>
	internal sealed class Class : IClass
	{
		internal Class()
		{
			IsFogBank = "0";
		}

		/// <summary>
		/// Class ID.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Class name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 0 for an unknown position, otherwise 1.
		/// </summary>
		public string IsFogBank { get; set; }
	}
}