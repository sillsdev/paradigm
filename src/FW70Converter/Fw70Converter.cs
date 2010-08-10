using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Chorus.Utilities;
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
		private static readonly Encoding _utf8 = Encoding.UTF8;
		private static readonly byte _closeDoubleQuote = _utf8.GetBytes("\"")[0];
		private static readonly byte _closeSingleQuote = _utf8.GetBytes("'")[0];

		#region Implementation of IGafawsConverter

		/// <summary>
		/// Do whatever it takes to convert the input this processor knows about.
		/// </summary>
		public string Convert(IGafawsData gafawsData)
		{
			string outputpathname = null;

			using (var openFileDlg = new OpenFileDialog())
			{
				openFileDlg.Filter = String.Format("{0} (*.fwdata)|*.fwdata", Resources.kFw7XmlFiles);
				openFileDlg.FilterIndex = 2;
				openFileDlg.Multiselect = false;
				if (openFileDlg.ShowDialog() != DialogResult.OK) return null;

				// TODO: Switch to Palaso lib, when FastXmlElementSplitter moves there.
				XElement langProj = null;
				var lists = new List<XElement>();
				var cats = new Dictionary<string, XElement>();
				var wordforms = new List<XElement>();
				var analyses = new Dictionary<string, XElement>();
				var morphBundles = new Dictionary<string, XElement>();
				var msas = new Dictionary<string, XElement>();
				var entries = new Dictionary<string, XElement>();
				var forms = new Dictionary<string, XElement>();
				var humanApprovedEvalIds = new HashSet<string>();
				using(var splitter = new FastXmlElementSplitter(openFileDlg.FileName))
				{
					// We want all PartOfSpeech instances that are owned by the list
					// that is owned by the LangProj in its "PartsOfSpeech" property.
					foreach (var element in splitter.GetSecondLevelElementBytes("rt"))
					{
						switch (GetClass(element))
						{
							default:
								// Skip it.
								break;
							case "LangProject":
								langProj = XElement.Parse(_utf8.GetString(element));
								break;
							case "CmAgent":
								var agentElement = XElement.Parse(_utf8.GetString(element));
								var humanElement = agentElement.Element("Human");
								if (humanElement != null && humanElement.Attribute("val").Value.ToLowerInvariant() == "true")
									humanApprovedEvalIds.Add(agentElement.Element("Approves").Element("objsur").Attribute("guid").Value.ToLowerInvariant());
								break;
							case "CmPossibilityList":
								lists.Add(XElement.Parse(_utf8.GetString(element)));
								break;
							case "WfiWordform":
								wordforms.Add(XElement.Parse(_utf8.GetString(element)));
								break;
							case "PartOfSpeech":
								AddItem(element, cats);
								break;
							case "WfiAnalysis":
								AddItem(element, analyses);
								break;
							case "WfiMorphBundle":
								AddItem(element, morphBundles);
								break;
							case "LexEntry":
								AddItem(element, entries);
								break;
							case "MoStemAllomorph":
							case "MoAffixProcess":
							case "MoAffixAllomorph":
								AddItem(element, forms);
								break;
							case "MoUnclassifiedAffixMsa":
							case "MoDerivAffMsa":
							case "MoDerivStepMsa":
							case "MoInflAffMsa":
							case "MoStemMsa":
								AddItem(element, msas);
								break;
						}
					}
				}

				// Only keep relevant cats.
				var catList = (from list in lists
							   where list.Attribute("guid").Value.ToLowerInvariant() ==
									langProj.Element("PartsOfSpeech").Element("objsur").Attribute("guid").Value.ToLowerInvariant()
							   select list).First();
				langProj = null;
				lists.Clear();

				var poses = (from objsur in catList.Element("Possibilities").Elements("objsur")
							 select cats[objsur.Attribute("guid").Value.ToLowerInvariant()]).Select(posElement => FwPos.Create(posElement, cats)).ToList();
				cats.Clear();

				// Show cats to user and let them pick one.
				FwPos selectedPos = null;
				using (var catDlg = new Fw70PosSelectorDlg())
				{
					catDlg.SetupDlg(poses);
					if (catDlg.ShowDialog() == DialogResult.OK)
					{
						selectedPos = catDlg.SelectedPos;
					}
					else
					{
						return null; // bail out, since nothing was selected.
					}
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
					wf.Convert(gafawsData, prefixes, stems, suffixes);

				outputpathname = Path.GetTempFileName() + ".xml";
			}
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
						afx.MIDREF = EatIds(afx.MIDREF);
				}

				wr.Stem.MIDREF = EatIds(wr.Stem.MIDREF);

				if (wr.Suffixes == null) continue;

				foreach (var afx in wr.Suffixes)
					afx.MIDREF = EatIds(afx.MIDREF);
			}
			foreach (var morph in gafawsData.Morphemes)
			{
				morph.MID = EatIds(morph.MID);
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

		private static void AddItem(byte[] item, IDictionary<string, XElement> holder)
		{
			var itemElement = XElement.Parse(_utf8.GetString(item));
			holder.Add(itemElement.Attribute("guid").Value.ToLowerInvariant(), itemElement);
		}

		private static string GetClass(byte[] rtElement)
		{
			return GetAttribute(_utf8.GetBytes("class=\""), _closeDoubleQuote, rtElement)
					   ?? GetAttribute(_utf8.GetBytes("class='"), _closeSingleQuote, rtElement);
		}

		private static string GetAttribute(byte[] name, byte closeQuote, byte[] input)
		{
			var start = input.IndexOfSubArray(name);
			if (start == -1)
				return null;

			start += name.Length;
			var end = Array.IndexOf(input, closeQuote, start);
			return (end == -1)
					? null
					: _utf8.GetString(input.SubArray(start, end - start));
		}

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
				//output = output.Trim();
				parts[i + 1] = parts[i + 1].Substring(36).Trim();
			}
			//output += parts[parts.Length - 1];

			return output.Trim();
		}
	}
}
