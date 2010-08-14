using System;
using System.Collections.Generic;
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
	public class Fw70Converter : IGafawsConverter
	{
		private readonly IWordRecordFactory _wordRecordFactory;
		private readonly IAffixFactory _affixFactory;
		private readonly IStemFactory _stemFactory;
		private readonly IMorphemeFactory _morphemeFactory;

		internal Fw70Converter(
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

		#region Implementation of IGafawsConverter

		/// <summary>
		/// Do whatever it takes to convert the input this processor knows about.
		/// </summary>
		public string Convert(IGafawsData gafawsData)
		{
			string outputpathname = null;

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
					return outputpathname; // bail out, since nothing was selected.

				converterDlg.GetReults(out selectedPos,
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

			outputpathname = Path.GetTempFileName() + ".xml";

			return outputpathname;
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
				return OutputPathServices.GetXslPathname("AffixPositionChart_FW7.0.xsl");
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
