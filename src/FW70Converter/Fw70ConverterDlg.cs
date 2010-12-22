using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Chorus.Utilities;
using SIL.WordWorks.GAFAWS.FW70Converter.Properties;

namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	public partial class Fw70ConverterDlg : Form
	{
		private static readonly Encoding _utf8 = Encoding.UTF8;
		private static readonly byte _closeDoubleQuote = _utf8.GetBytes("\"")[0];
		private static readonly byte _closeSingleQuote = _utf8.GetBytes("'")[0];
		private FwPos _selectedPos;
		private readonly List<XElement> _wordforms = new List<XElement>();
		private readonly Dictionary<string, XElement> _analyses = new Dictionary<string, XElement>();
		private readonly Dictionary<string, XElement> _morphBundles = new Dictionary<string, XElement>();
		private readonly Dictionary<string, XElement> _msas = new Dictionary<string, XElement>();
		private readonly Dictionary<string, XElement> _entries = new Dictionary<string, XElement>();
		private readonly Dictionary<string, XElement> _forms = new Dictionary<string, XElement>();
		private readonly HashSet<string> _humanApprovedEvalIds = new HashSet<string>();

		public Fw70ConverterDlg()
		{
			InitializeComponent();
			_btnClose.Enabled = false;
		}

		internal void SetupDlg(List<FwPos> poses)
		{
			foreach (var pos in poses)
			{
				var nodeCol = _tvPoses.Nodes;
				AddNode(nodeCol, pos);
			}
		}

		private static void AddNode(TreeNodeCollection treeNodeCollection, FwPos pos)
		{
			var tn = new TreeNode {Tag = pos, Text = pos.ToString()};
			treeNodeCollection.Add(tn);
			foreach (var subCat in pos.SubCats)
				AddNode(tn.Nodes, subCat);
		}

		private void BtnCloseClick(object sender, EventArgs e)
		{
			_selectedPos = (FwPos) _tvPoses.SelectedNode.Tag;
			Close();
		}

		private void TvPosesDoubleClick(object sender, EventArgs e)
		{
			_btnClose.PerformClick();
		}

		internal FwPos SelectedPos
		{
			get { return _selectedPos; }
		}

		internal void GetResults(
			out FwPos selectedPos,
			out List<XElement> wordforms,
			out Dictionary<string, XElement> analyses,
			out Dictionary<string, XElement> morphBundles,
			out Dictionary<string, XElement> msas,
			out Dictionary<string, XElement> entries,
			out Dictionary<string, XElement> forms,
			out HashSet<string> humanApprovedEvalIds)
		{
			selectedPos = _selectedPos;
			wordforms = _wordforms;
			analyses = _analyses;
			morphBundles = _morphBundles;
			msas = _msas;
			entries = _entries;
			forms = _forms;
			humanApprovedEvalIds = _humanApprovedEvalIds;
		}

		private void BrowseBtn_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			try
			{
				// 1. Pick file, and
				using (var openFileDlg = new OpenFileDialog())
				{
					openFileDlg.Filter = String.Format("{0} (*.fwdata)|*.fwdata", Resources.kFw7XmlFiles);
					openFileDlg.FilterIndex = 2;
					openFileDlg.Multiselect = false;
					if (openFileDlg.ShowDialog() != DialogResult.OK) return;

					_tbPathname.Text = openFileDlg.FileName;
				}

				// 2. Read file contents.
				XElement langProj = null;
				var lists = new List<XElement>();
				var cats = new Dictionary<string, XElement>();
				using (var splitter = new FastXmlElementSplitter(_tbPathname.Text))
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
									_humanApprovedEvalIds.Add(agentElement.Element("Approves").Element("objsur").Attribute("guid").Value.ToLowerInvariant());
								break;
							case "CmPossibilityList":
								lists.Add(XElement.Parse(_utf8.GetString(element)));
								break;
							case "WfiWordform":
								_wordforms.Add(XElement.Parse(_utf8.GetString(element)));
								break;
							case "PartOfSpeech":
								AddItem(element, cats);
								break;
							case "WfiAnalysis":
								AddItem(element, _analyses);
								break;
							case "WfiMorphBundle":
								AddItem(element, _morphBundles);
								break;
							case "LexEntry":
								AddItem(element, _entries);
								break;
							case "MoStemAllomorph":
							case "MoAffixProcess":
							case "MoAffixAllomorph":
								AddItem(element, _forms);
								break;
							case "MoUnclassifiedAffixMsa":
							case "MoDerivAffMsa":
							case "MoDerivStepMsa":
							case "MoInflAffMsa":
							case "MoStemMsa":
								AddItem(element, _msas);
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

				// 3. Populate tree view.
				_tvPoses.SuspendLayout();
				_tvPoses.Nodes.Clear();
				foreach (var pos in poses)
				{
					var nodeCol = _tvPoses.Nodes;
					AddNode(nodeCol, pos);
				}
				_tvPoses.ResumeLayout();
				if (_tvPoses.Nodes.Count > 0)
				{
					_tvPoses.SelectedNode = _tvPoses.Nodes[0];
					_btnClose.Enabled = true;
				}
				else
				{
					_btnClose.Enabled = false;
				}
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

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
	}
}
