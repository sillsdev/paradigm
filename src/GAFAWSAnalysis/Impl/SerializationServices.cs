using System.Xml.Linq;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	internal static class SerializationServices
	{
		internal static XElement WriteOtherElement(string otherContent)
		{
			return string.IsNullOrEmpty(otherContent) ? null : new XElement("Other", XElement.Parse(otherContent));
		}

		internal static string ReadOtherElement(XElement otherElement)
		{
			return otherElement == null ? null : otherElement.FirstNode.ToString();
		}
	}
}