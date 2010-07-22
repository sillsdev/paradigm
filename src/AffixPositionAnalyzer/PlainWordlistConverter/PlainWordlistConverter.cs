// --------------------------------------------------------------------------------------------
// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2007, SIL International. All Rights Reserved.
// </copyright>
//
// File: PlainWordlistConverter.cs
// Responsibility: Randy Regnier
// Last reviewed:
//
// <remarks>
// Converts a plain text analyzed wordlist for use by GAFAWS.
// The list will be one analyzed word per line, and follow this format:
// p1-p2-<stem>-s1-s2
// Prefixes or suffixes are optional, but the boundary <stem> is required.
// Optional whitespace can separate affixes and the stem.
// A hyphen is required to mark boundaries between other affixes and the stem.
// Technically, the optional whitespace can be on either side of the hyphen,
// or on both sides of it.
// </remarks>
//
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Xsl;
using SIL.WordWorks.GAFAWS.AffixPositionAnalyzer.Properties;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.AffixPositionAnalyzer.PlainWordlistConverter
{
	public class PlainWordlistConverter : GafawsProcessor, IGafawsConverter
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		internal PlainWordlistConverter()
		{
		}

		#region IGAFAWSConverter implementation

		/// <summary>
		/// Do whatever it takes to convert the input this processor knows about.
		/// </summary>
		public void Convert()
		{
			var openFileDlg = new OpenFileDialog
								{
									InitialDirectory = "c:\\",
									Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
									FilterIndex = 2,
									Multiselect = false
								};

			if (openFileDlg.ShowDialog() == DialogResult.OK)
			{
				var sourcePathname = openFileDlg.FileName;
				if (File.Exists(sourcePathname))
				{
					// Try to convert it.
					using (var reader = new StreamReader(sourcePathname))
					{
						var line = reader.ReadLine();
						var dictPrefixes = new Dictionary<string, bool>();
						var dictStems = new Dictionary<string, bool>();
						var dictSuffixes = new Dictionary<string, bool>();
						while (line != null)
						{
							line = line.Trim();
							if (line != String.Empty)
							{
								var openAngleLocation = line.IndexOf("<", 0);
								if (openAngleLocation < 0)
									continue;
								var closeAngleLocation = line.IndexOf(">", openAngleLocation + 1);
								if (closeAngleLocation < 0)
									continue;
								var wrdRec = new WordRecord();
								m_gd.WordRecords.Add(wrdRec);

								// Handle prefixes, if any.
								string prefixes = null;
								if (openAngleLocation > 0)
									prefixes = line.Substring(0, openAngleLocation);
								if (prefixes != null)
								{
									if (wrdRec.Prefixes == null)
										wrdRec.Prefixes = new List<Affix>();
									foreach (var prefix in prefixes.Split('-'))
									{
										if (string.IsNullOrEmpty(prefix)) continue;

										var afx = new Affix {MIDREF = prefix};
										wrdRec.Prefixes.Add(afx);
										if (dictPrefixes.ContainsKey(prefix)) continue;

										m_gd.Morphemes.Add(new Morpheme(MorphemeType.Prefix, prefix));
										dictPrefixes.Add(prefix, true);
									}
								}

								// Handle stem.
								string sStem;
								// Stem has content, so use it.
								sStem = line.Substring(openAngleLocation + 1, closeAngleLocation - openAngleLocation - 1);
								if (sStem.Length == 0)
									sStem = "stem";
								var stem = new Stem {MIDREF = sStem};
								wrdRec.Stem = stem;
								if (!dictStems.ContainsKey(sStem))
								{
									m_gd.Morphemes.Add(new Morpheme(MorphemeType.Stem, sStem));
									dictStems.Add(sStem, true);
								}

								// Handle suffixes, if any.
								string suffixes = null;
								if (line.Length > closeAngleLocation + 2)
									suffixes = line.Substring(closeAngleLocation + 1);
								if (suffixes != null)
								{
									if (wrdRec.Suffixes == null)
										wrdRec.Suffixes = new List<Affix>();
									foreach (var suffix in suffixes.Split('-'))
									{
										if (string.IsNullOrEmpty(suffix)) continue;

										var afx = new Affix {MIDREF = suffix};
										wrdRec.Suffixes.Add(afx);
										if (dictSuffixes.ContainsKey(suffix)) continue;

										m_gd.Morphemes.Add(new Morpheme(MorphemeType.Suffix, suffix));
										dictSuffixes.Add(suffix, true);
									}
								}
							}
							line = reader.ReadLine();
						}

						// Main processing.
						var anal = new PositionAnalyzer();
						anal.Process(m_gd);

						// Do any post-analysis processing here, if needed.
						// End of any optional post-processing.

						// Save, so it can be transformed.
						var outputPathname = GetOutputPathname(sourcePathname);
						m_gd.SaveData(outputPathname);

						// Transform.
						var trans = new XslCompiledTransform();
						try
						{
							trans.Load(XSLPathname);
						}
						catch
						{
							MessageBox.Show(Resources.kCouldNotLoadFile, Resources.kInformation);
							return;
						}

						var htmlOutput = Path.GetTempFileName() + ".html";
						try
						{
							trans.Transform(outputPathname, htmlOutput);
						}
						catch
						{
							MessageBox.Show(Resources.kCouldNotTransform, Resources.kInformation);
							return;
						}
						finally
						{
							if (outputPathname != null && File.Exists(outputPathname))
								File.Delete(outputPathname);
						}
						Process.Start(htmlOutput);
					} // end 'using'
				}
			}

			// Reset m_gd, in case it gets called for another file.
			m_gd = GAFAWSData.Create();
		}

		/// <summary>
		/// Gets the name of the converter that is suitable for display in a list
		/// of other converts.
		/// </summary>
		public string Name
		{
			get { return "Wordlist converter"; }
		}

		/// <summary>
		/// Gets a description of the converter that is suitable for display.
		/// </summary>
		public string Description
		{
			get { return "Prepare a wordlist for processing.\r\nThe list will follow this pattern:\r\np1-p2-<stem>-s1-s2\r\nAffixes are optional, but the stem/root is not. The content between the stem markers (< and >) is up to the user."; }
		}

		/// <summary>
		/// Gets the pathname of the XSL file used to turn the XML into HTML.
		/// </summary>
		public string XSLPathname
		{
			get
			{
				return Path.Combine(Path.GetDirectoryName(
					Assembly.GetExecutingAssembly().CodeBase),
					"AffixPositionChart_PWL.xsl");
			}
		}

		#endregion IGAFAWSConverter implementation
	}
}