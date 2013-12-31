// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	public interface IDataLayerMorpheme
	{
		/// <summary>
		/// Reference to ID of morpheme.
		/// </summary>
		string Id { get; set; }

		/// <summary>
		/// Model-specific data.
		/// </summary>
		string Other { get; set; }
	}
}