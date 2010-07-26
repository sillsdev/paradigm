// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: MorphemeLevel.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Implementation of various sub-word level classes.
// </remarks>
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	/// <summary>
	/// Morpheme type enumeration.
	/// </summary>
	public enum MorphemeType
	{
		Stem,
		Prefix,
		Suffix
	};

	public class DataLayerMorpheme
	{
		/// <summary>
		/// Reference to ID of morpheme.
		/// </summary>
		[XmlAttributeAttribute(DataType="IDREF")]
		public string MIDREF;

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

	/// <summary>
	/// An Affix in a word record.
	/// </summary>
	public sealed class Affix : DataLayerMorpheme
	{
	}

	/// <summary>
	/// The stem in a word record.
	/// </summary>
	public sealed class Stem : DataLayerMorpheme
	{
	}

	/// <summary>
	/// An individual morpheme.
	/// </summary>
	public sealed class Morpheme
	{
		/// <summary>
		/// Stores the type, since it has get/set.
		/// </summary>
		private string m_type;

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

		/// <summary>
		/// Morpheme ID.
		/// </summary>
		[XmlAttributeAttribute(DataType="ID")]
		public string MID;

		/// <summary>
		/// Type of morpheme.
		/// [NB: Legal values are: 'pfx' for prefix, 's' for stem, and 'sfx' for suffix.]
		/// </summary>
		[XmlAttributeAttribute]
		public string type
		{
			get { return m_type; }
			set
			{
				if (value == "s" || value == "pfx" || value == "sfx")
					m_type = value;
				else
					throw new ArgumentException("Invalid type.", "value");
			}
		}

		/// <summary>
		/// Starting position class ID. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		[XmlAttributeAttribute(DataType="IDREF")]
		public string StartCLIDREF;

		/// <summary>
		/// Ending position class ID. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		[XmlAttributeAttribute(DataType="IDREF")]
		public string EndCLIDREF;

		/// <summary>
		/// [DO NOT USE THIS CONSTRUCTOR. Only used by serialization.]
		/// </summary>
		public Morpheme()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="mType">Morpheme type.</param>
		/// <param name="id">Morpheme id.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when the type is not valid.
		/// </exception>
		public Morpheme(MorphemeType mType, string id)
		{
			MID = id;
			switch (mType)
			{
				case MorphemeType.Prefix:
					type = "pfx";
					break;
				case MorphemeType.Suffix:
					type = "sfx";
					break;
				case MorphemeType.Stem:
					type = "s";
					break;
				default:
					throw new ArgumentException("Invalid type.", "mType");
			}
		}
	}
}