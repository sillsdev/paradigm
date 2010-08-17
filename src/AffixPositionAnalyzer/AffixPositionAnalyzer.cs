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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Xsl;
using SIL.WordWorks.GAFAWS.AffixPositionAnalyzer.Properties;
using SIL.WordWorks.GAFAWS.PositionAnalysis;
using SIL.WordWorks.GAFAWS.PositionAnalysis.Properties;

namespace SIL.WordWorks.GAFAWS.AffixPositionAnalyzer
{
	/// <summary></summary>
	public partial class AffixPositionAnalyzer : Form
	{
		private readonly IGafawsAnalyzer _analyzer;
		private readonly IGafawsData _gafawsData;
		private IGafawsConverter _selectedConverter;
		private string _convertedPathname;

		/// <summary>
		/// Default c'tor
		/// </summary>
		public AffixPositionAnalyzer()
		{
			InitializeComponent();
		}

		public AffixPositionAnalyzer(
			IEnumerable<IGafawsConverter> converters,
			IGafawsAnalyzer analyzer,
			IGafawsData gafawsData)
			: this()
		{
			_analyzer = analyzer;
			_gafawsData = gafawsData;
			foreach (var lvi in converters.Select(gafawsConverter => new ListViewItem(gafawsConverter.Name) {Tag = gafawsConverter}))
			{
				m_lvConverters.Items.Add(lvi);
			}
			if (m_lvConverters.Items.Count > 0)
				m_lvConverters.Items[0].Selected = true;
		}

		private void m_btnProcess_Click(object sender, EventArgs e)
		{
			if (_selectedConverter == null)
				return;

			Cursor = Cursors.WaitCursor;
			try
			{
				_convertedPathname = _selectedConverter.Convert(_gafawsData);
				if (string.IsNullOrEmpty(_convertedPathname))
				{
					MessageBox.Show(AppResources.kNothingToShow);
					return; // Bail out.
				}

				// Main processing.
				_analyzer.Analyze(_gafawsData);
				// Give back to current converter,
				// in case it wants to do more with it now that it has been analyzed.
				_selectedConverter.PostAnalysisProcessing(_gafawsData);

				// Save, so it can be transformed.
				if (!File.Exists(_convertedPathname))
					_gafawsData.SaveData(_convertedPathname);

				_gafawsData.Reset();

				// Transform.
				var trans = new XslCompiledTransform();
				try
				{
					trans.Load(_selectedConverter.XslPathname);
				}
				catch
				{
					MessageBox.Show(PublicResources.kCouldNotLoadFile, PublicResources.kInformation);
					return;
				}

				var htmlOutput = Path.GetTempFileName() + ".html";
				try
				{
					trans.Transform(_convertedPathname, htmlOutput);
				}
				catch
				{
					MessageBox.Show(PublicResources.kCouldNotTransform, PublicResources.kInformation);
					return;
				}
				Process.Start(htmlOutput);
			}
			catch (Exception err)
			{
				Console.WriteLine("Crash. :-(");
				MessageBox.Show(AppResources.kProblemData, PublicResources.kInformation);
			}
			finally
			{
				if (!string.IsNullOrEmpty(_convertedPathname) && File.Exists(_convertedPathname))
					File.Delete(_convertedPathname);
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
			if (_selectedConverter != null)
				m_tbDescription.Text = _selectedConverter.Description;
		}

		private void m_lvConverters_DoubleClick(object sender, EventArgs e)
		{
			SetSelectedConverter();
			m_btnProcess.PerformClick();
		}

		private void SetSelectedConverter()
		{
			_selectedConverter = null;
			if (m_lvConverters.SelectedItems.Count <= 0) return;

			_selectedConverter = (IGafawsConverter)m_lvConverters.SelectedItems[0].Tag;
		}
	}
}