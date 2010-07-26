// <copyright from='2007' to='2010' company='SIL International'>
//		Copyright (c) 2007, SIL International. All Rights Reserved.
//
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright>
//
// File: AffixPositionAnalyzer.cs
// Responsibility: Randy Regnier
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SIL.WordWorks.GAFAWS.PositionAnalysis;
using SIL.WordWorks.GAFAWS.PositionAnalysis.Properties;

namespace SIL.WordWorks.GAFAWS.AffixPositionAnalyzer
{
	/// <summary></summary>
	public partial class AffixPositionAnalyzer : Form
	{
		private IGafawsConverter m_selectedConverter;

		/// <summary>
		/// Default c'tor
		/// </summary>
		public AffixPositionAnalyzer()
		{
			InitializeComponent();
		}

		internal AffixPositionAnalyzer(IEnumerable<IGafawsConverter> converters)
			: this()
		{
			foreach (var lvi in converters.Select(gafawsConverter => new ListViewItem(gafawsConverter.Name) {Tag = gafawsConverter}))
			{
				m_lvConverters.Items.Add(lvi);
			}
			if (m_lvConverters.Items.Count > 0)
				m_lvConverters.Items[0].Selected = true;
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
				MessageBox.Show(AppResources.kProblemData, PublicResources.kInformation);
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