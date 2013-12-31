// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
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
		/// A set of affixes (prefixes and suffixes) for the word record, if any.
		/// </summary>
		public HashSet<IAffix> AllAffixes
		{
			get
			{
				var result = new HashSet<IAffix>();
				if (Prefixes != null && Prefixes.Count > 0)
					result.UnionWith(Prefixes);
				if (Suffixes != null && Suffixes.Count > 0)
					result.UnionWith(Suffixes);
				return result;
			}
		}

		/// <summary>
		/// Model-specific data.
		/// </summary>
		public string Other { get; set; }
	}
}