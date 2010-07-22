// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2007, SIL International. All Rights Reserved.
// <copyright from='2007' to='2010' company='SIL International'>
//		Copyright (c) 2007, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
#endregion
//
// File: AffixPositionAnalyzer.cs
// Responsibility: Randy Regnier
//
// <remarks>
// </remarks>
// ---------------------------------------------------------------------------------------------
using System;
using System.Windows.Forms;
using SIL.WordWorks.GAFAWS.AffixPositionAnalyzer.ANAConverter;
using SIL.WordWorks.GAFAWS.AffixPositionAnalyzer.Properties;
using SIL.WordWorks.GAFAWS.PositionAnalysis;

namespace SIL.WordWorks.GAFAWS.AffixPositionAnalyzer
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public partial class AffixPositionAnalyzer : Form
	{
		private IGafawsConverter m_selectedConverter;
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Default c'tor
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public AffixPositionAnalyzer()
		{
			InitializeComponent();

			// Load up the converters.
			var converter = (IGafawsConverter)new PlainWordlistConverter.PlainWordlistConverter();
			var lvi = new ListViewItem(converter.Name) {Tag = converter};
			m_lvConverters.Items.Add(lvi);
			lvi.Selected = true;

			converter = new ANAGAFAWSConverter();
			lvi = new ListViewItem(converter.Name) {Tag = converter};
			m_lvConverters.Items.Add(lvi);

			//converter = new FWConverter();
			//lvi = new ListViewItem(converter.Name) {Tag = converter};
			//m_lvConverters.Items.Add(lvi);

			// TODO: Load up the other converters.
		}

		private void m_btnProcess_Click(object sender, EventArgs e)
		{
			if (m_selectedConverter == null)
				return;

			Cursor = Cursors.WaitCursor;
			try
			{
				m_selectedConverter.Convert();
			}
			catch
			{
				MessageBox.Show("There were problems with the original data, and it could not be processed.", Resources.kInformation);
			}
			finally
			{
				Cursor = Cursors.Default;
			}
		}

		private void m_btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void m_lvConverters_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetSelectedConverter();
			if (m_selectedConverter != null)
				m_tbDescription.Text = m_selectedConverter.Description;
		}

		private void m_lvConverters_DoubleClick(object sender, EventArgs e)
		{
			SetSelectedConverter();
			m_btnProcess.PerformClick();
		}

		private void SetSelectedConverter()
		{
			m_selectedConverter = null;
			if (m_lvConverters.SelectedItems.Count <= 0) return;

			m_selectedConverter = (IGafawsConverter)m_lvConverters.SelectedItems[0].Tag;
		}
	}
}