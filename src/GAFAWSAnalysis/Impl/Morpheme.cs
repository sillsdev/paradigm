// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System;
using SIL.WordWorks.GAFAWS.PositionAnalysis.Properties;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// An individual morpheme.
	/// </summary>
	internal sealed class Morpheme : IMorpheme
	{
		private string _type;

		/// <summary>
		/// Model-specific data.
		/// </summary>
		public string Other { get; set; }

		/// <summary>
		/// Morpheme ID.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Type of morpheme.
		/// [NB: Legal values are: 'pfx' for prefix, 's' for stem, and 'sfx' for suffix.]
		/// </summary>
		public string Type
		{
			get { return _type; }
			set
			{
				if (value == "s" || value == "pfx" || value == "sfx")
					_type = value;
				else
					throw new ArgumentException(PublicResources.kInvalidType, "value");
			}
		}

		/// <summary>
		/// Starting position class ID. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		public string StartClass { get; set; }

		/// <summary>
		/// Ending position class ID. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		public string EndClass { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="mType">Morpheme type.</param>
		/// <param name="id">Morpheme id.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when the type is not valid.
		/// </exception>
		internal Morpheme(MorphemeType mType, string id)
		{
			Id = id;
			switch (mType)
			{
				case MorphemeType.Prefix:
					Type = "pfx";
					break;
				case MorphemeType.Suffix:
					Type = "sfx";
					break;
				case MorphemeType.Stem:
					Type = "s";
					break;
				default:
					throw new ArgumentException(PublicResources.kInvalidType, "mType");
			}
		}

		internal Morpheme()
		{}

		public override string ToString()
		{
			return Id;
		}
	}
}