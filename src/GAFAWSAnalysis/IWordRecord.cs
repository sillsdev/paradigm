using System.Collections.Generic;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	public interface IWordRecord
	{
		/// <summary>
		/// Word record ID.
		/// </summary>
		string Id { get; set; }

		/// <summary>
		/// Collection of prefixes.
		/// </summary>
		List<IAffix> Prefixes { get; set; }

		/// <summary>
		/// The stem.
		/// </summary>
		IStem Stem { get; set; }

		/// <summary>
		/// Collection of suffixes.
		/// </summary>
		List<IAffix> Suffixes { get; set; }

		/// <summary>
		/// Model-specific data.
		/// </summary>
		string Other { get; set; }
	}
}