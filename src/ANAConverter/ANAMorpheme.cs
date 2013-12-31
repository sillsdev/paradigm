// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.ANAConverter
{
	/// <summary>
	/// Abstract class of morphemes.
	/// </summary>
	internal abstract class AnaMorpheme : AnaObject
	{
		protected string MorphnameAsString;
		protected string Decomposition;
		protected string UnderlyingForm;
		protected string Category;
		protected IDataLayerMorpheme DataLayerMorpheme;
		protected IMorpheme Morpheme;
		protected MorphemeType Type;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="morphname">The morphname of the morpheme.</param>
		internal AnaMorpheme(string morphname)
		{
			MorphnameAsString = morphname;
		}

		protected IWordRecord LastRecord
		{
			get
			{
				return Gd.WordRecords[Gd.WordRecords.Count - 1];
			}
		}

		/// <summary>
		/// Adds content from various fields in the ANA record.
		/// </summary>
		/// <param name="type">The type of field being processed.</param>
		/// <param name="form">The individual form being added.</param>
		internal virtual void AddContent(LineType type, string form)
		{
			switch (type)
			{
				case LineType.UnderlyingForm:
				{
					UnderlyingForm = form;
					break;
				}
				case LineType.Category:
				{
					Category = form;
					break;
				}
				case LineType.Decomposition:
				{
					Decomposition = form;
					break;
				}
			}
		}

		/// <summary>
		/// An abstract method used to convert a morpheme.
		/// Subclasses must override this method to do an appropriate conversion.
		/// </summary>
		internal virtual void Convert(IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory)
		{
			foreach (var m in Gd.Morphemes.Where(m => m.Id == MorphnameAsString))
			{
				Morpheme = m;
				break;
			}
			if (Morpheme == null)
			{
				Morpheme = morphemeFactory.Create(Type, MorphnameAsString);
				Gd.Morphemes.Add(Morpheme);
			}
			DataLayerMorpheme.Id = Morpheme.Id;
			if (UnderlyingForm != null)
			{
				Morpheme.Other = "<ANAInfo underlyingForm=\'" + UnderlyingForm + "\' />";
			}
			if (Category == null && Decomposition == null) return;

			var xml = "<ANAInfo";
			if (Category != null)
				xml += " category=\'" + Category + "\'";
			if (Decomposition != null)
				xml += " decomposition=\'" + Decomposition + "\'";
			xml += " />";
			DataLayerMorpheme.Other = xml;
		}
	}

	/// <summary>
	/// A stem morpheme.
	/// </summary>
	internal class AnaStem : AnaMorpheme
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="morphname">The morphname of the stem.</param>
		internal AnaStem(string morphname)
			: base(morphname.Trim().Replace(" ", "_"))
		{
			Type = MorphemeType.Stem;
		}

		/// <summary>
		/// Convert the stem to the data layer.
		/// </summary>
		internal override void Convert(IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory)
		{
			LastRecord.Stem = stemFactory.Create();
			DataLayerMorpheme = LastRecord.Stem;
			base.Convert(morphemeFactory, stemFactory, affixFactory);
		}

		/// <summary>
		/// Get the count of roots in the stem.
		/// </summary>
		internal int RootCount
		{
			get { return  MorphnameAsString.Split('_').Length/2; }
		}

		/// <summary>
		/// Adds content from various fields in the ANA record.
		/// </summary>
		/// <param name="type">The type of field being processed.</param>
		/// <param name="form">The individual form being added.</param>
		internal override void AddContent(LineType type, string form)
		{
			switch (type)
			{
				case LineType.UnderlyingForm:
				{
					if (UnderlyingForm == null)
						UnderlyingForm = form;
					else
						UnderlyingForm += "_" + form;
					break;
				}
				case LineType.Category:
				{
					if (Category == null)
						Category = form;
					else
						Category += "_" + form;
					break;
				}
				case LineType.Decomposition:
				{
					if (Decomposition == null)
						Decomposition = form;
					else
						Decomposition += "_" + form;
					break;
				}
			}
		}
	}

	/// <summary>
	/// A prefix or suffix morpheme.
	/// </summary>
	internal abstract class AnaAffix : AnaMorpheme
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="morphname">The morphname of the affix.</param>
		internal AnaAffix(string morphname)
			: base(morphname)
		{
		}
	}

	internal class AnaPrefix : AnaAffix
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="morphname">The morphname of the affix.</param>
		internal AnaPrefix(string morphname)
			: base(morphname)
		{
			Type = MorphemeType.Prefix;
		}

		/// <summary>
		/// Convert the affix to the data layer.
		/// </summary>
		internal override void Convert(IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory)
		{
			var afx = affixFactory.Create();
			DataLayerMorpheme = afx;
			LastRecord.Prefixes.Add(afx);
			base.Convert(morphemeFactory, stemFactory, affixFactory);
		}

		/// <summary>
		/// Tokenize the given input, and create affixes.
		/// </summary>
		/// <param name="affixes">One or more affixes to process.</param>
		/// <returns>An array of affixes that have been created.</returns>
		internal static List<AnaPrefix> TokenizeAffixes(string affixes)
		{
			List<AnaPrefix> list = null;
			if (affixes.Length > 0)
			{
				var afxes = affixes.Trim().Split();
				list = afxes.Select(t => new AnaPrefix(t)).ToList();
			}
			return list;
		}
	}

	internal class AnaSuffix : AnaAffix
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="morphname">The morphname of the affix.</param>
		internal AnaSuffix(string morphname)
			: base(morphname)
		{
			Type = MorphemeType.Suffix;
		}

		/// <summary>
		/// Convert the affix to the data layer.
		/// </summary>
		internal override void Convert(IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory)
		{
			var afx = affixFactory.Create();
			LastRecord.Suffixes.Add(afx);
			DataLayerMorpheme = afx;
			base.Convert(morphemeFactory, stemFactory, affixFactory);
		}

		/// <summary>
		/// Tokenize the given input, and create affixes.
		/// </summary>
		/// <param name="affixes">One or more affixes to process.</param>
		/// <returns>An array of affixes that have been created.</returns>
		internal static List<AnaSuffix> TokenizeAffixes(string affixes)
		{
			List<AnaSuffix> list = null;
			if (affixes.Length > 0)
			{
				var afxes = affixes.Trim().Split();
				list = afxes.Select(t => new AnaSuffix(t)).ToList();
			}
			return list;
		}
	}
}
