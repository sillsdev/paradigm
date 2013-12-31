// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.ComponentModel.Composition;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	[Export(typeof(IStemFactory))]
	internal class StemFactory : IStemFactory
	{
		#region Implementation of IStemFactory

		public IStem Create()
		{
			return new Stem();
		}

		#endregion
	}
}