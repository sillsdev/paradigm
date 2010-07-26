using System.Collections.Generic;
using System.Xml.Serialization;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	public interface IGafawsData
	{
		/// <summary>
		/// Collection of word records.
		/// </summary>
		[XmlArrayItem("WordRecord", IsNullable = false)]
		List<WordRecord> WordRecords { get; set; }

		/// <summary>
		/// Collection of morphemes.
		/// </summary>
		[XmlArrayItemAttribute("Morpheme", IsNullable = false)]
		List<Morpheme> Morphemes { get; set; }

		/// <summary>
		/// Collection of position classes. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		Classes Classes { get; set; }

		/// <summary>
		/// Collection of problems. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		[XmlArrayItemAttribute("Challenge", IsNullable = false)]
		List<Challenge> Challenges { get; set; }

		/// <summary>
		/// Model-specific data.
		/// </summary>
		[XmlIgnore]
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