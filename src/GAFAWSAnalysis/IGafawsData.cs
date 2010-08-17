using System.Collections.Generic;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	public interface IGafawsData
	{
		/// <summary>
		/// Collection of word records.
		/// </summary>
		List<IWordRecord> WordRecords { get; }

		/// <summary>
		/// Collection of morphemes.
		/// </summary>
		List<IMorpheme> Morphemes { get; }

		/// <summary>
		/// Get the sets of affixes that do cooccur within given data set.
		/// </summary>
		List<HashSet<IMorpheme>> AffixCooccurrences { get; }

		/// <summary>
		/// Get the sets of affixes that do *not* cooccur within given data set.
		/// </summary>
		List<HashSet<IMorpheme>> AffixNonCooccurrences { get; }

		/// <summary>
		/// Get the distinct sets of affixes that do not cooccur within given set.
		/// </summary>
		List<HashSet<IMorpheme>> DistinctSets { get; }

		/// <summary>
		/// Collection of position classes. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		IClasses Classes { get; }

		/// <summary>
		/// Collection of problems. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		List<IChallenge> Challenges { get; }

		/// <summary>
		/// Model-specific data.
		/// </summary>
		string Other { get; set; }

		/// <summary>
		/// Save the data to a file.
		/// </summary>
		/// <param name="pathname">Pathname of file to save to.</param>
		void SaveData(string pathname);

		/// <summary>
		/// Reset the object for more use.
		/// </summary>
		void Reset();
	}
}