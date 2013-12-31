// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.ANAConverter
{
	/// <summary>
	/// Summary description for ANAAnalysis.
	/// </summary>
	internal class AnaAnalysis : AnaObject
	{
		private readonly List<AnaPrefix> _prefixes;
		private readonly AnaStem _stem;
		private readonly string _stemAsString;
		private readonly List<AnaSuffix> _suffixes;
		private string _originalForm;
		private string _wordCategory;

		static private char _openRootDelimiter = '<';
		static private char _closeRootDelimiter = '>';
		static private char _separatorCharacter = '-';
		static private char _categorySeparator = '=';
		static private List<Category> _partsOfSpeech;

		static new internal void Reset()
		{
			_openRootDelimiter = '<';
			_closeRootDelimiter = '>';
			_separatorCharacter = '-';
			_categorySeparator = '=';
			_partsOfSpeech = null;
		}

		/// <summary>
		/// PartsOfSpeech.
		/// </summary>
		internal static List<Category> PartsOfSpeech
		{
			set { _partsOfSpeech = value; }
			get { return _partsOfSpeech; }
		}

		/// <summary>
		/// Set the open root delimiter property.
		/// </summary>
		internal static char OpenRootDelimiter
		{
			set { _openRootDelimiter = value; }
		}

		/// <summary>
		/// Set the close root delimiter property.
		/// </summary>
		internal static char CloseRootDelimiter
		{
			set { _closeRootDelimiter = value; }
		}

		/// <summary>
		/// Set the open separator property.
		/// </summary>
		internal static char SeparatorCharacter
		{
			set { _separatorCharacter = value; }
		}

		/// <summary>
		/// Gets or sets the original form.
		/// </summary>
		internal string OriginalForm
		{
			get { return _originalForm; }
			set { _originalForm = value; }
		}

		/// <summary>
		/// Process the content from various analysis fields.
		/// </summary>
		/// <param name="type">The type of line being processed.</param>
		/// <param name="form">The form from the ANA field.</param>
		internal void ProcessContent(LineType type, string form)
		{
			if (_stem == null)
				return;	// Don't process a failure.

			string[] forms;
			char[] sep = {_separatorCharacter};
			if (type == LineType.Category)
			{
				// \cat %5%N N%ADJ ADJ%N N%V VA/V=VA%V VA/V=VA%
				sep = new char[1];
				sep[0] = ' ';
				forms = TokenizeLine(form, sep);
				_wordCategory = forms[0];
				if (form.Length == _wordCategory.Length)
					return;
				form = form.Substring(_wordCategory.Length);
				sep = new char[1];
				sep[0] = _categorySeparator;
			}
			forms = TokenizeLine(form, sep);
			if (MorphemeCount != forms.Length)
			{
				// The count may not match for one of two reasons:
				// 1) The separator used to tokenize the string wasn't in the string, or
				// 2) Sentrans or Stamp didn't fix the contents of the field,
				//	when it did some Transfer operation.
				// We can't process the field in this case,
				// but we can continue the overall conversion.
				return;
			}
			var formCnt = 0;
			int i;
			for (i = 0; _prefixes != null && i < _prefixes.Count; ++i)
				_prefixes[i].AddContent(type, forms[formCnt++]);
			for (i = 0; i < _stem.RootCount; ++i)
				_stem.AddContent(type, forms[formCnt++]);
			for (i = 0; _suffixes != null && i < _suffixes.Count; ++i)
				_suffixes[i].AddContent(type, forms[formCnt++]);
		}

		/// <summary>
		/// Gets the count of morphemes.
		/// </summary>
		internal int MorphemeCount
		{
			get
			{
				var pfxCnt = (_prefixes != null) ? _prefixes.Count : 0;
				var sfxCnt = (_suffixes != null) ? _suffixes.Count : 0;
				return pfxCnt + _stem.RootCount + sfxCnt;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="analysis">An analysis string from the \a field.</param>
		internal AnaAnalysis(string analysis)
		{
			_wordCategory = null;
			_originalForm = null;

			if (analysis == null)
			{
				_prefixes = null;
				_stem = null;
				_suffixes = null;
			}
			else
			{
				char[] seps = {_openRootDelimiter, _closeRootDelimiter};
				var morphemes = TokenizeLine(analysis, seps);
				if (morphemes.Length != 3)
					throw new ApplicationException("Incorrect delimiters.");
				_prefixes = AnaPrefix.TokenizeAffixes(morphemes[0]);
				_stem = new AnaStem(morphemes[1]);
				_stemAsString = morphemes[1]; // For cat filter
				_suffixes = AnaSuffix.TokenizeAffixes(morphemes[2]);
			}
		}

		/// <summary>
		/// Convert the analysis and its morphemes.
		/// </summary>
		internal void Convert(IWordRecordFactory wordRecordFactory, IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory)
		{
			if (_stem == null || (_prefixes == null && _suffixes == null))
				return;	// Don't convert a failure or no affixes.

			// Category Filter
			if ((PartsOfSpeech != null) && (PartsOfSpeech.Count != 0))
			{
				// \cat?
				string[] catCats = {_wordCategory};
				if (_wordCategory != null)
					if (ContainsCat(catCats))
						goto label1;
					else
						return;
				// \a?
				var stemElements = _stemAsString.Split(' ');
				var stemCats = new string[(stemElements.Length - 2)/2];
				for (var i = 0; i < (stemElements.Length - 2)/2; ++i)
					stemCats[i] = new string(stemElements[(i*2)+1].ToCharArray());

				if (ContainsCat(stemCats))
					goto label1;

				return;
			}

label1:
			DoConversion(wordRecordFactory, morphemeFactory, stemFactory, affixFactory);
		}

		private void DoConversion(IWordRecordFactory wordRecordFactory, IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory)
		{
			var wr = wordRecordFactory.Create();
			Gd.WordRecords.Add(wr);
			if (_prefixes != null)
				wr.Prefixes = new List<IAffix>();
			if (_suffixes != null)
				wr.Suffixes = new List<IAffix>();

			if ((_originalForm != null) || (_wordCategory != null))
			{
				var infoElement = new XElement("ANAInfo",
					_originalForm == null ? null : new XAttribute("form", _originalForm),
					_wordCategory == null ? null : new XAttribute("category", _wordCategory));
				wr.Other = infoElement.ToString();
			}

			for (var i = 0; _prefixes != null && i < _prefixes.Count; ++i)
				_prefixes[i].Convert(morphemeFactory, stemFactory, affixFactory);
			_stem.Convert(morphemeFactory, stemFactory, affixFactory);
			for (var i = 0; _suffixes != null && i < _suffixes.Count; ++i)
				_suffixes[i].Convert(morphemeFactory, stemFactory, affixFactory);
		}

		/// <summary>
		/// ContainsCats - check to see if analysis fits cat filter.
		/// </summary>
		/// <returns></returns>
		private static bool ContainsCat(IEnumerable<string> cats)
		{
			return cats.Any(t => PartsOfSpeech.Any(cat => cat.Cat == t));
		}

		/// <summary>
		/// Tokenize the given line with the given separators.
		/// </summary>
		/// <param name="line">The line to tokenize.</param>
		/// <param name="seps">The characters used to tokenize the given string.</param>
		/// <returns>An array of token strings.</returns>
		protected string[] TokenizeLine(string line, char[] seps)
		{
			return line.Trim().Split(seps);
		}
	}
}