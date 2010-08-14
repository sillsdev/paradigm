// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2003, SIL International. All Rights Reserved.
// </copyright>
//
// File: WordLevel.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Implementation of WordRecord.
// </remarks>
using System.Collections.Generic;

namespace SIL.WordWorks.GAFAWS.PositionAnalysis.Impl
{
	/// <summary>
	/// A word record.
	/// </summary>
	internal sealed class WordRecord : IWordRecord
	{
		internal WordRecord(string id)
		{
			Id = id;
		}

		/// <summary>
		/// Word record ID.
		/// </summary>
		public string Id { get; private set; }

		/// <summary>
		/// Collection of prefixes.
		/// </summary>
		public List<IAffix> Prefixes { get; set; }

		/// <summary>
		/// The stem.
		/// </summary>
		public IStem Stem { get; set; }

		/// <summary>
		/// Collection of suffixes.
		/// </summary>
		public List<IAffix> Suffixes { get; set; }

		/// <summary>
		/// Model-specific data.
		/// </summary>
		public string Other { get; set; }
	}
}