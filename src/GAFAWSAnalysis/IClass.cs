// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	public interface IClass
	{
		/// <summary>
		/// Class ID.
		/// </summary>
		string Id { get; set; }

		/// <summary>
		/// Class name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// 0 for an unknown position, otherwise 1.
		/// </summary>
		string IsFogBank { get; set; }
	}
}