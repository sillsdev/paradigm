// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
namespace SIL.WordWorks.GAFAWS.AffixPositionAnalyzer
{
	/// <summary></summary>
	partial class AffixPositionAnalyzer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		//private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				//if (components != null)
				//	components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AffixPositionAnalyzer));
			this._btnProcess = new System.Windows.Forms.Button();
			this._btnClose = new System.Windows.Forms.Button();
			this._lvConverters = new System.Windows.Forms.ListView();
			this._chConverter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label1 = new System.Windows.Forms.Label();
			this._tbDescription = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			//
			// _btnProcess
			//
			this._btnProcess.Location = new System.Drawing.Point(110, 322);
			this._btnProcess.Name = "_btnProcess";
			this._btnProcess.Size = new System.Drawing.Size(75, 23);
			this._btnProcess.TabIndex = 0;
			this._btnProcess.Text = "Analyze...";
			this._btnProcess.UseVisualStyleBackColor = true;
			this._btnProcess.Click += new System.EventHandler(this.BtnProcessClick);
			//
			// _btnClose
			//
			this._btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._btnClose.Location = new System.Drawing.Point(219, 322);
			this._btnClose.Name = "_btnClose";
			this._btnClose.Size = new System.Drawing.Size(75, 23);
			this._btnClose.TabIndex = 1;
			this._btnClose.Text = "Close";
			this._btnClose.UseVisualStyleBackColor = true;
			this._btnClose.Click += new System.EventHandler(this.BtnCloseClick);
			//
			// _lvConverters
			//
			this._lvConverters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this._chConverter});
			this._lvConverters.FullRowSelect = true;
			this._lvConverters.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this._lvConverters.HideSelection = false;
			this._lvConverters.Location = new System.Drawing.Point(6, 24);
			this._lvConverters.MultiSelect = false;
			this._lvConverters.Name = "_lvConverters";
			this._lvConverters.Size = new System.Drawing.Size(176, 280);
			this._lvConverters.TabIndex = 2;
			this._lvConverters.UseCompatibleStateImageBehavior = false;
			this._lvConverters.View = System.Windows.Forms.View.Details;
			this._lvConverters.SelectedIndexChanged += new System.EventHandler(this.ConvertersSelectedIndexChanged);
			this._lvConverters.DoubleClick += new System.EventHandler(this.ConvertersDoubleClick);
			//
			// _chConverter
			//
			this._chConverter.Text = "Converter";
			this._chConverter.Width = 170;
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Available Converters";
			//
			// _tbDescription
			//
			this._tbDescription.Enabled = false;
			this._tbDescription.Location = new System.Drawing.Point(198, 24);
			this._tbDescription.Multiline = true;
			this._tbDescription.Name = "_tbDescription";
			this._tbDescription.Size = new System.Drawing.Size(203, 280);
			this._tbDescription.TabIndex = 4;
			//
			// AffixPositionAnalyzer
			//
			this.AcceptButton = this._btnClose;
			this.AccessibleName = "AffixPositionAnalyzer";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(426, 351);
			this.Controls.Add(this._tbDescription);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._lvConverters);
			this.Controls.Add(this._btnClose);
			this.Controls.Add(this._btnProcess);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AffixPositionAnalyzer";
			this.Text = "Affix Position Analyzer";
			this.Load += new System.EventHandler(this.AffixPositionAnalyzerLoad);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _btnProcess;
		private System.Windows.Forms.Button _btnClose;
		private System.Windows.Forms.ListView _lvConverters;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader _chConverter;
		private System.Windows.Forms.TextBox _tbDescription;
	}
}
