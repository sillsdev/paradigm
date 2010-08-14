using System.Collections.Generic;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	public interface IClasses
	{
		/// <summary>
		/// Prefix position classes.
		/// </summary>
		List<IClass> PrefixClasses { get; }

		/// <summary>
		/// Suffix position classes.
		/// </summary>
		List<IClass> SuffixClasses { get; }
	}
}