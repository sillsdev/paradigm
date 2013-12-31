// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.ComponentModel.Composition;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	[Export(typeof(IWordRecordFactory))]
	internal class WordRecordFactory : IWordRecordFactory
	{
		private int _idx = 1;

		#region Implementation of IWordRecordFactory

		public IWordRecord Create()
		{
			return new WordRecord("WR" + _idx++);
		}

		#endregion
	}
}