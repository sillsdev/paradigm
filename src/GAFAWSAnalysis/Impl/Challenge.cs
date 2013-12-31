﻿// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// A problem report. (Reserved for use by the Paradigm DLL.)
	/// </summary>
	internal sealed class Challenge : IChallenge
	{
		/// <summary>
		/// Report message.
		/// </summary>
		public string Message { get; set; }
	}
}