// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis
{
	public interface IClasses
	{
		/// <summary>
		/// Prefix position classes.
		/// </summary>
		List<IClass> PrefixClasses { get; }

		/// <summary>
		/// Suffix position classes.
		/// </summary>
		List<IClass> SuffixClasses { get; }
	}
}