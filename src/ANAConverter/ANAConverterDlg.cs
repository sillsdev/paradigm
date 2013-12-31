// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SIL.WordWorks.GAFAWS.ANAConverter.Properties;

namespace SIL.WordWorks.GAFAWS.ANAConverter
{
	/// <summary>
	/// Summary description for AnaConverterDlg.
	/// </summary>
	internal class AnaConverterDlg : Form
	{
		#region Data members

		private Button _btnAnal;
		private ToolTip _tipBtnAnal;
		private Button _btnClose;
		private ToolTip _tipBtnClose;
		private Button _btnBrowse;
		private ToolTip _tipBtnBrowse;
		private TextBox _tbAnaFile;
		private ToolTip _tipTbAnaFile;
		private CheckedListBox _chBxCategories;
		private ToolTip _tipChBxCategories;
		private TextBox _tbAmbigMarker;
		private ToolTip _tipTbAmbigMarker;
		private Label _lbAmbMkr;
		// No tool tip for labels
		private Label _lbOpenDel;
		// No tool tip for labels
		private TextBox _tbOpenDel;
		private ToolTip _tipTbOpenDel;
		private Label _lbCloseDel;
		// No tool tip for labels
		private TextBox _tbCloseDel;
		private ToolTip _tipTbCloseDel;
		private Label _lbAffixSep;
		// No tool tip for labels
		private TextBox _tbAffixSep;
		private ToolTip _tipTbAffixSep;
		private Button _btnSelect;
		private ToolTip _tipBtnSelect;
		private Label _lbCatAnal;
		// No tool tip for labels
		private Label _lbAnaFile;
		// No tool tip for labels
		private HelpProvider _helpMeOne;
		// No tool tip for labels
		private ToolTip _tipBtnOk;

		private IContainer components;

		#endregion Data members

		#region Properties

		private string AmbiguityMarker
		{
			get { return _tbAmbigMarker.Text; }
		}

		private string AffixSeparator
		{
			get { return _tbAffixSep.Text; }
		}

		private string OpenDelimiter
		{
			get { return _tbOpenDel.Text; }
		}

		private string CloseDelimiter
		{
			get { return _tbCloseDel.Text; }
		}

		#endregion Properties

		#region Construction and disposal

		/// <summary>
		/// Initializes a new instance of the MainWnd class.
		/// </summary>
		internal AnaConverterDlg()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			//Debug.WriteLineIf(!disposing, "****************** " + GetType().Name + " 'disposing' is false. ******************");
			// Must not be run more than once.
			if (IsDisposed)
				return;

