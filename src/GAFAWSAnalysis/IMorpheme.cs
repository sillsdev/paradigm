// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	public interface IMorpheme
	{
		/// <summary>
		/// Model-specific data.
		/// </summary>
		string Other { get; set; }

		/// <summary>
		/// Morpheme ID.
		/// </summary>
		string Id { get; set; }

		/// <summary>
		/// Type of morpheme.
		/// [NB: Legal values are: 'pfx' for prefix, 's' for stem, and 'sfx' for suffix.]
		/// </summary>
		string Type { get; set; }

		/// <summary>
		/// Starting position class ID. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		string StartClass { get; set; }

		/// <summary>
		/// Ending position class ID. (Reserved for use by the Paradigm DLL.)
		/// </summary>
		string EndClass { get; set; }
	}
}