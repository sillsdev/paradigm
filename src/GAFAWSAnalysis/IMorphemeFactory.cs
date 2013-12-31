// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	/// <summary>
	/// Factory for creating IMorpheme instances.
	/// </summary>
	public interface IMorphemeFactory
	{
		IMorpheme Create(MorphemeType mType, string id);
	}
}