// --------------------------------------------------------------------------------------------
// Copyright (C) 2003-2013 SIL International. All rights reserved.
//
// Distributable under the terms of the MIT License, as specified in the license.rtf file.
// --------------------------------------------------------------------------------------------
namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	partial class Fw70ConverterDlg
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
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
			this._tvPoses = new System.Windows.Forms.TreeView();
			this.label1 = new System.Windows.Forms.Label();
			this._btnClose = new System.Windows.Forms.Button();
			this._tbPathname = new System.Windows.Forms.TextBox();
			this._btnBrowse = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// _tvPoses
			//
			this._tvPoses.FullRowSelect = true;
			this._tvPoses.HideSelection = false;
			this._tvPoses.Location = new System.Drawing.Point(13, 58);
			this._tvPoses.Name = "_tvPoses";
			this._tvPoses.Size = new System.Drawing.Size(259, 309);
			this._tvPoses.TabIndex = 0;
			this._tvPoses.DoubleClick += new System.EventHandler(this.TvPosesDoubleClick);
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 42);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Parts of Speech";
			//
			// _btnClose
			//
			this._btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._btnClose.Location = new System.Drawing.Point(20, 376);
			this._btnClose.Name = "_btnClose";
			this._btnClose.Size = new System.Drawing.Size(75, 23);
			this._btnClose.TabIndex = 2;
			this._btnClose.Text = "Analyze";
			this._btnClose.UseVisualStyleBackColor = true;
			this._btnClose.Click += new System.EventHandler(this.BtnCloseClick);
			//
			// _tbPathname
			//
			this._tbPathname.Enabled = false;
			this._tbPathname.Location = new System.Drawing.Point(13, 13);
			this._tbPathname.Name = "_tbPathname";
			this._tbPathname.Size = new System.Drawing.Size(199, 20);
			this._tbPathname.TabIndex = 3;
			//
			// _btnBrowse
			//
			this._btnBrowse.Location = new System.Drawing.Point(219, 9);
			this._btnBrowse.Name = "_btnBrowse";
			this._btnBrowse.Size = new System.Drawing.Size(62, 23);
			this._btnBrowse.TabIndex = 4;
			this._btnBrowse.Text = "Browse...";
			this._btnBrowse.UseVisualStyleBackColor = true;
			this._btnBrowse.Click += new System.EventHandler(this.BrowseBtnClick);
			//
			// btnCancel
			//
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(113, 375);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			//
			// Fw70ConverterDlg
			//
			this.AcceptButton = this._btnClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(284, 408);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this._btnBrowse);
			this.Controls.Add(this._tbPathname);
			this.Controls.Add(this._btnClose);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._tvPoses);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Fw70ConverterDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Prepare FieldWorks 7/8 xml file";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView _tvPoses;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _btnClose;
		private System.Windows.Forms.TextBox _tbPathname;
		private System.Windows.Forms.Button _btnBrowse;
		private System.Windows.Forms.Button btnCancel;
	}
}