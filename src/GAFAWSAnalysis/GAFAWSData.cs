// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: GAFAWSData.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Implementation of the main GAFAWSData class, and some of its supporting classes.
// </remarks>
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	/// <summary>
	/// Main class in the GAFAWS data layer.
	/// </summary>
	[XmlRootAttribute(Namespace = "", IsNullable = false, DataType = "GafawsData", ElementName = "GAFAWSData")]
	public sealed class GafawsData : IGafawsData
	{
		private static XmlSerializer s_serializer = new XmlSerializer(typeof(GafawsData));

		#region Data members

		/// <summary>
		/// Collection of WordRecord objects.
		/// </summary>
		private List<WordRecord> m_wordRecords;

		/// <summary>
		/// Collection of Morpheme objects.
		/// </summary>
		private List<Morpheme> m_morphemes;

		/// <summary>
		/// Holder for the classes objects.
		/// </summary>
		private Classes m_classes;

		/// <summary>
		/// Collection of problems.
		/// </summary>
		private List<Challenge> m_challenges;

		#endregion // Data members

		#region Construction

		public GafawsData()
		{
			m_wordRecords = new List<WordRecord>();
			m_morphemes = new List<Morpheme>();
			m_classes = new Classes();
			m_challenges = new List<Challenge>();
		}

		#endregion // Construction

		#region Serialized Data
		// [NB: Don't reorder these, or they won't be dumped in the right order,
		// and the resulting XML file won't pass the schema.]
		/// <summary>
		/// Collection of word records.
		/// </summary>
		[XmlArrayItemAttribute("WordRecord", IsNullable=false)]
		public List<WordRecord> WordRecords
		{
			get { return m_wordRecords; }
			set { m_wordRecords = value; }
		}

		/// <summary>
		/// Collection of morphemes.
		/// </summary>
		[XmlArrayItemAttribute("Morpheme", IsNullable=false)]
		public List<Morpheme> Morphemes
		{
			get { return m_morphemes; }
			set { m_morphemes = value; }
		}

		/// <summary>
		/// Collection of position classes. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		public Classes Classes
		{
			get { return m_classes; }
			set { m_classes = value; }
		}

		/// <summary>
		/// Collection of problems. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		[XmlArrayItemAttribute("Challenge", IsNullable=false)]
		public List<Challenge> Challenges
		{
			get { return m_challenges; }
			set { m_challenges = value; }
		}

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
		/// An attribute for a date. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		[XmlAttributeAttribute]
		public string date;

		/// <summary>
		/// An attribute for a time. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		[XmlAttributeAttribute]
		public string time;

		#endregion	// Serialized Data

		#region Serialization

		/// <summary>
		/// Save the data to a file.
		/// </summary>
		/// <param name="pathname">Pathname of file to save to.</param>
		public void SaveData(string pathname)
		{
			using(TextWriter writer = new StreamWriter(pathname))
			{
				s_serializer.Serialize(writer, this);
			}
		}

		public void Reset()
		{
			m_wordRecords.Clear();
			m_morphemes.Clear();
			m_classes = new Classes();
			m_challenges.Clear();
		}

		/// <summary>
		/// Load data from file.
		/// </summary>
		/// <param name="pathname">Pathname of file to load.</param>
		/// <returns>An instance of GAFAWSData, if successful.</returns>
		/// <remarks>
		/// [NB: This may throw some exceptions, if pathname is bad,
		/// or it is not a suitable file.]
		/// </remarks>
		internal static IGafawsData LoadData(string pathname)
		{
			GafawsData gd;
			using (var reader = XmlReader.Create(pathname))
			{
				gd = (GafawsData)s_serializer.Deserialize(reader);
			}
			// Remove empty collections from WordRecord objects,
			// since the loader makes them, but the XML schema has them as optional.
			foreach (var wr in gd.m_wordRecords)
			{
				if (wr.Prefixes.Count == 0)
					wr.Prefixes = null;
				if (wr.Suffixes.Count == 0)
					wr.Suffixes = null;
			}

			return gd;
		}
		#endregion	// Serialization
	}

	/// <summary>
	/// A problem report. (Reserved for use by the Paradigm DLL.)
	/// </summary>
	public sealed class Challenge
	{
		/// <summary>
		/// Report message.
		/// </summary>
		[XmlAttributeAttribute]
		public string message;
	}

	/// <summary>
	/// Affix position class. (Reserved for use by the Paradigm DLL.)
	/// </summary>
	public sealed class Class
	{
		/// <summary>
		/// Class ID.
		/// </summary>
		[XmlAttributeAttribute(DataType="ID")]
		public string CLID;

		/// <summary>
		/// Class name.
		/// </summary>
		[XmlAttributeAttribute]
		public string name;

		/// <summary>
		/// 0 for an unknown position, otherwise 1.
		/// </summary>
		[XmlAttributeAttribute]
		[System.ComponentModel.DefaultValueAttribute("0")]
		public string isFogBank = "0";
	}

	/// <summary>
	/// Holder for classes. (Reserved for use by the Paradigm DLL.)
	/// </summary>
	public sealed class Classes
	{
		/// <summary>
		/// Prefix position classes.
		/// </summary>
		[XmlArrayItemAttribute(IsNullable=false)]
		public List<Class> PrefixClasses;

		/// <summary>
		/// Suffix position classes.
		/// </summary>
		[XmlArrayItemAttribute(IsNullable=false)]
		public List<Class> SuffixClasses;

		/// <summary>
		/// Constructor.
		/// </summary>
		public Classes()
		{
			PrefixClasses = new List<Class>();
			SuffixClasses = new List<Class>();
		}
	}

	/// <summary>
	/// Helper class that handles (De)Serialization of a property that is xml.
	/// </summary>
	public class RawString : IXmlSerializable
	{
		public string Value { get; set; }

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			Value = reader.ReadInnerXml();
		}

		public void WriteXml(XmlWriter writer)
		{
			writer.WriteRaw(Value);
		}
	}
}
