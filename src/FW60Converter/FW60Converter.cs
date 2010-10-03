// <copyright from='2003' to='2010' company='SIL International'>
//    Copyright (c) 2007, SIL International. All Rights Reserved.
// </copyright>
//
// File: FW60Converter.cs
// Responsibility: Randy Regnier
// Last reviewed:
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SIL.WordWorks.GAFAWS.FW60Converter.Properties;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.FW60Converter
{
	public class Fw60Converter : IGafawsConverter
	{
		private readonly IWordRecordFactory _wordRecordFactory;
		private readonly IAffixFactory _affixFactory;
		private readonly IStemFactory _stemFactory;
		private readonly IMorphemeFactory _morphemeFactory;

		internal Fw60Converter(
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

		#region IGafawsConverter implementation

		/// <summary>
		/// Do whatever it takes to convert the input this processor knows about.
		/// </summary>
		public string Convert(IGafawsData gafawsData)
		{
			using (var dlg = new FwConverterDlg())
			{
				dlg.ShowDialog();
				if (dlg.DialogResult != DialogResult.OK || dlg.CatInfo == null)
					return null;

				SqlConnection con = null;
				try
				{
					// 0 is the category id.
					// 1 is the entire connection string.
					var parts = dlg.CatInfo.Split('^');
					con = new SqlConnection(parts[1]);
					con.Open();
					using (var cmd = con.CreateCommand())
					{
						cmd.CommandType = CommandType.Text;
						string catIdQry;
						if (dlg.IncludeSubcategories)
						{
							catIdQry = string.Format("IN ({0}", parts[0]);
							cmd.CommandText = "SELECT Id\n" +
											  string.Format("FROM fnGetOwnedIds({0}, 7004, 7004)", parts[0]);
							using (var reader = cmd.ExecuteReader())
							{
								while (reader.Read())
									catIdQry += string.Format(", {0}", reader.GetInt32(0));
							}
							catIdQry += ")";
						}
						else
						{
							catIdQry = string.Format("= {0}", parts[0]);
						}
						cmd.CommandText =
							"SELECT anal.Owner$ AS Wf_Id,\n" +
							"	anal.Id AS Anal_Id,\n" +
							"	mb.OwnOrd$ AS Mb_Ord,\n" +
							"	Mb.Sense AS Mb_Sense,\n" +
							"	mb.Morph AS Mb_Morph,\n" +
							"	mb.Msa AS Mb_Msa, msa.Class$ AS Msa_Class\n" +
							"FROM WfiAnalysis_ anal\n" +
							"--) Only use those that are human approved\n" +
							"JOIN CmAgentEvaluation eval ON eval.Target = anal.Id\n" +
							"JOIN CmAgent agt ON agt.Human = 1\n" +
							"JOIN CmAgent_Evaluations j_agt_eval ON agt.Id = j_agt_eval.Src AND j_agt_eval.Dst = eval.Id\n" +
							"--) Get morph bundles\n" +
							"JOIN WfiMorphBundle_ mb ON mb.Owner$ = anal.Id\n" +
							"--) Get MSA class\n" +
							"LEFT OUTER JOIN MoMorphSynAnalysis_ msa ON mb.msa = msa.Id\n" +
							String.Format("WHERE anal.Category {0} AND eval.Accepted = 1\n", catIdQry) +
							"ORDER BY anal.Owner$, anal.Id, mb.OwnOrd$";
						var wordforms = ReadWordforms(cmd);
						// Convert all of the wordforms.
						var prefixes = new Dictionary<string, FwMsa>();
						var stems = new Dictionary<string, List<FwMsa>>();
						var suffixes = new Dictionary<string, FwMsa>();
						foreach (var wf in wordforms)
							wf.Convert(cmd, gafawsData, _wordRecordFactory, _morphemeFactory, _stemFactory, _affixFactory, prefixes, stems, suffixes);
					}
				}
				catch
				{
					// Eat exceptions.
				}
				finally
				{
					if (con != null)
						con.Close();
				}
			}
			return Path.GetTempFileName() + ".xml";
		}

		/// <summary>
		/// Optional processing after the conversion and analysis has been done.
		/// </summary>
		/// <param name="gafawsData"></param>
		public void PostAnalysisProcessing(IGafawsData gafawsData)
		{
			// Strip out all the _#### here.
			foreach (var wr in gafawsData.WordRecords)
			{
				if (wr.Prefixes != null)
				{
					foreach (var afx in wr.Prefixes)
						afx.Id = EatIds(afx.Id);
				}

				wr.Stem.Id = EatIds(wr.Stem.Id);

				if (wr.Suffixes == null) continue;

				foreach (var afx in wr.Suffixes)
					afx.Id = EatIds(afx.Id);
			}
			foreach (var morph in gafawsData.Morphemes)
			{
				morph.Id = EatIds(morph.Id);
			}
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
				return string.Format(Resources.kDescription, Environment.NewLine);
			}
		}

		/// <summary>
		/// Gets the pathname of the XSL file used to turn the XML into HTML.
		/// </summary>
		public string XslPathname
		{
			get
			{
				return OutputPathServices.GetXslPathname("AffixPositionChart.xsl");
			}
		}

		#endregion IGafawsConverter implementation

		private static IEnumerable<FwWordform> ReadWordforms(SqlCommand cmd)
		{
			var wordforms = new List<FwWordform>();
			using (var reader = cmd.ExecuteReader())
			{
				var moreRows = reader.Read();
				while (moreRows)
				{
					/*
										 * Return values, in order are:
										 *	Wordform Id: int: 0
										 *	Analysis Id: int: 1
										 *	MorphBundle Ord: int: 2
										 *	Sense Id: int: 3
										 *	MoForm Id: int: 4
										 *	MSA Id: int: 5
										 *	MSA Class: int: 6
										*/
					var wordform = new FwWordform();
					moreRows = wordform.LoadFromDB(reader);
					wordforms.Add(wordform);
				}
			}
			return wordforms;
		}

		private static string EatIds(string input)
		{
			var output = "";
			var parts = input.Split('_');
			for (var i = 0; i < parts.Length - 1; ++i)
			{
				output += parts[i];
				var nextPart = parts[i + 1];
				while (nextPart.IndexOfAny(new[] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0'}) == 0)
				{
					nextPart = nextPart.Substring(1);
				}
				parts[i + 1] = nextPart;
			}
			output += parts[parts.Length - 1];

			return output.Trim();
		}
	}

	internal class FwWordform
	{
		private int m_id;
		private readonly List<FwAnalysis> m_analyses = new List<FwAnalysis>();

		/// <summary>
		/// Load one wordform from DB 'reader'.
		/// As long as the wf id is the same, then keep going,
		/// as it is the same wordform.
		/// </summary>
		/// <param name="reader">'true' if there are more rows (wordforms) to process, otherwise, 'false'.</param>
		/// <returns></returns>
		internal bool LoadFromDB(SqlDataReader reader)
		{
			var moreRows = true; // Start on the optimistic side.
			if (m_id == 0)
			{
				var id = reader.GetInt32(0);
				m_id = id;
				while (moreRows && (reader.GetInt32(0) == m_id))
				{
					var anal = new FwAnalysis();
					moreRows = anal.LoadFromDB(reader);
					m_analyses.Add(anal);
				}
			}
			return moreRows;
		}

		internal void Convert(SqlCommand cmd, IGafawsData gData,
			IWordRecordFactory wordRecordFactory,
			IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory,
			Dictionary<string, FwMsa> prefixes, Dictionary<string, List<FwMsa>> stems, Dictionary<string, FwMsa> suffixes)
		{
			foreach (var anal in m_analyses)
				anal.Convert(cmd, gData, wordRecordFactory, morphemeFactory, stemFactory, affixFactory, prefixes, stems, suffixes);
		}
	}

	internal class FwAnalysis
	{
		private int m_id;
		private readonly SortedDictionary<int, FwMorphBundle> m_morphBundles = new SortedDictionary<int, FwMorphBundle>();

		/// <summary>
		/// Load one wordform from DB 'reader'.
		/// As long as the wf id is the same, then keep going,
		/// as it is the same wordform.
		/// </summary>
		/// <param name="reader">'true' if there are more rows (wordforms) to process, otherwise, 'false'.</param>
		/// <returns></returns>
		internal bool LoadFromDB(SqlDataReader reader)
		{
			var moreRows = true; // Start on the optimistic side.

			if (m_id == 0)
			{
				var analId = reader.GetInt32(1);
				m_id = analId;
				while (moreRows && (reader.GetInt32(1) == m_id))
				{
					var ord = reader.GetInt32(2);
					var msaId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
					var msaClass = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
					var mb = new FwMorphBundle(new FwMsa(msaId, msaClass));
					moreRows = reader.Read();
					m_morphBundles.Add(ord, mb);
				}
			}

			return moreRows;
		}

		private bool CanConvert
		{
			get
			{
				return !m_morphBundles.Values.Any(mb => mb.MSA.Id == 0) &&
					   ((m_morphBundles.Count != 1 || m_morphBundles[1].MSA.Class == 5001) && m_morphBundles.Count > 0);
			}
		}

		internal void Convert(SqlCommand cmd, IGafawsData gData,
			IWordRecordFactory wordRecordFactory,
			IMorphemeFactory morphemeFactory, IStemFactory stemFactory, IAffixFactory affixFactory,
			Dictionary<string, FwMsa> prefixes, Dictionary<string, List<FwMsa>> stems, Dictionary<string, FwMsa> suffixes)
		{
			if (!CanConvert)
				return;

			var wr = wordRecordFactory.Create();
			// Deal with prefixes, if any.
			var startStemOrd = 0;
			foreach (var kvp in m_morphBundles)
			{
				var mb = kvp.Value;
				var msaKey = mb.GetMsaKey(cmd);
				if (mb.MSA.Class == 5001 || mb.MSA.Class == 5031 || mb.MSA.Class == 5032 || mb.MSA.Class == 5117) // What about 5117-MoUnclassifiedAffixMsa?
				{
					// stem or derivational prefix, so bail out of this loop.
					startStemOrd = kvp.Key;
					break;
				}

				// Add prefix, if not already present.
				if (wr.Prefixes == null)
					wr.Prefixes = new List<IAffix>();
				if (!prefixes.ContainsKey(msaKey))
				{
					prefixes.Add(msaKey, mb.MSA);
					gData.Morphemes.Add(morphemeFactory.Create(MorphemeType.Prefix, msaKey));
				}
				var afx = affixFactory.Create();
				afx.Id = msaKey;
				wr.Prefixes.Add(afx);
			}

			// Deal with suffixes, if any.
			// Work through the suffixes from the end of the word.
			// We stop when we hit the stem or a derivational suffix.
			var endStemOrd = 0;
			for (var i = m_morphBundles.Count; i > 0; --i)
			{
				var mb = m_morphBundles[i];
				var msaKey = mb.GetMsaKey(cmd);
				if (mb.MSA.Class == 5001 || mb.MSA.Class == 5031 || mb.MSA.Class == 5032 || mb.MSA.Class == 5117) // What about 5117-MoUnclassifiedAffixMsa?
				{
					// stem or derivational suffix, so bail out of this loop.
					endStemOrd = i;
					break;
				}

				// Add suffix, if not already present.
				if (wr.Suffixes == null)
					wr.Suffixes = new List<IAffix>();
				if (!suffixes.ContainsKey(msaKey))
				{
					suffixes.Add(msaKey, mb.MSA);
					gData.Morphemes.Add(morphemeFactory.Create(MorphemeType.Suffix, msaKey));
				}
				var afx = affixFactory.Create();
				afx.Id = msaKey;
				wr.Suffixes.Insert(0, afx);
			}

			// Deal with stem.
			var localStems = new List<FwMsa>();
			var sStem = "";
			foreach (var kvp in m_morphBundles)
			{
				var mb = kvp.Value;
				var currentOrd = kvp.Key;
				if (currentOrd < startStemOrd || currentOrd > endStemOrd) continue;

				var msaKey = mb.GetMsaKey(cmd);
				var spacer = (currentOrd == 1) ? "" : " ";
				sStem += spacer + msaKey;
			}
			if (!stems.ContainsKey(sStem))
			{
				stems.Add(sStem, localStems);
				gData.Morphemes.Add(morphemeFactory.Create(MorphemeType.Stem, sStem));
			}

			var stem = stemFactory.Create();
			stem.Id = sStem;
			wr.Stem = stem;

			// Add wr.
			gData.WordRecords.Add(wr);
		}
	}

	internal class FwMorphBundle
	{
		private readonly FwMsa m_msa;

		internal FwMorphBundle(FwMsa msa)
		{
			m_msa = msa;
		}

		internal FwMsa MSA
		{
			get { return m_msa; }
		}

		internal string GetMsaKey(SqlCommand cmd)
		{
			var msaBase = "_" + m_msa.Id;
			cmd.CommandText = "SELECT mff.Txt\n" +
				"FROM CmObject msa\n" +
				"JOIN LexEntry_LexemeForm j_entry_form ON msa.Owner$ = j_entry_form.Src\n" +
				"JOIN MoForm_Form mff ON mff.Obj = j_entry_form.Dst\n" +
				"WHERE msa.Id =" + m_msa.Id;
			var txt = (string)cmd.ExecuteScalar();
			if (string.IsNullOrEmpty(txt))
				txt = "???";
			return txt + msaBase;
		}
	}

	internal class FwMsa
	{
		private readonly int m_id;
		private readonly int m_msaClass;

		internal FwMsa(int id, int msaClass)
		{
			m_id = id;
			m_msaClass = msaClass;
		}

		internal int Class
		{
			get { return m_msaClass; }
		}

		internal int Id
		{
			get { return m_id; }
		}
	}
}
