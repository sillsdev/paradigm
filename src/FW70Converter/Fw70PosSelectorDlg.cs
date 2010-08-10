using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	public partial class Fw70PosSelectorDlg : Form
	{
		public Fw70PosSelectorDlg()
		{
			InitializeComponent();
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
			Close();
		}

		private void TvPosesDoubleClick(object sender, EventArgs e)
		{
			_btnClose.PerformClick();
		}

		internal FwPos SelectedPos
		{
			get { return (FwPos) _tvPoses.SelectedNode.Tag; }
		}
	}
}
