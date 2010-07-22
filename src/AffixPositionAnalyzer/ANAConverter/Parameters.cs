// --------------------------------------------------------------------------------------------
// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2007, SIL International. All Rights Reserved.
// </copyright>
//
// File: Parameters.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Parameters Class for serialization.
// </remarks>
//
// --------------------------------------------------------------------------------------------
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace SIL.WordWorks.GAFAWS.AffixPositionAnalyzer.ANAConverter
{
	/// <summary>
	/// Parameters for the converter.
	/// </summary>
	[XmlRootAttribute("ANAConverterOptions", Namespace="", IsNullable=false)]
	public class Parameters
	{
		private static XmlSerializer s_serializer = new XmlSerializer(typeof(Parameters));

		/// <summary>
		/// Constructor.
		/// </summary>
		public Parameters()
		{
			RootDelimiter = new RootDelimiters();
			Marker = new Markers();
			Categories = new List<Category>();
		}

		/// <summary>
		/// RootDelimiters
		/// </summary>
		[XmlElementAttribute("RootDelimiter", typeof(RootDelimiters))]
		public RootDelimiters RootDelimiter { get; set; }

		/// <summary>
		/// Markers
		/// </summary>
		[XmlElementAttribute("Marker", typeof(Markers))]
		public Markers Marker { get; set; }

		/// <summary>
		/// Categories
		/// </summary>
		[XmlElementAttribute("Category", typeof(Category))]
		public List<Category> Categories { get; set; }

		/// <summary>
		/// Serialize.
		/// </summary>
		public void Serialize(string filename)
		{
			using (var writer = new StreamWriter(filename))
			{
				s_serializer.Serialize(writer, this);
			}
		}

		/// <summary>
		/// DeSerialize.
		/// </summary>
		/// <param name="parms"></param>
		/// <returns></returns>
		public static Parameters DeSerialize(string parms)
		{
			using (var parameterReader = new FileStream(parms, FileMode.Open))
			{
				if (parameterReader.Length == 0)
					throw new FileLoadException();
				return (Parameters)s_serializer.Deserialize(parameterReader);
			}
		}
}


	/// <summary>
	/// RootDelimeter class, open and close.
	/// </summary>
	public class RootDelimiters
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public RootDelimiters()
		{
			OpenDelimiter = '<';
			CloseDelimiter = '>';
		}

		/// <summary>
		/// OpenDelimiter attribute
		/// </summary>
		[XmlAttributeAttribute("openDelimiter")]
		public char OpenDelimiter { get; set; }

		/// <summary>
		/// CloseDelimiter attribute.
		/// </summary>
		[XmlAttributeAttribute("closeDelimiter")]
		public char CloseDelimiter { get; set; }
	}

	/// <summary>
	/// Marker class, ambiguity and decomposition.
	/// </summary>
	public class Markers
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public Markers()
		{
			Ambiguity = '%';
			Decomposition = '-';
		}

		/// <summary>
		/// Ambiguity attribute.
		/// </summary>
		[XmlAttributeAttribute("ambiguity")]
		public char Ambiguity { get; set; }

		/// <summary>
		/// Decomposition attribute.
		/// </summary>
		[XmlAttributeAttribute("decomposition")]
		public char Decomposition { get; set; }
	}

	/// <summary>
	/// Category
	/// </summary>
	public class Category
	{
		/// <summary>
		/// Empty Constructor.
		/// </summary>
		public Category()
		{
		}

		/// <summary>
		/// Empty Constructor.
		/// </summary>
		public Category(string c)
		{
			Cat = c;
		}

		/// <summary>
		/// Cat attribute.
		/// </summary>
		[XmlAttributeAttribute("name")]
		public string Cat { get; set; }
	}
}
