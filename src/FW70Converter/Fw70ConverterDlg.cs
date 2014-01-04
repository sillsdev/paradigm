// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
		[ImportMany]
		private IEnumerable<ILoaderForModelVersion> _loaders;

		public Fw70ConverterDlg()
		{
			InitializeComponent();
			_btnClose.Enabled = false;
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
				// 1. Pick file
				using (var openFileDlg = new OpenFileDialog())
				{
					openFileDlg.Filter = String.Format("{0} (*.fwdata)|*.fwdata", Resources.kFw7XmlFiles);
					openFileDlg.FilterIndex = 2;
					openFileDlg.Multiselect = false;
					if (openFileDlg.ShowDialog() != DialogResult.OK) return;

					_tbPathname.Text = openFileDlg.FileName;
				}

				// 2. Read file contents.
				var lists = new List<XElement>();
				var cats = new Dictionary<string, XElement>();
				// 7.0.6 - DM 7000037
				// 7.1.? - DM 7000044
				// 7.2.> - DM 7000051
				// 8.0.RC1 - DM 7000068
				var doc = XDocument.Load(_tbPathname.Text);
				var modelVersionNumber = uint.Parse(doc.Root.Attribute("version").Value);
				// We want all PartOfSpeech instances that are owned by the list
				// that is owned by the LangProj in its "PartsOfSpeech" property.
				XElement langProj;
				if (modelVersionNumber <= 7000068)
				{
					langProj = Fw7ConverterServices.LoadFileForGeneralVersionNumbers(doc, lists, cats, _wordforms, _analyses, _morphBundles, _msas,
						_entries, _forms, _humanApprovedEvalIds);
				}
				else
				{
					var currentLoader = _loaders.FirstOrDefault(loader => loader.SupportedModelVersion == modelVersionNumber);
					if (currentLoader == null)
					{
						return;
					}
					langProj = currentLoader.LoadFile(doc, lists, cats, _wordforms, _analyses, _morphBundles, _msas, _entries, _forms, _humanApprovedEvalIds);
				}

				// Only keep relevant cats.
				var catList = (from list in lists
							   where list.Attribute("guid").Value.ToLowerInvariant() ==
									 langProj.Element("PartsOfSpeech").Element("objsur").Attribute("guid").Value.ToLowerInvariant()
							   select list).First();
				lists.Clear();
				// Theory has it that FwPos.Create creates the top level instance and then creates all sub=poses, on down to the bottom.
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
	}
}
