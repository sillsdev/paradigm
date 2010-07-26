// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: WordLevel.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Implementation of WordRecord.
// </remarks>
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	/// <summary>
	/// A word record.
	/// </summary>
	public sealed class WordRecord
	{
		/// <summary>
		/// Word record ID.
		/// </summary>
		[XmlAttributeAttribute(DataType = "ID")]
		public string WRID;

		/// <summary>
		/// Collection of prefixes.
		/// </summary>
		[XmlArrayItemAttribute(IsNullable=false)]
		public List<Affix> Prefixes;

		/// <summary>
		/// The stem.
		/// </summary>
		public Stem Stem;

		/// <summary>
		/// Collection of suffixes.
		/// </summary>
		[XmlArrayItemAttribute(IsNullable=false)]
		public List<Affix> Suffixes;

		/// <summary>
		/// Model-specific data.
		/// </summary>
		[XmlIgnore]
		public string Other
		{
			get
			{
				return SerializedXmlString == null ? null : SerializedXmlString.Value;
			}
			set
			{
				if (SerializedXmlString == null)
					SerializedXmlString = new RawString();
				SerializedXmlString.Value = value;
			}
		}

		[XmlElement("Other")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public RawString SerializedXmlString;
	}
}