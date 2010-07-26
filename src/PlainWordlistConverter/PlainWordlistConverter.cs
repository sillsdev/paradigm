// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2007, SIL International. All Rights Reserved.
// </copyright>
//
// File: PlainWordlistConverter.cs
// Responsibility: Randy Regnier
// Last reviewed:
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml.Xsl;
using SIL.WordWorks.GAFAWS.PlainWordlistConverter.Properties;
using SIL.WordWorks.GAFAWS.PositionAnalysis;
using SIL.WordWorks.GAFAWS.PositionAnalysis.Properties;

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
		private readonly IPositionAnalyzer m_analyzer;

		/// <summary>
		/// An instance of GAFAWSData.
		/// </summary>
		private readonly IGafawsData m_gd;

		/// <summary>
		/// Constructor.
		/// </summary>
		public PlainWordlistConverter(IPositionAnalyzer analyzer, IGafawsData gd)
		{
			m_analyzer = analyzer;
			m_gd = gd;
		}

		#region IGAFAWSConverter implementation

		/// <summary>
		/// Do whatever it takes to convert the input this processor knows about.
		/// </summary>
		public void Convert()
		{
			var openFileDlg = new OpenFileDialog
								{
									//InitialDirectory = "c:\\",
									Filter = String.Format("{0} (*.txt)|*.txt|{1} (*.*)|*.*", Resources.kPlainTexFiles, Resources.kAllFiles),
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
								// Stem has content, so use it.
								var sStem = line.Substring(openAngleLocation + 1, closeAngleLocation - openAngleLocation - 1);
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
						m_analyzer.Process(m_gd);

						// Do any post-analysis processing here, if needed.
						// End of any optional post-processing.

						// Save, so it can be transformed.
						var outputPathname = OutputPathServices.GetOutputPathname(sourcePathname);
						m_gd.SaveData(outputPathname);

						// Transform.
						var trans = new XslCompiledTransform();
						try
						{
							trans.Load(XSLPathname);
						}
						catch
						{
							MessageBox.Show(PublicResources.kCouldNotLoadFile, PublicResources.kInformation);
							return;
						}

						var htmlOutput = Path.GetTempFileName() + ".html";
						try
						{
							trans.Transform(outputPathname, htmlOutput);
						}
						catch
						{
							MessageBox.Show(PublicResources.kCouldNotTransform, PublicResources.kInformation);
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
			m_gd.Reset();
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
		public string XSLPathname
		{
			get
			{
				return OutputPathServices.GetXslPathname("AffixPositionChart_PWL.xsl");
			}
		}

		#endregion IGAFAWSConverter implementation
	}
}
