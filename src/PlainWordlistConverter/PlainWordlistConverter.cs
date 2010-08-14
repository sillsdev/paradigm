﻿// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2007, SIL International. All Rights Reserved.
// </copyright>
//
// File: PlainWordlistConverter.cs
// Responsibility: Randy Regnier
// Last reviewed:
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SIL.WordWorks.GAFAWS.PlainWordlistConverter.Properties;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.PlainWordlistConverter
{
	/// <summary>
	/// Converts a plain text analyzed wordlist for use by GAFAWS.
	///
	/// The list will be one analyzed word per line, and follow this format:
	/// p1-p2-&lt;stem&gt;-s1-s2
	///
	/// Prefixes or suffixes are optional, but the boundary &lt;stem&gt; is required.
	///
	/// Optional whitespace can separate affixes and the stem.
	///
	/// A hyphen is required to mark boundaries between other affixes and the stem.
	/// Technically, the optional whitespace can be on either side of the hyphen,
	/// or on both sides of it.
	/// </summary>
	public class PlainWordlistConverter : IGafawsConverter
	{
		private readonly IWordRecordFactory _wordRecordFactory;
		private readonly IAffixFactory _affixFactory;
		private readonly IStemFactory _stemFactory;
		private readonly IMorphemeFactory _morphemeFactory;

		public PlainWordlistConverter(
			IWordRecordFactory wordRecordFactory,
			IAffixFactory affixFactory,
			IStemFactory stemFactory,
			IMorphemeFactory morphemeFactory)
		{
			_wordRecordFactory = wordRecordFactory;
			_affixFactory = affixFactory;
			_stemFactory = stemFactory;
			_morphemeFactory = morphemeFactory;
		}

		#region IGAFAWSConverter implementation

		/// <summary>
		/// Do whatever it takes to convert the input this processor knows about.
		/// </summary>
		public string Convert(IGafawsData gafawsData)
		{
			using (var openFileDlg = new OpenFileDialog())
			{
				openFileDlg.Filter = String.Format("{0} (*.txt)|*.txt|{1} (*.*)|*.*", Resources.kPlainTexFiles, Resources.kAllFiles);
				openFileDlg.FilterIndex = 2;
				openFileDlg.Multiselect = false;
				if (openFileDlg.ShowDialog() != DialogResult.OK)
					return null;
				var sourcePathname = openFileDlg.FileName;
				if (!File.Exists(sourcePathname))
					return null;

				var outputPathname = OutputPathServices.GetOutputPathname(sourcePathname);
				// Try to convert it.
				using (var reader = new StreamReader(sourcePathname))
				{
					var line = reader.ReadLine();
					var dictPrefixes = new Dictionary<string, bool>();
					var dictStems = new Dictionary<string, bool>();
					var dictSuffixes = new Dictionary<string, bool>();
					while (line != null)
					{
						var parts = line.Split(';');
						if (parts.Length > 1)
							line = parts[0];
						line = line.Trim();
						if (line != String.Empty)
						{
							var openAngleLocation = line.IndexOf("<", 0);
							if (openAngleLocation < 0)
								continue;
							var closeAngleLocation = line.IndexOf(">", openAngleLocation + 1);
							if (closeAngleLocation < 0)
								continue;
							var wrdRec = _wordRecordFactory.Create();
							gafawsData.WordRecords.Add(wrdRec);

							// Handle prefixes, if any.
							string prefixes = null;
							if (openAngleLocation > 0)
								prefixes = line.Substring(0, openAngleLocation);
							if (prefixes != null)
							{
								if (wrdRec.Prefixes == null)
									wrdRec.Prefixes = new List<IAffix>();
								foreach (var prefix in prefixes.Split('-'))
								{
									if (string.IsNullOrEmpty(prefix)) continue;

									var afx = _affixFactory.Create();
									afx.MidRef = prefix;
									wrdRec.Prefixes.Add(afx);
									if (dictPrefixes.ContainsKey(prefix)) continue;

									gafawsData.Morphemes.Add(_morphemeFactory.Create(MorphemeType.Prefix, prefix));
									dictPrefixes.Add(prefix, true);
								}
							}

							// Handle stem.
							// Stem has content, so use it.
							var sStem = line.Substring(openAngleLocation + 1, closeAngleLocation - openAngleLocation - 1);
							if (sStem.Length == 0)
								sStem = "stem";
							var stem = _stemFactory.Create();
							stem.MidRef = sStem;
							wrdRec.Stem = stem;
							if (!dictStems.ContainsKey(sStem))
							{
								gafawsData.Morphemes.Add(_morphemeFactory.Create(MorphemeType.Stem, sStem));
								dictStems.Add(sStem, true);
							}

							// Handle suffixes, if any.
							string suffixes = null;
							if (line.Length > closeAngleLocation + 2)
								suffixes = line.Substring(closeAngleLocation + 1);
							if (suffixes != null)
							{
								if (wrdRec.Suffixes == null)
									wrdRec.Suffixes = new List<IAffix>();
								foreach (var suffix in suffixes.Split('-'))
								{
									if (string.IsNullOrEmpty(suffix)) continue;

									var afx = _affixFactory.Create();
									afx.MidRef = suffix;
									wrdRec.Suffixes.Add(afx);
									if (dictSuffixes.ContainsKey(suffix)) continue;

									gafawsData.Morphemes.Add(_morphemeFactory.Create(MorphemeType.Suffix, suffix));
									dictSuffixes.Add(suffix, true);
								}
							}
						}
						line = reader.ReadLine();
					}
				} // end 'using' for StreamReader
				return outputPathname;
			}
		}

		/// <summary>
		/// Optional processing after the conversion and analysis has been done.
		/// </summary>
		/// <param name="gafawsData"></param>
		public void PostAnalysisProcessing(IGafawsData gafawsData)
		{
		}

		/// <summary>
		/// Gets the name of the converter that is suitable for display in a list
		/// of other converts.
		/// </summary>
		public string Name
		{
			get { return Resources.kName; }
		}

		/// <summary>
		/// Gets a description of the converter that is suitable for display.
		/// </summary>
		public string Description
		{
			get
			{
				return string.Format(Resources.kDescription, Environment.NewLine); }
		}

		/// <summary>
		/// Gets the pathname of the XSL file used to turn the XML into HTML.
		/// </summary>
		public string XslPathname
		{
			get
			{
				return OutputPathServices.GetXslPathname("AffixPositionChart_PWL.xsl");
			}
		}

		#endregion IGAFAWSConverter implementation
	}
}