			if( disposing )
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion Construction and disposal

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnaConverterDlg));
			this._btnAnal = new System.Windows.Forms.Button();
			this._tipBtnAnal = new System.Windows.Forms.ToolTip(this.components);
			this._btnClose = new System.Windows.Forms.Button();
			this._tbAnaFile = new System.Windows.Forms.TextBox();
			this._tipBtnClose = new System.Windows.Forms.ToolTip(this.components);
			this._btnBrowse = new System.Windows.Forms.Button();
			this._tipTbAnaFile = new System.Windows.Forms.ToolTip(this.components);
			this._tipBtnBrowse = new System.Windows.Forms.ToolTip(this.components);
			this._tbAmbigMarker = new System.Windows.Forms.TextBox();
			this._tbOpenDel = new System.Windows.Forms.TextBox();
			this._lbAnaFile = new System.Windows.Forms.Label();
			this._tipTbAmbigMarker = new System.Windows.Forms.ToolTip(this.components);
			this._lbAmbMkr = new System.Windows.Forms.Label();
			this._lbOpenDel = new System.Windows.Forms.Label();
			this._tipTbOpenDel = new System.Windows.Forms.ToolTip(this.components);
			this._tbCloseDel = new System.Windows.Forms.TextBox();
			this._tbAffixSep = new System.Windows.Forms.TextBox();
			this._btnSelect = new System.Windows.Forms.Button();
			this._lbCloseDel = new System.Windows.Forms.Label();
			this._tipTbCloseDel = new System.Windows.Forms.ToolTip(this.components);
			this._chBxCategories = new System.Windows.Forms.CheckedListBox();
			this._lbAffixSep = new System.Windows.Forms.Label();
			this._tipTbAffixSep = new System.Windows.Forms.ToolTip(this.components);
			this._tipChBxCategories = new System.Windows.Forms.ToolTip(this.components);
			this._lbCatAnal = new System.Windows.Forms.Label();
			this._tipBtnSelect = new System.Windows.Forms.ToolTip(this.components);
			this._helpMeOne = new System.Windows.Forms.HelpProvider();
			this._tipBtnOk = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			//
			// btnAnal
			//
			this._btnAnal.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this._btnAnal, "_btnAnal");
			this._btnAnal.Name = "_btnAnal";
			this._tipBtnAnal.SetToolTip(this._btnAnal, resources.GetString("btnAnal.ToolTip"));
			this._btnAnal.Click += new System.EventHandler(this.BtnAnalClick);
			//
			// btnClose
			//
			this._btnClose.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
			this._btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._helpMeOne.SetHelpString(this._btnClose, resources.GetString("btnClose.HelpString"));
			resources.ApplyResources(this._btnClose, "_btnClose");
			this._btnClose.Name = "_btnClose";
			this._helpMeOne.SetShowHelp(this._btnClose, ((bool)(resources.GetObject("btnClose.ShowHelp"))));
			this._tipBtnAnal.SetToolTip(this._btnClose, resources.GetString("btnClose.ToolTip"));
			this._tipBtnClose.SetToolTip(this._btnClose, resources.GetString("btnClose.ToolTip1"));
			this._btnClose.Click += new System.EventHandler(this.BtnCloseClick);
			//
			// tbANAFile
			//
			this._tbAnaFile.AcceptsReturn = true;
			resources.ApplyResources(this._tbAnaFile, "_tbAnaFile");
			this._helpMeOne.SetHelpString(this._tbAnaFile, resources.GetString("tbANAFile.HelpString"));
			this._tbAnaFile.Name = "_tbAnaFile";
			this._helpMeOne.SetShowHelp(this._tbAnaFile, ((bool)(resources.GetObject("tbANAFile.ShowHelp"))));
			this._tipBtnAnal.SetToolTip(this._tbAnaFile, resources.GetString("tbANAFile.ToolTip"));
			this._tipTbAnaFile.SetToolTip(this._tbAnaFile, resources.GetString("tbANAFile.ToolTip1"));
			//
			// btnBrowse
			//
			this._helpMeOne.SetHelpString(this._btnBrowse, resources.GetString("btnBrowse.HelpString"));
			resources.ApplyResources(this._btnBrowse, "_btnBrowse");
			this._btnBrowse.Name = "_btnBrowse";
			this._helpMeOne.SetShowHelp(this._btnBrowse, ((bool)(resources.GetObject("btnBrowse.ShowHelp"))));
			this._tipBtnClose.SetToolTip(this._btnBrowse, resources.GetString("btnBrowse.ToolTip"));
			this._tipBtnBrowse.SetToolTip(this._btnBrowse, resources.GetString("btnBrowse.ToolTip1"));
			this._btnBrowse.Click += new System.EventHandler(this.BtnBrowseClick);
			//
			// tbAmbigMarker
			//
			resources.ApplyResources(this._tbAmbigMarker, "_tbAmbigMarker");
			this._tbAmbigMarker.Name = "_tbAmbigMarker";
			this._tipBtnBrowse.SetToolTip(this._tbAmbigMarker, resources.GetString("tbAmbigMarker.ToolTip"));
			this._tipTbAmbigMarker.SetToolTip(this._tbAmbigMarker, resources.GetString("tbAmbigMarker.ToolTip1"));
			this._tbAmbigMarker.Validating += new System.ComponentModel.CancelEventHandler(this.TbAmbigMarkerValidating);
			//
			// tbOpenDel
			//
			resources.ApplyResources(this._tbOpenDel, "_tbOpenDel");
			this._tbOpenDel.Name = "_tbOpenDel";
			this._tipBtnBrowse.SetToolTip(this._tbOpenDel, resources.GetString("tbOpenDel.ToolTip"));
			this._tipTbOpenDel.SetToolTip(this._tbOpenDel, resources.GetString("tbOpenDel.ToolTip1"));
			this._tbOpenDel.Validating += new System.ComponentModel.CancelEventHandler(this.TbOpenDelValidating);
			//
			// lbANAFile
			//
			resources.ApplyResources(this._lbAnaFile, "_lbAnaFile");
			this._lbAnaFile.Name = "_lbAnaFile";
			//
			// lbAmbMkr
			//
			resources.ApplyResources(this._lbAmbMkr, "_lbAmbMkr");
			this._lbAmbMkr.Name = "_lbAmbMkr";
			//
			// lbOpenDel
			//
			resources.ApplyResources(this._lbOpenDel, "_lbOpenDel");
			this._lbOpenDel.Name = "_lbOpenDel";
			//
			// tbCloseDel
			//
			resources.ApplyResources(this._tbCloseDel, "_tbCloseDel");
			this._tbCloseDel.Name = "_tbCloseDel";
			this._tipTbOpenDel.SetToolTip(this._tbCloseDel, resources.GetString("tbCloseDel.ToolTip"));
			this._tipTbCloseDel.SetToolTip(this._tbCloseDel, resources.GetString("tbCloseDel.ToolTip1"));
			this._tbCloseDel.Validating += new System.ComponentModel.CancelEventHandler(this.TbCloseDelValidating);
			//
			// tbAffixSep
			//
			resources.ApplyResources(this._tbAffixSep, "_tbAffixSep");
			this._tbAffixSep.Name = "_tbAffixSep";
			this._tipTbOpenDel.SetToolTip(this._tbAffixSep, resources.GetString("tbAffixSep.ToolTip"));
			this._tipTbAffixSep.SetToolTip(this._tbAffixSep, resources.GetString("tbAffixSep.ToolTip1"));
			this._tbAffixSep.Validating += new System.ComponentModel.CancelEventHandler(this.TbAffixSepValidating);
			//
			// btnSelect
			//
			resources.ApplyResources(this._btnSelect, "_btnSelect");
			this._btnSelect.Name = "_btnSelect";
			this._tipTbOpenDel.SetToolTip(this._btnSelect, resources.GetString("btnSelect.ToolTip"));
			this._tipBtnSelect.SetToolTip(this._btnSelect, resources.GetString("btnSelect.ToolTip1"));
			this._btnSelect.Click += new System.EventHandler(this.BtnSelectClick);
			//
			// lbCloseDel
			//
			resources.ApplyResources(this._lbCloseDel, "_lbCloseDel");
			this._lbCloseDel.Name = "_lbCloseDel";
			//
			// chBxCategories
			//
			resources.ApplyResources(this._chBxCategories, "_chBxCategories");
			this._chBxCategories.MultiColumn = true;
			this._chBxCategories.Name = "_chBxCategories";
			this._tipChBxCategories.SetToolTip(this._chBxCategories, resources.GetString("chBxCategories.ToolTip"));
			this._tipTbCloseDel.SetToolTip(this._chBxCategories, resources.GetString("chBxCategories.ToolTip1"));
			//
			// lbAffixSep
			//
			resources.ApplyResources(this._lbAffixSep, "_lbAffixSep");
			this._lbAffixSep.Name = "_lbAffixSep";
			//
			// lbCatAnal
			//
			resources.ApplyResources(this._lbCatAnal, "_lbCatAnal");
			this._lbCatAnal.Name = "_lbCatAnal";
			//
			// tipBtnOK
			//
			this._tipBtnOk.ShowAlways = true;
			//
			// ANAConverterDlg
			//
			this.AcceptButton = this._btnAnal;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this._btnClose;
			this.Controls.Add(this._btnSelect);
			this.Controls.Add(this._lbCatAnal);
			this.Controls.Add(this._chBxCategories);
			this.Controls.Add(this._lbAffixSep);
			this.Controls.Add(this._tbAffixSep);
			this.Controls.Add(this._tbCloseDel);
			this.Controls.Add(this._tbOpenDel);
			this.Controls.Add(this._tbAmbigMarker);
			this.Controls.Add(this._tbAnaFile);
			this.Controls.Add(this._lbCloseDel);
			this.Controls.Add(this._lbOpenDel);
			this.Controls.Add(this._lbAmbMkr);
			this.Controls.Add(this._lbAnaFile);
			this.Controls.Add(this._btnBrowse);
			this.Controls.Add(this._btnClose);
			this.Controls.Add(this._btnAnal);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AnaConverterDlg";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		internal string ParametersPathname { get; private set; }

		internal string AnaPathname
		{
			get { return _tbAnaFile.Text; }
		}

		// <summary
		// ResetCategories - get categories from an ana file and display them.
		// </summary>
		private void ResetCategories()
		{
			_chBxCategories.BeginUpdate();
			StreamReader reader = null;

			try
			{
				reader = new StreamReader(_tbAnaFile.Text);
				var analysisLines = new StringCollection();
				var categoryLines = new StringCollection();
				var categories = new StringCollection();
				var line = reader.ReadLine();

				while (line != null)
				{
					// If the line contains %0%, then skip it, as it is a failure
					// and has no useful information in it.
					var failure = (line.IndexOf(String.Format("{0}0{1}", AmbiguityMarker, AmbiguityMarker), StringComparison.OrdinalIgnoreCase) > -1);
					if (line.StartsWith(@"\a")
						&& !analysisLines.Contains(line)
						&& !failure)
						analysisLines.Add(line); // Save unique \a lines.
					else if (line.StartsWith(@"\cat")
						&& !categoryLines.Contains(line)
						&& !failure)
						categoryLines.Add(line); // Save unique \cat lines.
					line = reader.ReadLine();
				}
				reader.Close();
				reader = null;

				// Process \a lines.
				foreach (var tokens in from string aLine in analysisLines select aLine.Split(OpenDelimiter[0]))
				{
					for (var i = 1; i < tokens.Length; ++i)
					{
						var str = tokens[i];
						// Find end of roots.
						str = str.Substring(0, str.IndexOf(CloseDelimiter[0])).Trim();
						// It can have compound roots, so get category from each root.
						var roots = str.Split();
						for (var j = 0; j < roots.Length; j = ++j + 1)
						{
							var category = roots[j];
							if (!categories.Contains(category))
								categories.Add(category);
						}
					}
				}

				// Process \cat lines.
				foreach (var catsMain in from string catLine in categoryLines select catLine.Substring(5).Split(AmbiguityMarker[0]))
				{
					for (var i = (catsMain.Length == 1) ? 0 : 2; // Start with 0, if it wasn't ambiguous. otherwise start at 2.
						 i < catsMain.Length;
						 ++i)
					{
						var catsInner = catsMain[i].Split();
						for (var j = 0; j < catsInner.Length; ++j)
						{
							if ((j % 2) == 0)
							{
								// Final word-level category
								var cat = catsInner[j];
								if (!categories.Contains(cat))
									categories.Add(cat);
							}
							else
							{
								// We want the root categories, but not affix categories.
								var catsInnermost = catsInner[j].Split('=');
								foreach (var cat in catsInnermost.Where(cat => cat.IndexOf("/", StringComparison.OrdinalIgnoreCase) == -1 && !categories.Contains(cat)))
								{
									categories.Add(cat);
								}
							}
						}
					}
				}

				// Add them to the control,and check them all.
				// TODO: Add a control to select/unselect all.
				foreach (var idx in from string cat in categories where cat != string.Empty select _chBxCategories.Items.Add(cat))
				{
					_chBxCategories.SetItemChecked(idx, true);
				}

			}
			finally
			{
				if (reader != null)
					reader.Close();
				_chBxCategories.EndUpdate();
			}
		}

		/// <summary>
		/// ValidateTextBox - Check for valid ana delimiter characters
		/// </summary>
		/// <param name="textBox"></param>
		/// <param name="defaultValue"></param>
		private static void ValidateTextBox(Control textBox, string defaultValue)
		{
			if (textBox.Text == string.Empty)
				textBox.Text = defaultValue;
		}

		#region Event handlers

		/// <summary>
		/// btnBrowse_Click - Browse for an ana file to analyze.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnBrowseClick(object sender, EventArgs e)
		{
			using (var openDlg = new OpenFileDialog())
			{
				openDlg.Filter = Resources.kFileTypes ;
				openDlg.Title = Resources.kAmpleAnalysisFile;
				_chBxCategories.Items.Clear();
				if(openDlg.ShowDialog() == DialogResult.OK)
				{
					_tbAnaFile.Text = openDlg.FileName;
					_btnSelect.Enabled = true;
					_btnAnal.Enabled = true;
					_chBxCategories.Enabled = true;
					ResetCategories();
				}
				else
				{
					_tbAnaFile.Text = "";
					_btnSelect.Enabled = false;
					_btnAnal.Enabled = false;
				}
			}
		}

		/// <summary>
		/// btnAnal_Click - Does the actual analysis.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnAnalClick(object sender, EventArgs e)
		{
			// User Defined parameters.
			var parms = new Parameters();
			ParametersPathname = (_tbAnaFile.Text.Split('.'))[0] + ".prm";
			parms.Marker.Ambiguity = AmbiguityMarker[0];
			parms.RootDelimiter.OpenDelimiter = OpenDelimiter[0];
			parms.RootDelimiter.CloseDelimiter = CloseDelimiter[0];
			parms.Marker.Decomposition = AffixSeparator[0];
			foreach (var t in _chBxCategories.CheckedItems)
				parms.Categories.Add(new Category(t.ToString()));
			parms.Serialize(ParametersPathname);

			Close();
		}

		/// <summary>
		/// btnClose_Click- Exit
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnCloseClick(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// tbAmbigMarker_Validating- Validate for default ambiguity marker "%".
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TbAmbigMarkerValidating(object sender, CancelEventArgs e)
		{
			ValidateTextBox(sender as TextBox, "%");
		}

		/// <summary>
		/// tbAffixSep_Validating - Validate for default affix separator char "-".
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TbAffixSepValidating(object sender, CancelEventArgs e)
		{
			ValidateTextBox(sender as TextBox, "-");
		}

		/// <summary>
		/// tbOpenDel_Validating - Validate for default opening char "%lt;".
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TbOpenDelValidating(object sender, CancelEventArgs e)
		{
			ValidateTextBox(sender as TextBox, "<");
		}

		/// <summary>
		/// tbCloseDel_Validating - Validate for default closing char ">".
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TbCloseDelValidating(object sender, CancelEventArgs e)
		{
			ValidateTextBox(sender as TextBox, ">");
		}

		/// <summary>
		/// btnSelect_Click - Select or unselect all categories.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnSelectClick(object sender, EventArgs e)
		{
			// Select or unselect them all.
			bool catChecked;

			if (_btnSelect.Text == Resources.kSelectAll)
			{
				catChecked = true;
				_btnSelect.Text = Resources.kDeSelectAll;
			}
			else
			{
				catChecked = false;
				_btnSelect.Text = Resources.kSelectAll;
			}

			_chBxCategories.BeginUpdate();
			for (var i = 0; i < _chBxCategories.Items.Count; ++i)
			{
				_chBxCategories.SetItemChecked(i, catChecked);
			}
			_chBxCategories.EndUpdate();
		}

	#endregion Event handlers
	}
}
