// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2007, SIL International. All Rights Reserved.
// </copyright>
//
// File: ANAMorpheme.cs
// Responsibility: Randy Regnier
// Last reviewed:
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
		protected string m_morphname;
		protected string m_decomposition;
		protected string m_underlyingForm;
		protected string m_category;
		protected IDataLayerMorpheme m_dataLayerMorpheme;
		protected IMorpheme m_morpheme;
		protected MorphemeType m_type;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="morphname">The morphname of the morpheme.</param>
		internal AnaMorpheme(string morphname)
		{
			m_morphname = morphname;
		}

		protected IWordRecord LastRecord
		{
			get
			{
				return s_gd.WordRecords[s_gd.WordRecords.Count - 1];
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
					m_underlyingForm = form;
					break;
				}
				case LineType.Category:
				{
					m_category = form;
					break;
				}
				case LineType.Decomposition:
				{
					m_decomposition = form;
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
			foreach (var m in s_gd.Morphemes.Where(m => m.Id == m_morphname))
			{
				m_morpheme = m;
				break;
			}
			if (m_morpheme == null)
			{
				m_morpheme = morphemeFactory.Create(m_type, m_morphname);
				s_gd.Morphemes.Add(m_morpheme);
			}
			m_dataLayerMorpheme.MidRef = m_morpheme.Id;
			if (m_underlyingForm != null)
			{
				m_morpheme.Other = "<ANAInfo underlyingForm=\'" + m_underlyingForm + "\' />";
			}
			if (m_category == null && m_decomposition == null) return;

			var xml = "<ANAInfo";
			if (m_category != null)
				xml += " category=\'" + m_category + "\'";
			if (m_decomposition != null)
				xml += " decomposition=\'" + m_decomposition + "\'";
			xml += " />";
			m_dataLayerMorpheme.Other = xml;
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
			m_type = MorphemeType.Stem;
		}

		/// <summary>
		/// Convert the stem to the data layer.
		/// </summary>
		internal override void Convert(IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory)
		{
			LastRecord.Stem = stemFactory.Create();
			m_dataLayerMorpheme = LastRecord.Stem;
			base.Convert(morphemeFactory, stemFactory, affixFactory);
		}

		/// <summary>
		/// Get the count of roots in the stem.
		/// </summary>
		internal int RootCount
		{
			get { return  m_morphname.Split('_').Length/2; }
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
					if (m_underlyingForm == null)
						m_underlyingForm = form;
					else
						m_underlyingForm += "_" + form;
					break;
				}
				case LineType.Category:
				{
					if (m_category == null)
						m_category = form;
					else
						m_category += "_" + form;
					break;
				}
				case LineType.Decomposition:
				{
					if (m_decomposition == null)
						m_decomposition = form;
					else
						m_decomposition += "_" + form;
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
			m_type = MorphemeType.Prefix;
		}

		/// <summary>
		/// Convert the affix to the data layer.
		/// </summary>
		internal override void Convert(IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory)
		{
			var afx = affixFactory.Create();
			m_dataLayerMorpheme = afx;
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
			m_type = MorphemeType.Suffix;
		}

		/// <summary>
		/// Convert the affix to the data layer.
		/// </summary>
		internal override void Convert(IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory)
		{
			var afx = affixFactory.Create();
			LastRecord.Suffixes.Add(afx);
			m_dataLayerMorpheme = afx;
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
