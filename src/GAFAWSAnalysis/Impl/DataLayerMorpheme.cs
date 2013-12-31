// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	internal abstract class DataLayerMorpheme : IDataLayerMorpheme
	{
		/// <summary>
		/// Reference to ID of morpheme.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Model-specific data.
		/// </summary>
		public string Other { get; set; }
	}
}