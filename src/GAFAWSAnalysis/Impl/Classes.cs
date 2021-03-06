﻿// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// Holder for classes. (Reserved for use by the Paradigm DLL.)
	/// </summary>
	internal sealed class Classes : IClasses
	{
		/// <summary>
		/// Prefix position classes.
		/// </summary>
		public List<IClass> PrefixClasses { get; private set;  }

		/// <summary>
		/// Suffix position classes.
		/// </summary>
		public List<IClass> SuffixClasses { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		internal Classes()
		{
			PrefixClasses = new List<IClass>();
			SuffixClasses = new List<IClass>();
		}
	}
}