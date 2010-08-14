using System.Xml.Serialization;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	public interface IDataLayerMorpheme
	{
		/// <summary>
		/// Reference to ID of morpheme.
		/// </summary>
		[XmlAttribute(DataType = "IDREF")]
		string MidRef { get; set; }

		/// <summary>
		/// Model-specific data.
		/// </summary>
		[XmlIgnore]
		string Other { get; set; }
	}
}