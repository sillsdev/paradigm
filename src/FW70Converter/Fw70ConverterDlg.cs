using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SIL.WordWorks.GAFAWS.FW70Converter.Properties;

namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	public partial class Fw70ConverterDlg : Form
	{
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

		private void BrowseBtnClick(object sender, EventArgs e)
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
				var doc = XDocument.Load(_tbPathname.Text);
				// We want all PartOfSpeech instances that are owned by the list
				// that is owned by the LangProj in its "PartsOfSpeech" property.
				foreach (var currentRtElement in doc.Root.Elements("rt"))
				{
					switch (currentRtElement.Attribute("class").Value)
					{
						// Skip it.
						case "LangProject":
							langProj = currentRtElement;
							break;
						case "CmAgent":
							var agentElement = currentRtElement;
							var humanElement = agentElement.Element("Human");
							if (humanElement != null && humanElement.Attribute("val").Value.ToLowerInvariant() == "true")
								_humanApprovedEvalIds.Add(agentElement.Element("Approves").Element("objsur").Attribute("guid").Value.ToLowerInvariant());
							break;
						case "CmPossibilityList":
							lists.Add(currentRtElement);
							break;
						case "WfiWordform":
							_wordforms.Add(currentRtElement);
							break;
						case "PartOfSpeech":
							AddItem(currentRtElement, cats);
							break;
						case "WfiAnalysis":
							AddItem(currentRtElement, _analyses);
							break;
						case "WfiMorphBundle":
							AddItem(currentRtElement, _morphBundles);
							break;
						case "LexEntry":
							AddItem(currentRtElement, _entries);
							break;
						case "MoStemAllomorph":
						case "MoAffixProcess":
						case "MoAffixAllomorph":
							AddItem(currentRtElement, _forms);
							break;
						case "MoUnclassifiedAffixMsa":
						case "MoDerivAffMsa":
						case "MoDerivStepMsa":
						case "MoInflAffMsa":
						case "MoStemMsa":
							AddItem(currentRtElement, _msas);
							break;
					}
				}

				// Only keep relevant cats.
				var catList = (from list in lists
							   where list.Attribute("guid").Value.ToLowerInvariant() ==
									 langProj.Element("PartsOfSpeech").Element("objsur").Attribute("guid").Value.ToLowerInvariant()
							   select list).First();
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

		private static void AddItem(XElement item, IDictionary<string, XElement> holder)
		{
			holder.Add(item.Attribute("guid").Value.ToLowerInvariant(), item);
		}
	}
}
