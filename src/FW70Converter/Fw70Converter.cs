using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SIL.WordWorks.GAFAWS.FW70Converter.Properties;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	/// <summary>
	/// Implementation of the IGafawsConverter interface that supports
	/// the FieldWorks 7.0 xml file format.
	/// </summary>
	[Export(typeof(IGafawsConverter))]
	public class Fw70Converter : IGafawsConverter
	{
		[Import(typeof(IWordRecordFactory))]
		private IWordRecordFactory _wordRecordFactory;
		[Import(typeof(IAffixFactory))]
		private IAffixFactory _affixFactory;
		[Import(typeof(IStemFactory))]
		private IStemFactory _stemFactory;
		[Import(typeof(IMorphemeFactory))]
		private IMorphemeFactory _morphemeFactory;

		internal Fw70Converter()
		{}

		#region Implementation of IGafawsConverter

		/// <summary>
		/// Do whatever it takes to convert the input this processor knows about.
		/// </summary>
		public string Convert(IGafawsData gafawsData)
		{
			List<XElement> wordforms;
			Dictionary<string, XElement> analyses;
			Dictionary<string, XElement> morphBundles;
			Dictionary<string, XElement> msas;
			Dictionary<string, XElement> entries;
			Dictionary<string, XElement> forms;
			HashSet<string> humanApprovedEvalIds;
			FwPos selectedPos;
			using (var converterDlg = new Fw70ConverterDlg())
			{
				if (converterDlg.ShowDialog() != DialogResult.OK)
					return null; // bail out, since nothing was selected.

				converterDlg.GetResults(out selectedPos,
					out wordforms,
					out analyses,
					out morphBundles,
					out msas,
					out entries,
					out forms,
					out humanApprovedEvalIds);
			}

			// Process wordform, et al., data.
			// Include selectedPos to get filtered list.
			var wordformsList = new List<FwWordform>(wordforms.Count);
			wordformsList.AddRange(wordforms.Select(wordform => FwWordform.Create(wordform, analyses, humanApprovedEvalIds, selectedPos.AllIds, morphBundles, msas, entries, forms)));
			wordforms.Clear();
			analyses.Clear();
			humanApprovedEvalIds.Clear();
			morphBundles.Clear();
			msas.Clear();
			entries.Clear();
			forms.Clear();

			// Convert all of the wordforms.
			var prefixes = new Dictionary<string, FwMsa>();
			var stems = new Dictionary<string, List<FwMsa>>();
			var suffixes = new Dictionary<string, FwMsa>();
			foreach (var wf in wordformsList)
				wf.Convert(gafawsData, _wordRecordFactory, _morphemeFactory, _affixFactory, _stemFactory, prefixes, stems, suffixes);

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
			var newSets = new Dictionary<string, List<HashSet<IMorpheme>>>();
			foreach (var kvp in gafawsData.ElementarySubgraphs)
			{
				var oldKey = kvp.Key;
				newSets.Add(oldKey == "xxx" ? oldKey : EatIds(oldKey), kvp.Value);
			}
			gafawsData.ElementarySubgraphs.Clear();
			foreach (var kvp in newSets)
				gafawsData.ElementarySubgraphs.Add(kvp.Key, kvp.Value);
		}

		/// <summary>
		/// Gets the name of the converter that is suitable for display in a list
		/// of other converters.
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

		#endregion

		private static string EatIds(string input)
		{
			var output = "";
			var parts = input.Split('_');
			var spacer = "";
			for (var i = 0; i < parts.Length - 1; ++i)
			{
				if (i > 0)
					spacer = "_";
				output += (spacer + parts[i]);
				parts[i + 1] = parts[i + 1].Substring(36).Trim();
			}

			return output.Trim();
		}
	}
}
