// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
	[Export(typeof(AffixPositionAnalyzer))]
	public partial class AffixPositionAnalyzer : Form
	{
		[Import(typeof(IGafawsAnalyzer))]
		private IGafawsAnalyzer _analyzer;
		[Import(typeof(IGafawsData))]
		private IGafawsData _gafawsData;
		[ImportMany]
		private IEnumerable<IGafawsConverter> _converters;
		private IGafawsConverter _selectedConverter;
		private string _convertedPathname;

		/// <summary>
		/// Default c'tor
		/// </summary>
		public AffixPositionAnalyzer()
		{
			InitializeComponent();
		}

		private void BtnProcessClick(object sender, EventArgs e)
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
				Console.WriteLine(AppResources.kCrash);
				MessageBox.Show(AppResources.kProblemData, PublicResources.kInformation);
			}
			finally
			{
				if (!string.IsNullOrEmpty(_convertedPathname) && File.Exists(_convertedPathname))
					File.Delete(_convertedPathname);
				Cursor = Cursors.Default;
			}
		}

		private void BtnCloseClick(object sender, EventArgs e)
		{
			Close();
		}

		private void ConvertersSelectedIndexChanged(object sender, EventArgs e)
		{
			SetSelectedConverter();
			if (_selectedConverter != null)
				_tbDescription.Text = _selectedConverter.Description;
		}

		private void ConvertersDoubleClick(object sender, EventArgs e)
		{
			SetSelectedConverter();
			_btnProcess.PerformClick();
		}

		private void SetSelectedConverter()
		{
			_selectedConverter = null;
			if (_lvConverters.SelectedItems.Count <= 0) return;

			_selectedConverter = (IGafawsConverter)_lvConverters.SelectedItems[0].Tag;
		}

		private void AffixPositionAnalyzerLoad(object sender, EventArgs e)
		{
			foreach (var lvi in _converters.Select(gafawsConverter => new ListViewItem(gafawsConverter.Name) { Tag = gafawsConverter }))
			{
				_lvConverters.Items.Add(lvi);
			}
			if (_lvConverters.Items.Count > 0)
				_lvConverters.Items[0].Selected = true;
		}
	}
}